using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Facturi.App.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Facturi.App
{
    public class FactureAppService : ApplicationService, IFactureAppService
    {
        private readonly IRepository<Facture, long> _factureRepository;
        private readonly IRepository<FactureInfosPaiement, long> _factureInfosPaiementRepository;
        private readonly IRepository<FactureItem, long> _factureItemRepository;
        private readonly IReportGeneratorAppService _reportGeneratorAppService;
        private readonly IRepository<Client, long> _clientRepository;

        public FactureAppService(IRepository<Facture, long> FactureRepository, IRepository<FactureItem, long> factureItemRepository,
            IReportGeneratorAppService reportGeneratorAppService, IRepository<FactureInfosPaiement, long> factureInfosPaiementRepository,
            IRepository<Client, long> clientRepository
            )
        {
            _factureRepository = FactureRepository ?? throw new ArgumentNullException(nameof(FactureRepository));
            _factureItemRepository = factureItemRepository ?? throw new ArgumentNullException(nameof(factureItemRepository));
            _reportGeneratorAppService = reportGeneratorAppService ?? throw new ArgumentNullException(nameof(reportGeneratorAppService));
            _factureInfosPaiementRepository = factureInfosPaiementRepository ?? throw new ArgumentNullException(nameof(factureInfosPaiementRepository));
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));

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
            var facture = await _factureRepository
                .GetAll()
                .Where(f => (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId))
                .OrderByDescending(f => f.Reference).ToListAsync();
            if (facture != null && facture.Any())
            {
                return facture.First().Reference;
            }
            else
            {
                return 0;
            }
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

            var FactureList = new List<Facture>();
            var query = _factureRepository.GetAllIncluding(f => f.FactureItems, f => f.Client)
                .Where(f => (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId))
                .WhereIf(factureCriterias.GlobalFilter != null & !isRef,
                    f => f.Client.Nom.Trim().Contains(factureCriterias.GlobalFilter.Trim())
                    || f.Client.RaisonSociale.Trim().Contains(factureCriterias.GlobalFilter.Trim()))
                .WhereIf(isRef, f => minRef <= f.Reference && f.Reference <= maxRef);


            //.WhereIf(!factureCriterias.FactureCategory.Equals("0"), f => f.CategorieFacture.Equals(factureCriterias.FactureCategory));

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
                    default:
                        FactureList = await query.OrderByDescending(f => f.LastModificationTime != null ? f.LastModificationTime : f.CreationTime).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync();
                        break;
                }

            }
            else
            {
                FactureList = await query.OrderByDescending(f => f.LastModificationTime != null ? f.LastModificationTime : f.CreationTime).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync();
            }
            var result = new ListResultDto<FactureDto>(ObjectMapper.Map<List<FactureDto>>(FactureList));
            return result;
        }

        public async Task<int> GetAllFactureTotalRecords(CriteriasDto factureCriterias)
        {
            CheckIfIsRefSearch(factureCriterias, out bool isRef, out int minRef, out int maxRef);
            var query = _factureRepository.GetAllIncluding(f => f.FactureItems, f => f.Client)
                .Where(f => (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId))
                .WhereIf(factureCriterias.GlobalFilter != null & !isRef,
                    f => f.Client.Nom.Trim().Contains(factureCriterias.GlobalFilter.Trim())
                    || f.Client.RaisonSociale.Trim().Contains(factureCriterias.GlobalFilter.Trim()))
                .WhereIf(isRef, f => minRef <= f.Reference && f.Reference <= maxRef);


            //.WhereIf(!factureCriterias.FactureCategory.Equals("0"), f => f.CategorieFacture.Equals(factureCriterias.FactureCategory));

            return await query.CountAsync();
        }

        public async Task<float> GetAllFactureMontantTotal(CriteriasDto factureCriterias)
        {
            CheckIfIsRefSearch(factureCriterias, out bool isRef, out int minRef, out int maxRef);
            var query = _factureRepository.GetAllIncluding(f => f.FactureItems, f => f.Client)
                .Where(f => (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId))
                .WhereIf(factureCriterias.GlobalFilter != null & !isRef,
                    f => f.Client.Nom.Trim().Contains(factureCriterias.GlobalFilter.Trim())
                    || f.Client.RaisonSociale.Trim().Contains(factureCriterias.GlobalFilter.Trim()))
                .WhereIf(isRef, f => minRef <= f.Reference && f.Reference <= maxRef);


            //.WhereIf(!factureCriterias.FactureCategory.Equals("0"), f => f.CategorieFacture.Equals(factureCriterias.FactureCategory));

            var result = await query.SelectMany(f => f.FactureItems).SumAsync(di => (float?)di.TotalTtc) ?? 0;
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

        public async Task<byte[]> GetByIdFactureReport(long id)
        {
            var facture = await _factureRepository.GetAllIncluding(f => f.Client, f => f.FactureItems)
                .Where(f => f.Id == id)
                .ToListAsync();
            return _reportGeneratorAppService.GetByteDataFacture(ObjectMapper.Map<Facture>(facture.First()));
        }

        public async Task<byte[]> GetByteDataFactureReport(CreateFactureInput input)
        {
            var facture = ObjectMapper.Map<Facture>(input);
            facture.Client = (await _clientRepository.GetAll()
                .Where(c => (c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId) && c.Id == input.ClientId).ToListAsync()).First();
            return _reportGeneratorAppService.GetByteDataFacture(facture);
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

        public async Task<FactureInfosPaiementDto> GetByFactureIdFactureInfosPaiement(long factureId)
        {
            var fip = await _factureInfosPaiementRepository.GetAll()
                .Where(fip => fip.FactureId == factureId)
                .FirstOrDefaultAsync();
            return ObjectMapper.Map<FactureInfosPaiementDto>(fip == null ? new FactureInfosPaiementDto() : fip);
        }
    }
}
