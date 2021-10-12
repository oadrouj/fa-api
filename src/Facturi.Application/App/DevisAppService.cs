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
                var newDevisId = await _devisRepository.InsertAndGetIdAsync(devis);

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
                await _devisRepository.InsertOrUpdateAsync(devis);

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
                .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId) && d.Id == id)
                .ToListAsync();
            var result = ObjectMapper.Map<DevisDto>(devis.First());
            return result;
        }

        public async Task DeleteDevis(long DevisId)
        {
            var devisItemsToDelete = _devisItemRepository.GetAll().Where(di => di.DevisId == DevisId).Select(di => di.Id).ToArray();
            if (devisItemsToDelete != null && devisItemsToDelete.Length > 0)
            {
                foreach (long devisItemId in devisItemsToDelete)
                {
                    await _devisItemRepository.DeleteAsync(devisItemId);
                }
            }
            await _devisRepository.DeleteAsync(DevisId);
        }

        public async Task<int> GetLastReference()
        {
            var devis = await _devisRepository
                .GetAll()
                .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId))
                .OrderByDescending(d => d.Reference).ToListAsync();
            if (devis != null && devis.Any())
            {
                return devis.First().Reference;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> ChangeDevistatut(long DevisId, DevisStatutEnum statut)
        {
            try
            {
                var devis = (await _devisRepository.GetAllIncluding(d => d.DevisItems, d => d.Client)
                                .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId) && d.Id == DevisId)
                                .ToListAsync()).First();
                devis.Statut = statut;
                await _devisRepository.UpdateAsync(devis);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ListResultDto<DevisDto>> GetAllDevis(DevisCriteriasDto listCriteria)
        {
            //bool isRef = false;
            //int minRef = 0;
            //int maxRef = 0;

            //if (listCriteria.ChampsRecherche != null && listCriteria.ChampsRecherche.Trim().ToLower().StartsWith('d'))
            //{
            //    string strRef = listCriteria.ChampsRecherche.Trim().Remove(0, 1);
            //    if (Int32.TryParse(strRef, out int n))
            //    {
            //        isRef = true;
            //        minRef = Int32.Parse(strRef + new String('0', 5 - strRef.Length));
            //        maxRef = Int32.Parse(strRef + new String('9', 5 - strRef.Length));
            //    }
            //}
            var DevisList = new List<Devis>();
            var query = _devisRepository.GetAllIncluding(d => d.DevisItems, d => d.Client)
                .Where(d => (d.CreatorUserId == AbpSession.UserId || d.LastModifierUserId == AbpSession.UserId));
                //.WhereIf(listCriteria.ChampsRecherche != null & !isRef,
                //    d => d.Client.Nom.Trim().Contains(listCriteria.ChampsRecherche.Trim())
                //    || d.Client.RaisonSociale.Trim().Contains(listCriteria.ChampsRecherche.Trim()))
                //.WhereIf(isRef, d => minRef <= d.Reference && d.Reference <= maxRef);
                //.WhereIf(!listCriteria.DevisCategory.Equals("0"), d => d.CategorieDevis.Equals(listCriteria.DevisCategory));

            if (listCriteria.SortField != null && listCriteria.SortField.Length != 0)
            {
                switch (listCriteria.SortField)
                {
                    case "reference":
                        if (listCriteria.SortOrder.Equals("1")) { DevisList = await query.OrderBy(d => d.Reference).ToListAsync(); }
                        else if (listCriteria.SortOrder.Equals("-1")) { DevisList = await query.OrderByDescending(d => d.Reference).ToListAsync(); }
                        break;
                    case "client":
                        if (listCriteria.SortOrder.Equals("1")) { DevisList = await query.OrderBy(d => d.Client.RaisonSociale + d.Client.Nom).ToListAsync(); }
                        else if (listCriteria.SortOrder.Equals("-1")) { DevisList = await query.OrderByDescending(d => d.Client.Nom + d.Client.RaisonSociale).ToListAsync(); }
                        break;
                    case "dateEmission":
                        if (listCriteria.SortOrder.Equals("1")) { DevisList = await query.OrderBy(d => d.DateEmission).ToListAsync(); }
                        else if (listCriteria.SortOrder.Equals("-1")) { DevisList = await query.OrderByDescending(d => d.DateEmission).ToListAsync(); }
                        break;
                    default:
                        DevisList = await query.OrderByDescending(d => d.LastModificationTime != null ? d.LastModificationTime : d.CreationTime).ToListAsync();
                        break;
                }

            }
            else
            {
                DevisList = await query.OrderByDescending(d => d.LastModificationTime != null ? d.LastModificationTime : d.CreationTime).ToListAsync();
            }
            var result = new ListResultDto<DevisDto>(ObjectMapper.Map<List<DevisDto>>(DevisList));
            return result;
        }
    }
}
