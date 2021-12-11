using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Facturi.App.Dtos;
using Facturi.Application.App.Dtos;
using System.Threading.Tasks;

namespace Facturi.Application.App
{
    public interface ICountryServiceApp: IApplicationService
    {
         Task<ListResultDto<CountryDto>> GetAllCountries();
    }
}