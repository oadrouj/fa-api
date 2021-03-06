using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Facturi.App.Dtos;
using Facturi.App.Dtos.GenericDtos;
using Facturi.App.Dtos.InvoiceDtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Facturi.App
{
    public class FactureAppService : ApplicationService, IFactureAppService
    {

        private readonly IRepository<Facture, long> _factureRepository;
        private readonly IRepository<FactureInfosPaiement, long> _factureInfosPaiementRepository;
        private readonly IRepository<FactureItem, long> _factureItemRepository;
        private readonly IReportGeneratorAppService _reportGeneratorAppService;
        private readonly IRepository<Client, long> _clientRepository;
        private readonly IInfosEntrepriseAppService _infosEntrepriseAppService;


        public FactureAppService(
            IRepository<Facture, long> FactureRepository, 
            IRepository<FactureItem, long> factureItemRepository,
            IReportGeneratorAppService reportGeneratorAppService, 
            IRepository<FactureInfosPaiement, long> factureInfosPaiementRepository,
            IRepository<Client, long> clientRepository,
            IInfosEntrepriseAppService infosEntrepriseAppService
            )
        {
            _factureRepository = FactureRepository ?? throw new ArgumentNullException(nameof(FactureRepository));
            _factureItemRepository = factureItemRepository ?? throw new ArgumentNullException(nameof(factureItemRepository));
            _reportGeneratorAppService = reportGeneratorAppService ?? throw new ArgumentNullException(nameof(reportGeneratorAppService));
            _factureInfosPaiementRepository = factureInfosPaiementRepository ?? throw new ArgumentNullException(nameof(factureInfosPaiementRepository));
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
            _infosEntrepriseAppService = infosEntrepriseAppService ?? throw new ArgumentNullException(nameof(infosEntrepriseAppService));
        }

        public async Task<long> CreateFacture(CreateFactureInput input)
        {
            try
            {
                //Gerer reference
                var facture = ObjectMapper.Map<Facture>(input);
                var newFactureId = await _factureRepository.InsertAndGetIdAsync(facture);

                return newFactureId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> UpdateFacture(UpdateFactureInput input)
        {
            try
            {
                var factureItemsToDelete = _factureItemRepository.GetAll().Where(di => di.FactureId == input.Id).Select(di => di.Id).ToArray();
                if (factureItemsToDelete != null && factureItemsToDelete.Length > 0)
                {
                    foreach (long factureItemId in factureItemsToDelete)
                    {
                        _factureItemRepository.Delete(factureItemId);
                    }
                }

                var facture = ObjectMapper.Map<Facture>(input);
                await _factureRepository.InsertOrUpdateAsync(facture);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<FactureDto> GetByIdFacture(long id)
        {
            var facture = await _factureRepository.GetAllIncluding(f => f.FactureItems, f => f.Client)
                .Where(f => (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId) && f.Id == id)
                .ToListAsync();
            var result = ObjectMapper.Map<FactureDto>(facture.First());
            return result;
        }

        public async Task<bool> DeleteFacture(long FactureId)
        {
            try
            {
                var factureItemsToDelete = _factureItemRepository.GetAll().Where(di => di.FactureId == FactureId).Select(di => di.Id).ToArray();
                if (factureItemsToDelete != null && factureItemsToDelete.Length > 0)
                {
                    foreach (long factureItemId in factureItemsToDelete)
                    {
                        await _factureItemRepository.DeleteAsync(factureItemId);
                    }
                }
                await _factureRepository.DeleteAsync(FactureId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetLastReference()
        {
            var facture =  (await _factureRepository.GetAllListAsync())
                .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId)
                    && (Regex.IsMatch(d.Reference, @"^F[0-9]{5}"))).OrderByDescending(d => d.Reference).ToList();

            if (facture != null && facture.Any() && Int32.TryParse(facture.First().Reference.Substring(1), out int reference))
                return reference;
            else
                return 0;
        }

        public async Task<InvoiceInitiationDto> GetLastReferenceWithIntroMessageAndFooter()
        {
            var facture = (await _factureRepository.GetAllListAsync())
               .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId)
                   && (Regex.IsMatch(d.Reference, @"^F[0-9]{5}"))).OrderByDescending(d => d.Reference).ToList();

           var defaultAnnotations = await _infosEntrepriseAppService.GetDefaultAnnotations();
            var estimateInitiation = new InvoiceInitiationDto();

            if (defaultAnnotations == null)
            {
                estimateInitiation.InvoiceIntroMessage = null;
                estimateInitiation.InvoiceFooter = null;
            }
            else
            {
                estimateInitiation.InvoiceIntroMessage = defaultAnnotations.EstimateIntroMessage;
                estimateInitiation.InvoiceFooter = defaultAnnotations.EstimateFooter;
            }

            if (facture != null && facture.Any() && Int32.TryParse(facture.First().Reference.Substring(1), out int reference))
                estimateInitiation.LastReference = reference;
            else
                estimateInitiation.LastReference = 0;

            return estimateInitiation;
        }
        public async Task<bool> ChangeFactureStatut(long FactureId, FactureStatutEnum statut)
        {
            try
            {
                var facture = (await _factureRepository.GetAllIncluding(f => f.FactureItems, f => f.Client)
                                .Where(f => (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId) && f.Id == FactureId)
                                .ToListAsync()).First();
                facture.Statut = statut;
                await _factureRepository.UpdateAsync(facture);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ListResultDto<FactureDto>> GetAllFacture(CriteriasDto factureCriterias)
        {
            CheckIfIsRefSearch(factureCriterias, out bool isRef, out int minRef, out int maxRef);
            CheckIfIsFilterSearch(factureCriterias, out long client, out DateTime[] dateEmission, out int echeancePaiement,
            out float montantTtc, out FactureStatutEnum statut);

            var FactureList = new List<Facture>();
           
            var query = _factureRepository.GetAllIncluding(f => f.FactureItems, f => f.Client)
                .Where(f => (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId))
                //.WhereIf(factureCriterias.GlobalFilter != null & !isRef,
                //    f => f.Client.Nom.Trim().Contains(factureCriterias.GlobalFilter.Trim())
                //    || f.Client.RaisonSociale.Trim().Contains(factureCriterias.GlobalFilter.Trim()))
                .WhereIf(factureCriterias.GlobalFilter != null, f => f.Reference.StartsWith(factureCriterias.GlobalFilter))
                .WhereIf(client != 0, f => f.ClientId == client)
                .WhereIf(dateEmission != null, f => f.DateEmission.Date >= dateEmission[0] && f.DateEmission.Date <= dateEmission[1])
                .WhereIf(echeancePaiement != 0, f => f.EcheancePaiement == echeancePaiement)
                .WhereIf(montantTtc != -1, f => f.FactureItems.Sum(item => item.TotalTtc) == montantTtc)
                .WhereIf(statut != FactureStatutEnum.Undefined && statut != FactureStatutEnum.PaiementAttente
                    && statut != FactureStatutEnum.PaiementRetard, f => f.Statut == statut)
                .WhereIf(statut == FactureStatutEnum.PaiementAttente, e => e.Statut == FactureStatutEnum.Valide &&
                    DateTime.Compare(e.DateEmission.AddDays(e.EcheancePaiement), DateTime.Now) > 0)
                .WhereIf(statut == FactureStatutEnum.PaiementRetard, e => e.Statut == FactureStatutEnum.Valide &&
                    DateTime.Compare(e.DateEmission.AddDays(e.EcheancePaiement), DateTime.Now) < 0)
                .OrderByDescending(f => f.LastModificationTime);

            if (factureCriterias.SortField != null && factureCriterias.SortField.Length != 0)
            {
                switch (factureCriterias.SortField)
                {
                    case "reference":
                        if (factureCriterias.SortOrder.Equals("1")) { FactureList = await query.OrderBy(f => f.Reference).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        else if (factureCriterias.SortOrder.Equals("-1")) { FactureList = await query.OrderByDescending(f => f.Reference).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        break;
                    case "client":
                        if (factureCriterias.SortOrder.Equals("1")) { FactureList = await query.OrderBy(f => f.Client.RaisonSociale + f.Client.Nom).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        else if (factureCriterias.SortOrder.Equals("-1")) { FactureList = await query.OrderByDescending(f => f.Client.Nom + f.Client.RaisonSociale).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        break;
                    case "dateEmission":
                        if (factureCriterias.SortOrder.Equals("1")) { FactureList = await query.OrderBy(f => f.DateEmission).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        else if (factureCriterias.SortOrder.Equals("-1")) { FactureList = await query.OrderByDescending(f => f.DateEmission).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        break;
                    case "montantTtc":
                        if (factureCriterias.SortOrder.Equals("1")) { FactureList = await query.OrderBy(f => f.MontantTtc).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        else if (factureCriterias.SortOrder.Equals("-1")) { FactureList = await query.OrderByDescending(f => f.MontantTtc).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        break;
                    case "statut":
                        if (factureCriterias.SortOrder.Equals("1")) { FactureList = await query.OrderBy(f => f.Statut).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        else if (factureCriterias.SortOrder.Equals("-1")) { FactureList = await query.OrderByDescending(f => f.Statut).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        break;
                    default:
                        FactureList = await query.OrderBy(d => d.EcheancePaiement).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync();
                        break;
                }

            }
            else
            {
                FactureList = await query.OrderBy(d => d.EcheancePaiement).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync();
            }

            var result = new ListResultDto<FactureDto>(ObjectMapper.Map<List<FactureDto>>(FactureList));
            return result;
        }

        public async Task<int> GetAllFactureTotalRecords(CriteriasDto factureCriterias)
        {
            CheckIfIsRefSearch(factureCriterias, out bool isRef, out int minRef, out int maxRef);
            CheckIfIsFilterSearch(factureCriterias, out long client, out DateTime[] dateEmission, out int echeancePaiement,
            out float montantTtc, out FactureStatutEnum statut);

            var query = _factureRepository.GetAllIncluding(f => f.FactureItems, f => f.Client)
                .Where(f => (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId))
                //.WhereIf(factureCriterias.GlobalFilter != null & !isRef,
                //    f => f.Client.Nom.Trim().Contains(factureCriterias.GlobalFilter.Trim())
                //    || f.Client.RaisonSociale.Trim().Contains(factureCriterias.GlobalFilter.Trim()))
                .WhereIf(factureCriterias.GlobalFilter != null, f => f.Reference.StartsWith(factureCriterias.GlobalFilter))
                .WhereIf(client != 0, f => f.ClientId == client)
                .WhereIf(dateEmission != null, f => f.DateEmission >= dateEmission[0] && f.DateEmission <= dateEmission[1])
                 .WhereIf(echeancePaiement != 0, f => f.EcheancePaiement == echeancePaiement)
                 .WhereIf(montantTtc != -1, f => f.FactureItems.Sum(item => item.TotalTtc) == montantTtc)
                  .WhereIf(statut != FactureStatutEnum.Undefined && statut != FactureStatutEnum.PaiementAttente
                    && statut != FactureStatutEnum.PaiementRetard, f => f.Statut == statut)
                .WhereIf(statut == FactureStatutEnum.PaiementAttente, e => e.Statut == FactureStatutEnum.Valide &&
                    DateTime.Compare(e.DateEmission.AddDays(e.EcheancePaiement), DateTime.Now) > 0)
                .WhereIf(statut == FactureStatutEnum.PaiementRetard, e => e.Statut == FactureStatutEnum.Valide &&
                    DateTime.Compare(e.DateEmission.AddDays(e.EcheancePaiement), DateTime.Now) < 0)
                .OrderByDescending(f => f.LastModificationTime);

            return await query.CountAsync();
        }

        public async Task<float> GetAllFactureMontantTotal(CriteriasDto factureCriterias)
        {
            CheckIfIsRefSearch(factureCriterias, out bool isRef, out int minRef, out int maxRef);
            CheckIfIsFilterSearch(factureCriterias, out long client, out DateTime[] dateEmission, out int echeancePaiement,
            out float montantTtc, out FactureStatutEnum statut);
            
            var query = _factureRepository.GetAllIncluding(f => f.FactureItems, f => f.Client)
                .Where(f => (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId))
                //.WhereIf(factureCriterias.GlobalFilter != null & !isRef,
                //    f => f.Client.Nom.Trim().Contains(factureCriterias.GlobalFilter.Trim())
                //    || f.Client.RaisonSociale.Trim().Contains(factureCriterias.GlobalFilter.Trim()))
                .WhereIf(factureCriterias.GlobalFilter != null, f => f.Reference.StartsWith(factureCriterias.GlobalFilter))
                .WhereIf(client != 0, f => f.ClientId == client)
                .WhereIf(dateEmission != null, f => f.DateEmission >= dateEmission[0] && f.DateEmission <= dateEmission[1])
                .WhereIf(echeancePaiement != 0, f => f.EcheancePaiement == echeancePaiement)
                .WhereIf(montantTtc != -1, f => f.FactureItems.Sum(item => item.TotalTtc) == montantTtc)
                .WhereIf(statut != FactureStatutEnum.Undefined && statut != FactureStatutEnum.PaiementAttente
                    && statut != FactureStatutEnum.PaiementRetard, f => f.Statut == statut)
                .WhereIf(statut == FactureStatutEnum.PaiementAttente, e => e.Statut == FactureStatutEnum.Valide &&
                    DateTime.Compare(e.DateEmission.AddDays(e.EcheancePaiement), DateTime.Now) > 0)
                .WhereIf(statut == FactureStatutEnum.PaiementRetard, e => e.Statut == FactureStatutEnum.Valide &&
                    DateTime.Compare(e.DateEmission.AddDays(e.EcheancePaiement), DateTime.Now) < 0)
                .OrderByDescending(f => f.LastModificationTime);

            var result = 0.0f;
            foreach (var item in query)
            {
               /*  result += (float)(item.FactureItems.Sum(di => (float?)di.TotalTtc) -
                 item.FactureItems.Sum(di => (float?)di.UnitPriceHT * di.Quantity) * item.Remise / 100); */
                result += item.MontantTtc;
            }
            return result;
        }

        private static void CheckIfIsRefSearch(CriteriasDto factureCriterias, out bool isRef, out int minRef, out int maxRef)
        {
            isRef = false;
            minRef = 0;
            maxRef = 0;
            if (factureCriterias.GlobalFilter != null && factureCriterias.GlobalFilter.Trim().ToLower().StartsWith('f'))
            {
                string strRef = factureCriterias.GlobalFilter.Trim().Remove(0, 1);
                if (Int32.TryParse(strRef, out int n))
                {
                    isRef = true;
                    minRef = Int32.Parse(strRef + new String('0', 5 - strRef.Length));
                    maxRef = Int32.Parse(strRef + new String('9', 5 - strRef.Length));
                }
            }
        }

        private static void CheckIfIsFilterSearch(CriteriasDto factureCriterias, out long client, out DateTime[] dateEmission, out int echeancePaiement,
            out float montantTtc, out FactureStatutEnum statut) 
        {
            client = 0;
            dateEmission = null;
            echeancePaiement = 0;
            montantTtc = -1;
            statut = FactureStatutEnum.Undefined;

            if(factureCriterias.Filtres != null)
            {
                client = factureCriterias.Filtres.Client;
                dateEmission = factureCriterias.Filtres.DateEmission;
                echeancePaiement = factureCriterias.Filtres.EcheancePaiement;
                montantTtc = factureCriterias.Filtres.MontantTtc  ;
                statut = (FactureStatutEnum)factureCriterias.Filtres.Statut;
            }
        }
        public async Task<byte[]> GetByIdFactureReport(long id)
        {
            var facture = await _factureRepository.GetAllIncluding(f => f.Client, f => f.FactureItems)
                .Where(f => f.Id == id)
                .ToListAsync();
            return _reportGeneratorAppService.GetByteDataFacture(ObjectMapper.Map<FactureDto>(facture.First()));
        }

        public async Task<byte[]> GetByteDataFactureReport(CreateFactureInput input)
        {
            var facture = ObjectMapper.Map<Facture>(input);
            facture.Client = (await _clientRepository.GetAll()
                .Where(c => (c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId) && c.Id == input.ClientId).ToListAsync()).First();
            return _reportGeneratorAppService.GetByteDataFacture(ObjectMapper.Map<FactureDto>(facture));
        }


        public async Task<bool> CreateOrUpdateFactureInfosPaiement(FactureInfosPaiementDto factureInfosPaiement)
        {
            var fip = ObjectMapper.Map<FactureInfosPaiement>(factureInfosPaiement);
            try
            {
                await _factureInfosPaiementRepository.InsertOrUpdateAsync(fip);
            }
            catch (Exception)
            {
                return false;
            } 
            return true;
        } 

        public async Task<ListResultWithTotalEntityItemsDto<FactureInfosPaiementDto>> GetByFactureIdFactureInfosPaiement(FactureInfosPaiementCriteriaDto factureInfosPaiementCriteriaDto)
        {
            var query = _factureInfosPaiementRepository.GetAll()
                .Where(fip => fip.FactureId == factureInfosPaiementCriteriaDto.FactureId);

            var chunk = query.Skip(factureInfosPaiementCriteriaDto.First).Take(factureInfosPaiementCriteriaDto.Rows);

            var list = ObjectMapper.Map<List<FactureInfosPaiementDto>>(await chunk.ToListAsync());
            return new ListResultWithTotalEntityItemsDto<FactureInfosPaiementDto>(list, await query.LongCountAsync());
        }

        public async Task<float> GetRestOfAmount(long factureId)
        {
            var fip = await _factureInfosPaiementRepository.GetAll()
                .Where(fip => fip.FactureId == factureId)
                .SumAsync(x => x.MontantPaye);

            return fip;
        }

        public async Task<bool> deleteByFactureIdFactureInfosPaiement(long factureId)
        {
            try{
                await _factureInfosPaiementRepository
                .DeleteAsync(item => item.FactureId == factureId);
                return true;
            }
            catch(Exception ){
                return false;
            }
        }

        public async Task<bool> deleteFactureInfosPaiement(long paiementId)
        {
            try{
                await _factureInfosPaiementRepository
                .DeleteAsync(item => item.Id == paiementId);
                return true;
            }
            catch(Exception ){
                return false;
            }
        }

         public async Task<bool> CheckIfReferenceIsExist(string reference) {
            var query = await this._factureRepository.GetAll()
                .FirstOrDefaultAsync(item => 
                    (item.CreatorUserId == AbpSession.UserId || item.LastModifierUserId == AbpSession.UserId) &&
                    item.Reference == reference);
            if(query != null)
                return true;
            else 
                return false;
        }

    }
}
