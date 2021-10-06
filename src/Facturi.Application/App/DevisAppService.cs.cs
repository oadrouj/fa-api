using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Facturi.App.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App
{
    public class DevisAppService : ApplicationService, IDevisAppService
    {
        private readonly IRepository<Devis, long> _devisRepository;
        private readonly IRepository<DevisItem, long> _devisItemRepository;

        public DevisAppService(IRepository<Devis, long> DevisRepository, IRepository<DevisItem, long> devisItemRepository)
        {
            _devisRepository = DevisRepository ?? throw new ArgumentNullException(nameof(DevisRepository));
            _devisItemRepository = devisItemRepository ?? throw new ArgumentNullException(nameof(devisItemRepository));
        }

        public async Task<bool> CreateDevis(CreateDevisInput input)
        {
            try
            {
                //Gerer reference
                var devis = ObjectMapper.Map<Devis>(input);
                var newDevisId = _devisRepository.InsertAndGetId(devis);

                var devisItems = ObjectMapper.Map<List<DevisItem>>(devis.DevisItems);
                for (int i = 0; i < devisItems.Count; i++)
                {
                    devisItems.ElementAt(i).DevisId = newDevisId;
                    await _devisItemRepository.InsertAsync(devisItems.ElementAt(i));
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateDevis(UpdateDevisInput input)
        {
            try
            {
                var devisItemsToDelete = _devisItemRepository.GetAll().Where(di => di.DevisId == input.Id).Select(di => di.Id).ToArray();
                if (devisItemsToDelete != null && devisItemsToDelete.Length > 0)
                {
                    foreach (long devisItemId in devisItemsToDelete)
                    {
                        _devisItemRepository.Delete(devisItemId);
                    }
                }

                var devis = ObjectMapper.Map<Devis>(input);
                await _devisRepository.UpdateAsync(devis);

                var devisItemsToInsert = ObjectMapper.Map<List<DevisItem>>(devis.DevisItems);
                for (int i = 0; i < devisItemsToInsert.Count; i++)
                {
                    devisItemsToInsert.ElementAt(i).DevisId = devis.Id;
                    await _devisItemRepository.InsertAsync(devisItemsToInsert.ElementAt(i));
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<DevisDto> GetByIdDevis(long id)
        {
            var devis = await _devisRepository.GetAllIncluding(d => d.DevisItems, d => d.Client)
                .Where(c => (c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId) && c.Id == id)
                .ToListAsync();
            var result = ObjectMapper.Map<DevisDto>(devis.First());
            return result;
        }

        //public async Task<ListResultDto<DevisDto>> GetAllDevis(ListCriteriaDto listCriteria)
        //{
        //    bool isRef = false;
        //    int minRef = 0;
        //    int maxRef = 0;

        //    if (listCriteria.ChampsRecherche != null && listCriteria.ChampsRecherche.Trim().ToLower().StartsWith('c'))
        //    {
        //        string strRef = listCriteria.ChampsRecherche.Trim().Remove(0, 1);
        //        if (Int32.TryParse(strRef, out int n))
        //        {
        //            isRef = true;
        //            minRef = Int32.Parse(strRef + new String('0', 5 - strRef.Length));
        //            maxRef = Int32.Parse(strRef + new String('9', 5 - strRef.Length));
        //        }
        //    }
        //    var Deviss = new List<Devis>();
        //    var query = _devisRepository.GetAll()
        //        .Where(c => (c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId))
        //        .WhereIf(listCriteria.ChampsRecherche != null & !isRef,
        //            c => c.Nom.Trim().Contains(listCriteria.ChampsRecherche.Trim())
        //            || c.RaisonSociale.Trim().Contains(listCriteria.ChampsRecherche.Trim()))
        //        .WhereIf(isRef, c => minRef <= c.Reference && c.Reference <= maxRef)
        //        .WhereIf(!listCriteria.DevisCategory.Equals("0"), c => c.CategorieDevis.Equals(listCriteria.DevisCategory));

        //    if (listCriteria.SortField != null && listCriteria.SortField.Length != 0)
        //    {
        //        switch (listCriteria.SortField)
        //        {
        //            case "reference":
        //                if (listCriteria.SortOrder == 1) { Deviss = await query.OrderBy(c => c.Reference).ToListAsync(); }
        //                else { Deviss = await query.OrderByDescending(c => c.Reference).ToListAsync(); }
        //                break;
        //            case "creationTime":
        //                if (listCriteria.SortOrder == 1) { Deviss = await query.OrderBy(c => c.CreationTime).ToListAsync(); }
        //                else { Deviss = await query.OrderByDescending(c => c.CreationTime).ToListAsync(); }
        //                break;
        //            case "nom":
        //                if (listCriteria.SortOrder == 1) { Deviss = await query.OrderBy(c => c.RaisonSociale + c.Nom).ToListAsync(); }
        //                else { Deviss = await query.OrderByDescending(c => c.Nom + c.RaisonSociale).ToListAsync(); }
        //                break;
        //            default:
        //                Deviss = await query.OrderByDescending(c => c.LastModificationTime != null ? c.LastModificationTime : c.CreationTime).ToListAsync();
        //                break;
        //        }

        //    }
        //    else
        //    {
        //        Deviss = await query.OrderByDescending(c => c.LastModificationTime != null ? c.LastModificationTime : c.CreationTime).ToListAsync();
        //    }
        //    var result = new ListResultDto<DevisDto>(ObjectMapper.Map<List<DevisDto>>(Deviss));
        //    return result;
        //}

        public async Task DeleteDevis(long DevisId)
        {
            var devisItemsToDelete = _devisItemRepository.GetAll().Where(di => di.DevisId == DevisId).Select(di => di.Id).ToArray();
            if (devisItemsToDelete != null && devisItemsToDelete.Length > 0)
            {
                foreach (long devisItemId in devisItemsToDelete)
                {
                    _devisItemRepository.Delete(devisItemId);
                }
            }
            await _devisRepository.DeleteAsync(DevisId);
        }

        public async Task<int> GetLastReference()
        {
            var devis = await _devisRepository
                .GetAll()
                .Where(c => (c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId))
                .OrderByDescending(c => c.Reference).ToListAsync();
            if (devis != null && devis.Any())
            {
                return devis.First().Reference;
            }
            else
            {
                return 0;
            }
        }
    }
}
