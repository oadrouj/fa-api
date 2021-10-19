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

namespace Facturi.App
{
    public class FactureAppService : ApplicationService, IFactureAppService
    {
        private readonly IRepository<Facture, long> _factureRepository;
        private readonly IRepository<FactureItem, long> _factureItemRepository;

        public FactureAppService(IRepository<Facture, long> FactureRepository, IRepository<FactureItem, long> factureItemRepository)
        {
            _factureRepository = FactureRepository ?? throw new ArgumentNullException(nameof(FactureRepository));
            _factureItemRepository = factureItemRepository ?? throw new ArgumentNullException(nameof(factureItemRepository));
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
            var facture = await _factureRepository.GetAllIncluding(d => d.FactureItems, d => d.Client)
                .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId) && d.Id == id)
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
                .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId))
                .OrderByDescending(d => d.Reference).ToListAsync();
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
                var facture = (await _factureRepository.GetAllIncluding(d => d.FactureItems, d => d.Client)
                                .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId) && d.Id == FactureId)
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
            var query = _factureRepository.GetAllIncluding(d => d.FactureItems, d => d.Client)
                .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId))
                .WhereIf(factureCriterias.GlobalFilter != null & !isRef,
                    d => d.Client.Nom.Trim().Contains(factureCriterias.GlobalFilter.Trim())
                    || d.Client.RaisonSociale.Trim().Contains(factureCriterias.GlobalFilter.Trim()))
                .WhereIf(isRef, d => minRef <= d.Reference && d.Reference <= maxRef);


            //.WhereIf(!factureCriterias.FactureCategory.Equals("0"), d => d.CategorieFacture.Equals(factureCriterias.FactureCategory));

            if (factureCriterias.SortField != null && factureCriterias.SortField.Length != 0)
            {
                switch (factureCriterias.SortField)
                {
                    case "reference":
                        if (factureCriterias.SortOrder.Equals("1")) { FactureList = await query.OrderBy(d => d.Reference).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        else if (factureCriterias.SortOrder.Equals("-1")) { FactureList = await query.OrderByDescending(d => d.Reference).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        break;
                    case "client":
                        if (factureCriterias.SortOrder.Equals("1")) { FactureList = await query.OrderBy(d => d.Client.RaisonSociale + d.Client.Nom).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        else if (factureCriterias.SortOrder.Equals("-1")) { FactureList = await query.OrderByDescending(d => d.Client.Nom + d.Client.RaisonSociale).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        break;
                    case "dateEmission":
                        if (factureCriterias.SortOrder.Equals("1")) { FactureList = await query.OrderBy(d => d.DateEmission).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        else if (factureCriterias.SortOrder.Equals("-1")) { FactureList = await query.OrderByDescending(d => d.DateEmission).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync(); }
                        break;
                    default:
                        FactureList = await query.OrderByDescending(d => d.LastModificationTime != null ? d.LastModificationTime : d.CreationTime).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync();
                        break;
                }

            }
            else
            {
                FactureList = await query.OrderByDescending(d => d.LastModificationTime != null ? d.LastModificationTime : d.CreationTime).Skip(factureCriterias.First).Take(factureCriterias.Rows).ToListAsync();
            }
            var result = new ListResultDto<FactureDto>(ObjectMapper.Map<List<FactureDto>>(FactureList));
            return result;
        }

        public async Task<int> GetAllFactureTotalRecords(CriteriasDto factureCriterias)
        {
            CheckIfIsRefSearch(factureCriterias, out bool isRef, out int minRef, out int maxRef);
            var query = _factureRepository.GetAllIncluding(d => d.FactureItems, d => d.Client)
                .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId))
                .WhereIf(factureCriterias.GlobalFilter != null & !isRef,
                    d => d.Client.Nom.Trim().Contains(factureCriterias.GlobalFilter.Trim())
                    || d.Client.RaisonSociale.Trim().Contains(factureCriterias.GlobalFilter.Trim()))
                .WhereIf(isRef, d => minRef <= d.Reference && d.Reference <= maxRef);


            //.WhereIf(!factureCriterias.FactureCategory.Equals("0"), d => d.CategorieFacture.Equals(factureCriterias.FactureCategory));

            return await query.CountAsync();
        }

        public async Task<float> GetAllFactureMontantTotal(CriteriasDto factureCriterias)
        {
            CheckIfIsRefSearch(factureCriterias, out bool isRef, out int minRef, out int maxRef);
            var query = _factureRepository.GetAllIncluding(d => d.FactureItems, d => d.Client)
                .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId))
                .WhereIf(factureCriterias.GlobalFilter != null & !isRef,
                    d => d.Client.Nom.Trim().Contains(factureCriterias.GlobalFilter.Trim())
                    || d.Client.RaisonSociale.Trim().Contains(factureCriterias.GlobalFilter.Trim()))
                .WhereIf(isRef, d => minRef <= d.Reference && d.Reference <= maxRef);


            //.WhereIf(!factureCriterias.FactureCategory.Equals("0"), d => d.CategorieFacture.Equals(factureCriterias.FactureCategory));

            var result = await query.SelectMany(d => d.FactureItems).SumAsync(di => (float?)di.TotalTtc) ?? 0;
            return result;
        }

        private static void CheckIfIsRefSearch(CriteriasDto factureCriterias, out bool isRef, out int minRef, out int maxRef)
        {
            isRef = false;
            minRef = 0;
            maxRef = 0;
            if (factureCriterias.GlobalFilter != null && factureCriterias.GlobalFilter.Trim().ToLower().StartsWith('d'))
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

        public async Task<bool> GenererPlanning(long id)
        {
            try
            {
                //byte[] file = _generateurRapportAppService.GetByteDataPlanning();
                if (File.Exists(@"c:\PlanningNephroLog.pdf"))
                {
                    File.Delete(@"c:\PlanningNephroLog.pdf");
                }
                FileStream fs2 = new FileStream(@"c:\PlanningNephroLog.pdf", FileMode.OpenOrCreate);
                fs2.Write(file, 0, file.Length);
                fs2.Close();
                fs2.Dispose();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

    }
}
