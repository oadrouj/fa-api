using Abp.Application.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Facturi.App.Dtos;

namespace Facturi.App
{
    public interface IReportGeneratorAppService : IApplicationService
    {
        byte[] GetByteDataFacture(FactureDto facture);
        byte[] GetByteDataDevis(DevisDto facture);
        Task<FileDto> GetUsersAsPdfAsync();
        Task<FileDto> GetFactureAsPdfAsync(FactureDto factureDto,InfosEntrepriseDto infosEnteprise); 
        Task<FileDto> GetDevisAsPdfAsync(DevisDto devisDto,InfosEntrepriseDto infosEnteprise);
        /* 
        Task<ActionResult> DownloadAsPdfAsync(); */
    }
}
