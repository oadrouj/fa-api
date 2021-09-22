using System.Threading.Tasks;
using Abp.Application.Services;
using Facturi.Sessions.Dto;

namespace Facturi.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
