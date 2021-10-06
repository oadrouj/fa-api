using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Facturi.App.Dtos;
using System.Threading.Tasks;

namespace Facturi.App
{
    public interface IClientAppService : IApplicationService
    {
        Task<ClientDto> CreateClient(ClientDto input);
        Task<ClientDto> UpdateClient(ClientDto input);
        Task DeleteClient(long clientId);
        Task<ClientDto> GetByIdClient(long id);
        Task<ListResultDto<ClientDto>> GetByCategClient(string categ);
        Task<ListResultDto<ClientDto>> GetAllClients(ListCriteriaDto listCriteria);
        Task<ListResultDto<ClientForAutoCompleteDto>> GetClientForAutoComplete(string motCle);
    }
}
