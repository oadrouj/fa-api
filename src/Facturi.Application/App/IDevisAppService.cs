using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Facturi.App.Dtos;
using System.Threading.Tasks;

namespace Facturi.App
{
    public interface IDevisAppService : IApplicationService
    {
        Task<bool> CreateDevis(CreateDevisInput input);
        Task<bool> UpdateDevis(UpdateDevisInput input);
        Task DeleteDevis(long DevisId);
        Task<DevisDto> GetByIdDevis(long id);
        Task<int> GetLastReference();
        Task<bool> ChangeDevistatut(long DevisId, DevisStatutEnum statut);
        //Task<ListResultDto<DevisDto>> GetAllDevis(ListCriteriaDto listCriteria);
    }
}
