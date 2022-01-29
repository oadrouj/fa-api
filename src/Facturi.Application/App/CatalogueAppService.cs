using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using  Facturi.Application.App;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Facturi.Application.App.Dtos.CatalogueDtos;
using Facturi.Core.App;
using Facturi.App;
using Facturi.App.Dtos.GenericDtos;

namespace Facturi.Application.App
{
    public class CatalogueAppService: ApplicationService, ICatalogueAppService
    {
        private readonly IRepository<Catalogue, long> _catalogueRepository;
        private readonly IRepository<Facture, long> _factureRepository;
        public CatalogueAppService(
            IRepository<Catalogue, long> catalogueRepository,
            IRepository<Facture, long> factureRepository
        )
        {
            this._catalogueRepository = catalogueRepository ?? throw new ArgumentNullException(nameof(catalogueRepository));
            this._factureRepository = factureRepository ?? throw new ArgumentNullException(nameof(factureRepository));
        
        }
        public async Task<CatalogueDto> CreateCatalogue(CreateCatalogueInput input)
        {
            try {
            int Reference = 1;
            var maxRefClient = _catalogueRepository.GetAll().Where(c => c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId).OrderByDescending(c => c.Reference).ToList();
            if (maxRefClient != null && maxRefClient.Any())
            {
                Reference = maxRefClient.First().Reference + 1;
            }

            var catalogue = ObjectMapper.Map<Catalogue>(input);
            catalogue.Reference = Reference;
            catalogue.AddedDate = DateTime.Now;
            long id = await _catalogueRepository.InsertAndGetIdAsync(catalogue);
                return ObjectMapper.Map<CatalogueDto>(catalogue);
            }
              catch (Exception)
            {
                return null;
            }

        }
        public async Task<bool> UpdateCatalogue(UpdateCatalogueInput input)
        {
            try 
            {
                var reference =  (_catalogueRepository.GetAll()
                    .AsNoTracking().FirstOrDefault(v => v.Id == input.Id)).Reference;
                var catalogue = ObjectMapper.Map<Catalogue>(input);
                catalogue.Reference = reference;
               
                await _catalogueRepository.UpdateAsync(catalogue);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public async Task<bool> DeleteCatalogue(long catalogueId)
        {
            try
            {
                await _catalogueRepository.DeleteAsync(catalogueId);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public async Task<CatalogueDto> GetByIdCatalogue(long id)
        {
            var catalogue = await _catalogueRepository
                .FirstOrDefaultAsync(c => (c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId) && c.Id == id);
            
            return ObjectMapper.Map<CatalogueDto>(catalogue);
        }

        public async Task<ListResultDto<CatalogueForAutoCompleteDto>> GetCatalogueForAutoComplete(string keyword)
        {
            var result = await _catalogueRepository.GetAll()
                .Where(c => c.CreatorUserId == AbpSession.UserId || c.LastModifierUserId == AbpSession.UserId)
                .WhereIf(keyword != null, c => c.Designation.Contains(keyword))
                .Select(c => ObjectMapper.Map<CatalogueForAutoCompleteDto>(c))
                .ToListAsync();

            return new ListResultDto<CatalogueForAutoCompleteDto>(result);
        }
        public async Task<ListResultWithTotalEntityItemsDto<CatalogueDto>> GetAllCatalogues(CatalogueCriteriaDto catalogueCriterias)
        {
            CheckIfIsRefSearch(catalogueCriterias, out bool isRef, out int minRef, out int maxRef);
            
            CheckIfIsFilterSearch(catalogueCriterias, out string CatalogueType);

            var CatalogueList = new List<Catalogue>();
            var query = _catalogueRepository.GetAll()
            .Where(f => (f.CreatorUserId == AbpSession.UserId || f.LastModifierUserId == AbpSession.UserId))
            .WhereIf(catalogueCriterias.GlobalFilter != null & !isRef,
                f => f.Designation.Trim().Contains(catalogueCriterias.GlobalFilter.Trim()))
            .WhereIf(isRef, f => minRef <= f.Reference && f.Reference <= maxRef)
            .WhereIf(CatalogueType != null, f => f.CatalogueType == CatalogueType);

            if (catalogueCriterias.SortField != null && catalogueCriterias.SortField.Length != 0)
            {
                switch (catalogueCriterias.SortField)
                {
                    case "reference":
                        if (catalogueCriterias.SortOrder.Equals("1")) { CatalogueList = await query.OrderBy(d => d.Reference).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync(); }
                        else if (catalogueCriterias.SortOrder.Equals("-1")) { CatalogueList = await query.OrderByDescending(d => d.Reference).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync(); }
                        break;
                    case "addedDate":
                        if (catalogueCriterias.SortOrder.Equals("1")) { CatalogueList = await query.OrderBy(d => d.AddedDate).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync(); }
                        else if (catalogueCriterias.SortOrder.Equals("-1")) { CatalogueList = await query.OrderByDescending(d => d.AddedDate).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync(); }
                        break;
                    case "designation":
                        if (catalogueCriterias.SortOrder.Equals("1")) { CatalogueList = await query.OrderBy(d => d.Designation).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync(); }
                        else if (catalogueCriterias.SortOrder.Equals("-1")) { CatalogueList = await query.OrderByDescending(d => d.Designation).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync(); }
                        break;
                     case "htPrice":
                        if (catalogueCriterias.SortOrder.Equals("1")) { CatalogueList = await query.OrderBy(d => d.HtPrice).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync(); }
                        else if (catalogueCriterias.SortOrder.Equals("-1")) { CatalogueList = await query.OrderByDescending(d => d.HtPrice).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync(); }
                        break;
                     case "unity":
                        if (catalogueCriterias.SortOrder.Equals("1")) { CatalogueList = await query.OrderBy(d => d.Unity).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync(); }
                        else if (catalogueCriterias.SortOrder.Equals("-1")) { CatalogueList = await query.OrderByDescending(d => d.Unity).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync(); }
                        break;
                     case "tva":
                        if (catalogueCriterias.SortOrder.Equals("1")) { CatalogueList = await query.OrderBy(d => d.Tva).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync(); }
                        else if (catalogueCriterias.SortOrder.Equals("-1")) { CatalogueList = await query.OrderByDescending(d => d.Tva).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync(); }
                        break;
                    //  case "totalSalesTTC":
                    //     if (catalogueCriterias.SortOrder.Equals("1")) { CatalogueList = await query.OrderBy(d => d.To).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync(); }
                    //     else if (catalogueCriterias.SortOrder.Equals("-1")) { CatalogueList = await query.OrderByDescending(d => d.DateEmission).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync(); }
                    //     break;
                    default:
                        CatalogueList = await query.OrderByDescending(d => d.AddedDate).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync();
                        break;
                }

            }
            else
            {
                CatalogueList = await query.OrderByDescending(d => d.AddedDate).Skip(catalogueCriterias.First).Take(catalogueCriterias.Rows).ToListAsync();
            }

            var list = ObjectMapper.Map<List<CatalogueDto>>(CatalogueList);

            foreach (var item in list)
            {
                var factureItems = _factureRepository.GetAllIncluding(v => v.FactureItems)
                    .Where(x => x.Statut == FactureStatutEnum.Valide)
                    .SelectMany(v => v.FactureItems).Where(v => v.Id == item.Id);

                item.TotalSalesTTC = factureItems.Sum(v => v.TotalTtc);
                item.TotalUnitsSold = factureItems.Sum(v => v.Quantity);
            }

            var totalItems = await query.LongCountAsync();
            return new ListResultWithTotalEntityItemsDto<CatalogueDto>(list, totalItems);
        }

        private static void CheckIfIsRefSearch(CatalogueCriteriaDto catalogueCriterias, out bool isRef, out int minRef, out int maxRef)
        {
            isRef = false;
            minRef = 0;
            maxRef = 0;
            if (catalogueCriterias.GlobalFilter != null && catalogueCriterias.GlobalFilter.Trim().ToLower().StartsWith('p'))
            {
                string strRef = catalogueCriterias.GlobalFilter.Trim().Remove(0, 1);
                if (Int32.TryParse(strRef, out int n))
                {
                    isRef = true;
                    minRef = Int32.Parse(strRef + new String('0', 5 - strRef.Length));
                    maxRef = Int32.Parse(strRef + new String('9', 5 - strRef.Length));
                }
            }
        }

        private static void CheckIfIsFilterSearch(CatalogueCriteriaDto catalogueCriterias, out string catalogueType) 
        {
            catalogueType = null;

            if(catalogueCriterias.Filtres != null)
            {
                catalogueType = catalogueCriterias.Filtres.CatalogueType;
                
            }
        }
    }
}