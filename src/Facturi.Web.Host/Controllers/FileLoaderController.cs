using Facturi.App;
using Facturi.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Facturi.Web.Host.Controllers
{
    public class FileLoaderController : FacturiControllerBase
    {
        private readonly IFactureAppService _factureAppService;

        public FileLoaderController(IFactureAppService factureAppService)
        {
            _factureAppService = factureAppService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFacture()
        {
            var file = await _factureAppService.GetByIdFactureReport(0);
            string fileDownloadName = "Facture_Template.pdf";
            var fileContentResult = File(file, "application/pdf");
            fileContentResult.FileDownloadName = fileDownloadName;
            return fileContentResult;
        }
    }
}
