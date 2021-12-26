using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Facturi.App.Dtos;
using System.Threading.Tasks;
using Facturi.Application.App.Dtos.CatalogueDtos;
using System;
using Facturi.App.Dtos.GenericDtos;

namespace Facturi.Application.App
{
    public interface ICatalogueAppService: IApplicationService
    {
        Task<CatalogueDto> CreateCatalogue(CreateCatalogueInput input);
        Task<bool> UpdateCatalogue(UpdateCatalogueInput input);
        Task<bool> DeleteCatalogue(long clientId);
        Task<CatalogueDto> GetByIdCatalogue(long id);
        Task<ListResultDto<CatalogueForAutoCompleteDto>> GetCatalogueForAutoComplete(string keyword);
        Task<ListResultWithTotalEntityItemsDto<CatalogueDto>> GetAllCatalogues(CatalogueCriteriaDto catalogueCriterias);

    }
}