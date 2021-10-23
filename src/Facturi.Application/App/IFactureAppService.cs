using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Facturi.App.Dtos;
using System.Threading.Tasks;

namespace Facturi.App
{
    public interface IFactureAppService : IApplicationService
    {
        Task<long> CreateFacture(CreateFactureInput input);
        Task<bool> UpdateFacture(UpdateFactureInput input);
        Task<bool> DeleteFacture(long factureId);
        Task<FactureDto> GetByIdFacture(long id);
        Task<int> GetLastReference();
        Task<bool> ChangeFactureStatut(long FactureId, FactureStatutEnum statut);
        Task<ListResultDto<FactureDto>> GetAllFacture(CriteriasDto factureCriterias);
        Task<int> GetAllFactureTotalRecords(CriteriasDto factureCriterias);
        Task<float> GetAllFactureMontantTotal(CriteriasDto factureCriterias);
        Task<bool> CreateOrUpdateFactureInfosPaiement(FactureInfosPaiementDto factureInfosPaiement);
        Task<FactureInfosPaiementDto> GetByFactureIdFactureInfosPaiement(long factureId);
        Task<byte[]> GetByIdFactureReport(long id);
    }
}
