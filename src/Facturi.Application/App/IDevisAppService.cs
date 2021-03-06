using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Facturi.App.Dtos;
using Facturi.App.Dtos.EstimationDtos;
using System.Threading.Tasks;

namespace Facturi.App
{
    public interface IDevisAppService : IApplicationService
    {
        Task<long> CreateDevis(CreateDevisInput input);
        Task<bool> UpdateDevis(UpdateDevisInput input);
        Task<bool> DeleteDevis(long DevisId);
        Task<DevisDto> GetByIdDevis(long id);
        Task<int> GetLastReference();
        Task<EstimationInitiationDto> GetLastReferenceWithIntroMessageAndFooter();
        Task<bool> ChangeDevisStatut(long DevisId, DevisStatutEnum statut);
        Task<ListResultDto<DevisDto>> GetAllDevis(CriteriasDto listCriteria);
        Task<byte[]> GetByIdDevisReport(long id);
        Task<int> GetAllDevisTotalRecords(CriteriasDto devisCriterias);
        Task<float> GetAllDevisMontantTotal(CriteriasDto devisCriterias);
        Task<byte[]> GetByteDataDevisReport(CreateDevisInput input);
        Task<bool> CheckIfReferenceIsExist(char referencePrefix, string reference);

    }
}
