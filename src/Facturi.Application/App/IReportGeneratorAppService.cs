using Abp.Application.Services;
using System.Threading.Tasks;

namespace Facturi.App
{
    public interface IReportGeneratorAppService : IApplicationService
    {
        Task<byte[]> GetByteDataFacture();
    }
}
