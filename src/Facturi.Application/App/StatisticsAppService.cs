using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;
using Abp.Domain.Repositories;
using Facturi.App.Dtos.Statistics;
using Facturi.Core.App;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App
{
    public class StatisticsAppService : ApplicationService, IStatisticsAppService
    {
        private readonly IRepository<Devis, long> _estimateRepo;
        private readonly IRepository<Facture, long> _invoiceRepo;
        private readonly IRepository<Client, long> _clientRepo;
        private readonly IRepository<Catalogue, long> _catalogRepo;
        private readonly IRepository<FactureInfosPaiement, long> _factureInfosPaiementRepo;
        private readonly IRepository<InfosEntreprise, long> _infosEntrepriseRepo;
        private readonly IRepository<FactureItem, long> _invoiceItemRepo;
        private readonly IRepository<DevisItem, long> _estimateItemRepo;

        public StatisticsAppService(
                IRepository<Devis, long> estimateRepo,
                IRepository<Facture, long> invoiceRepo,
                IRepository<FactureItem, long> invoiceItemRepo,
                IRepository<Client, long> clientRepo,
                IRepository<Catalogue, long> catalogRepo,
                IRepository<FactureInfosPaiement, long> factureInfosPaiementRepo,
                IRepository<InfosEntreprise, long> infosEntrepriseRepo,
                IRepository<DevisItem, long> estimateItemRepo
            )
        {
            _estimateRepo = estimateRepo;
            _invoiceRepo = invoiceRepo;
            _clientRepo = clientRepo;
            _catalogRepo = catalogRepo;
            _factureInfosPaiementRepo = factureInfosPaiementRepo;
            _infosEntrepriseRepo = infosEntrepriseRepo;
            _invoiceItemRepo = invoiceItemRepo;
            _estimateItemRepo = estimateItemRepo;
        }

        public async Task<bool> CreateOrUpdateMonthTargetAmount(float amount)
        {
            try
            {
                var infoEnreprise = await _infosEntrepriseRepo
               .SingleAsync(e => e.CreatorUserId == AbpSession.UserId || e.LastModifierUserId == AbpSession.UserId);

                infoEnreprise.MonthTargetAmount = amount;

                await _infosEntrepriseRepo.UpdateAsync(infoEnreprise);

                return true;
            }
            catch(Exception)
            {
                return false;
            }
           
        }

        public async Task<ListResultDto<ActivityLogDto>> GetActivityLog()
        {
            var activityLogList = new List<ActivityLogDto>();
            var lastEstimate = await _estimateRepo.GetAllIncluding(e => e.Client, e => e.DevisItems)
                .Where(e => (e.CreatorUserId == AbpSession.UserId || e.LastModifierUserId == AbpSession.UserId))
                .OrderByDescending(e => e.CreationTime).FirstOrDefaultAsync();
            

            activityLogList.Add(new()
            {
                LogType = "estimate",
                ClientName = lastEstimate.Client.Nom != "" ? lastEstimate.Client.Nom : lastEstimate.Client.RaisonSociale,
                Reference = lastEstimate.Reference,
                Date = lastEstimate.CreationTime,
                Amount = lastEstimate.DevisItems.Sum(e => e.TotalTtc - ((e.Quantity * e.UnitPriceHT) * e.Devis.Remise / 100)),
                Currency = lastEstimate.Currency
            });

            var lastInvoice = _invoiceRepo.GetAllIncluding(e => e.Client, e => e.FactureItems)
                .Where(e => (e.CreatorUserId == AbpSession.UserId || e.LastModifierUserId == AbpSession.UserId))
                .OrderByDescending(e => e.CreationTime).FirstOrDefault();

            activityLogList.Add(new()
            {
                LogType = "invoice",
                ClientName = lastInvoice.Client.Nom != "" ? lastInvoice.Client.Nom : lastInvoice.Client.RaisonSociale,
                Reference = lastInvoice.Reference,
                Date = lastInvoice.CreationTime,
                Amount = lastInvoice.FactureItems.Sum(e => e.TotalTtc - ((e.Quantity * e.UnitPriceHT) * e.Facture.Remise / 100)),
                Currency = lastInvoice.Currency
            });

            var lastPayment = _factureInfosPaiementRepo.GetAll()
                .Where(e => (e.CreatorUserId == AbpSession.UserId || e.LastModifierUserId == AbpSession.UserId))
                .OrderByDescending(e => e.DatePaiement).FirstOrDefault();

            var invoice = _invoiceRepo.GetAllIncluding(e => e.Client, e => e.FactureItems).SingleOrDefault(e => e.Id == lastPayment.FactureId);
            activityLogList.Add(new()
            {
                LogType = "payment",
                ClientName = invoice.Client.Nom != "" ? invoice.Client.Nom : invoice.Client.RaisonSociale,
                Reference = invoice.Reference,
                Date = lastPayment.DatePaiement,
                Amount = lastPayment.MontantPaye,
                Currency = invoice.Currency
            });

            var lastClient = _clientRepo.GetAll()
                .Where(e => (e.CreatorUserId == AbpSession.UserId || e.LastModifierUserId == AbpSession.UserId))
                .OrderByDescending(e => e.CreationTime).FirstOrDefault();
            activityLogList.Add(new()
            {
                LogType = "client",
                ClientName = lastClient.Nom != "" ? lastClient.Nom : lastClient.RaisonSociale,
                Reference = formatReferenceNumber(lastClient.Reference, 'C'),
                Date = lastClient.CreationTime,

                Amount = 0,
                Currency = null
            });

            var lastCatalog = _catalogRepo.GetAll().OrderByDescending(e => e.CreationTime).FirstOrDefault();
            activityLogList.Add(new()
            {
                LogType = "catalog",
                ClientName = null,
                Reference = formatReferenceNumber(lastCatalog.Reference, 'P'),
                Date = lastCatalog.CreationTime,
                Amount = 0,
                Currency = null
            });

            return new ListResultDto<ActivityLogDto>(activityLogList);

        }

        public async Task<MonthTargetInfosDto> GetMonthTargetInfos()
        {
            var infoEnreprise = await _infosEntrepriseRepo
              .SingleAsync(e => e.CreatorUserId == AbpSession.UserId || e.LastModifierUserId == AbpSession.UserId);

            return (new MonthTargetInfosDto()
            {
                MonthTargetAmount = infoEnreprise.MonthTargetAmount,
                AccomplishedAmount = await _factureInfosPaiementRepo.GetAll()
                .Where(e => e.CreatorUserId == AbpSession.UserId || e.LastModifierUserId == AbpSession.UserId)
                .SumAsync(e => e.MontantPaye)
            });

        }

        public async Task<PeriodicTrackingDto> GetPeriodicTrackingInfos(PeriodicTrackingInputDto periodicTrackingInputDto)
        {
            
            try
            {
                var actualDay = DateTime.Now.Day;
                var actualMonth = DateTime.Now.Month;
                var actualYear = DateTime.Now.Year;
                var daysInActualMonth = DateTime.DaysInMonth(actualYear, actualMonth);

                await _invoiceItemRepo.GetAllIncluding(e => e.Facture)
                .Where(e => e.Facture.CreatorUserId == AbpSession.UserId || e.Facture.LastModifierUserId == AbpSession.UserId)
                .Where(e => e.Facture.Statut != FactureStatutEnum.Cree)
                .SumAsync(e => e.TotalTtc - ((e.Quantity * e.UnitPriceHT) * e.Facture.Remise / 100));

                var allInvoices = ( _invoiceItemRepo.GetAllIncluding(e => e.Facture)
                .Where(e => (e.Facture.CreatorUserId == AbpSession.UserId || e.Facture.LastModifierUserId == AbpSession.UserId)
                       && e.Facture.Statut != FactureStatutEnum.Cree)
                .WhereIf(periodicTrackingInputDto.DateStart == null && periodicTrackingInputDto.DateEnd == null,
                    e => e.Facture.CreationTime >= new DateTime(actualYear, actualMonth, 1) 
                    && e.Facture.CreationTime <= new DateTime(actualYear, actualMonth, daysInActualMonth))
                .WhereIf(periodicTrackingInputDto.DateStart != null, e => e.Facture.CreationTime >= periodicTrackingInputDto.DateStart)
                .WhereIf(periodicTrackingInputDto.DateEnd != null, e => e.Facture.CreationTime <= periodicTrackingInputDto.DateEnd));


                var invoicePeriodicTracking = new InvoicePeriodicTrackingDto()

                {
                    TotalInvoicesAmount = await allInvoices.SumAsync(e => e.TotalTtc - ((e.Quantity * e.UnitPriceHT) * e.Facture.Remise / 100)),
                    CashedInvoicesAmount = await allInvoices
                        .Where(e => e.Facture.Statut == FactureStatutEnum.ReglePartiellemt || e.Facture.Statut == FactureStatutEnum.Regle)
                        .SumAsync(e => e.TotalTtc - ((e.Quantity * e.UnitPriceHT) * e.Facture.Remise / 100)),
                    PendingInvoicesAmount = await allInvoices
                        .Where(e => e.Facture.Statut == FactureStatutEnum.Valide
                            && DateTime.Compare(e.Facture.DateEmission.AddDays(e.Facture.EcheancePaiement), DateTime.Now) > 0)
                        .SumAsync(e => e.TotalTtc - ((e.Quantity * e.UnitPriceHT) * e.Facture.Remise / 100)),
                    LateInvoicesAmount = await allInvoices
                        .Where(e => e.Facture.Statut == FactureStatutEnum.Valide
                             && DateTime.Compare(e.Facture.DateEmission.AddDays(e.Facture.EcheancePaiement), DateTime.Now) < 0)
                        .SumAsync(e => e.TotalTtc - ((e.Quantity * e.UnitPriceHT) * e.Facture.Remise / 100)),

                };

                var allEstimates = (_estimateItemRepo.GetAllIncluding(e => e.Devis)
                   .Where(e => (e.Devis.CreatorUserId == AbpSession.UserId || e.Devis.LastModifierUserId == AbpSession.UserId)
                       && e.Devis.Statut != DevisStatutEnum.Cree)
                   .WhereIf(periodicTrackingInputDto.DateStart == null && periodicTrackingInputDto.DateEnd == null,
                        e => e.Devis.CreationTime >= new DateTime(actualYear, actualMonth, 1)
                        && e.Devis.CreationTime <= new DateTime(actualYear, actualMonth, daysInActualMonth))
                   .WhereIf(periodicTrackingInputDto.DateStart != null, e => e.Devis.CreationTime >= periodicTrackingInputDto.DateStart)
                   .WhereIf(periodicTrackingInputDto.DateEnd != null, e => e.Devis.CreationTime <= periodicTrackingInputDto.DateEnd));


                var estimatePeriodicTracking = new EstimatePeriodicTrackingDto()
                {
                    TotalEstimatesAmount = await allEstimates.SumAsync(e => e.TotalTtc - ((e.Quantity * e.UnitPriceHT) * e.Devis.Remise / 100)),
                    TransformedInvoicesAmount = await allEstimates.Where(e => e.Devis.Statut == DevisStatutEnum.Converti)
                        .SumAsync(e => e.TotalTtc - ((e.Quantity * e.UnitPriceHT) * e.Devis.Remise / 100))
                };

                var invoiceItemsEnumerable = _invoiceItemRepo.GetAllIncluding(e => e.Facture, e => e.Facture.Client)
                 .Where(e => e.Facture.CreatorUserId == AbpSession.UserId || e.Facture.LastModifierUserId == AbpSession.UserId)
                 .AsEnumerable();

                var bestsellerInvoiceItems = invoiceItemsEnumerable
                 .Where(e => (e.Facture.Statut == FactureStatutEnum.Regle || e.Facture.Statut == FactureStatutEnum.ReglePartiellemt) && e.CatalogueId != 0)
                 .GroupBy(e => new {e.CatalogueId, e.Designation})
                 .Select(e => new { Designation = e.Key.Designation, Total =  e.Sum(x => x.TotalTtc)})
                 .OrderByDescending(e => e.Total)
                 .Take(5)
                 .Select(e => new BestsellerPeriodicTrackingDto() { CatalogName = e.Designation, Amount = e.Total})
                 .ToList();

                var topClients = invoiceItemsEnumerable
                 .GroupBy(e => new { ClientId = e.Facture.ClientId, Name = (e.Facture.Client.Nom == "" ? e.Facture.Client.RaisonSociale : e.Facture.Client.Nom) })
                 .Select(e => new { Name = e.Key.Name, Total = e.Sum(x => x.TotalTtc) })
                 .OrderByDescending(e => e.Total)
                 .Take(5)
                 .Select(e => new TopClientsPeriodicTrackingDto() { ClientName = e.Name, Amount = e.Total })
                 .ToList();
                
                return (new PeriodicTrackingDto()
                {
                    InvoicePeriodicTrackingDto = invoicePeriodicTracking,
                    EstimatePeriodicTrackingDto = estimatePeriodicTracking,
                    BestsellerPeriodicTrackingDto = bestsellerInvoiceItems,
                    TopClientsPeriodicTrackingDto = topClients
                });
    
            }
           
            catch(Exception e)
            {
                return null;
            }

        }

        public async Task<TotalStatisticsDto> GetTotalStatistics()
        {
            var issuedInvoicesSum = await _invoiceItemRepo.GetAllIncluding(e => e.Facture)
                .Where(e => e.Facture.CreatorUserId == AbpSession.UserId || e.Facture.LastModifierUserId == AbpSession.UserId)
                .Where(e => e.Facture.Statut != FactureStatutEnum.Cree)
                .SumAsync(e => e.TotalTtc - ((e.Quantity * e.UnitPriceHT) * e.Facture.Remise / 100));

            var issuedEstimatessSum = await _estimateItemRepo.GetAllIncluding(e => e.Devis)
               .Where(e => e.Devis.CreatorUserId == AbpSession.UserId || e.Devis.LastModifierUserId == AbpSession.UserId)
               .Where(e => e.Devis.Statut != DevisStatutEnum.Cree || e.Devis.Statut == DevisStatutEnum.rejete)
               .SumAsync(e => e.TotalTtc - ((e.Quantity * e.UnitPriceHT) * e.Devis.Remise / 100));

            var topProducts = await _catalogRepo.GetAll()
                .Where(e => e.CreatorUserId == AbpSession.UserId || e.LastModifierUserId == AbpSession.UserId)
                .Distinct().CountAsync();

            var topClients = await _clientRepo.GetAll()
             .Where(e => e.CreatorUserId == AbpSession.UserId || e.LastModifierUserId == AbpSession.UserId)
             .Distinct().CountAsync();

            return new TotalStatisticsDto()
            {
                IssuedInvoices = issuedInvoicesSum,
                IssuedEstimates = issuedEstimatessSum,
                Products = topProducts,
                Clients = topClients
            };
            

        }

        private string formatReferenceNumber(int referenceNumber, char referencePrefix)
        {
            string reference = referenceNumber + "";
            while(reference.Length < 5)
            {
                reference = "0" + reference;
            }

            return referencePrefix + reference;
        }
    
    }
}
