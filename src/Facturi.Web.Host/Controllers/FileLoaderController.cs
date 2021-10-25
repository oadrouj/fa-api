using Facturi.App;
using Facturi.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Facturi.Web.Host.Controllers
{
    public class FileLoaderController : FacturiControllerBase
    {
        private readonly IFactureAppService _factureAppService;
        private readonly IDevisAppService _devisAppService;

        public FileLoaderController(IFactureAppService factureAppService, IDevisAppService devisAppService)
        {
            _factureAppService = factureAppService;
            _devisAppService = devisAppService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFacture(long id)
        {
            var file = await _factureAppService.GetByIdFactureReport(id);
            string fileDownloadName = "Facture_Template.pdf";
            var fileContentResult = File(file, "application/pdf");
            fileContentResult.FileDownloadName = fileDownloadName;
            return fileContentResult;
        }

        [HttpGet]
        public async Task<IActionResult> GetDevis(long id)
        {
            var file = await _devisAppService.GetByIdDevisReport(id);
            string fileDownloadName = "Devis_Template.pdf";
            var fileContentResult = File(file, "application/pdf");
            fileContentResult.FileDownloadName = fileDownloadName;
            return fileContentResult;
        }
    }
}
