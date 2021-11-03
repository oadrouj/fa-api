using Abp.Application.Services;
using System.Threading.Tasks;

namespace Facturi.App
{
    public interface IReportGeneratorAppService : IApplicationService
    {
        byte[] GetByteDataFacture(FactureDto facture);
        byte[] GetByteDataDevis(DevisDto facture);
    }
}
