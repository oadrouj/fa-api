using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Facturi.App.Dtos;
using Facturi.App.Dtos.GenericDtos;
using Facturi.App.Dtos.InvoiceDtos;
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
        Task<InvoiceInitiationDto> GetLastReferenceWithIntroMessageAndFooter();
        Task<bool> ChangeFactureStatut(long FactureId, FactureStatutEnum statut);
        Task<ListResultDto<FactureDto>> GetAllFacture(CriteriasDto factureCriterias);
        Task<int> GetAllFactureTotalRecords(CriteriasDto factureCriterias);
        Task<float> GetAllFactureMontantTotal(CriteriasDto factureCriterias);
        Task<bool> CreateOrUpdateFactureInfosPaiement(FactureInfosPaiementDto factureInfosPaiement);
        Task<ListResultWithTotalEntityItemsDto<FactureInfosPaiementDto>> GetByFactureIdFactureInfosPaiement(FactureInfosPaiementCriteriaDto factureInfosPaiementCriteriaDto);
        Task<float> GetRestOfAmount(long factureId);
        Task<byte[]> GetByIdFactureReport(long id);
        Task<byte[]> GetByteDataFactureReport(CreateFactureInput input);
        Task<bool> deleteByFactureIdFactureInfosPaiement(long factureId);
        Task<bool> CheckIfReferenceIsExist(string reference);

    }
}
