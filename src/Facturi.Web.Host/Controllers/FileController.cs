using Facturi.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using NSwag.Annotations;
using Facturi.App;
using Abp.Domain.Repositories;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Facturi.App.Dtos;
using Facturi.App.Dtos.GenericDtos;
using Facturi.App.Dtos.InvoiceDtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.RegularExpressions;



namespace Facturi.Web.Host.Controllers
{
     [Route("file-api")]
     [ApiController]
    public class FileController : FacturiControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private readonly ReportGeneratorAppService _reportGeneratorAppService;
        private readonly IRepository<Facture, long> _factureRepository;
        private readonly IRepository<FactureInfosPaiement, long> _factureInfosPaiementRepository;
        private readonly IRepository<FactureItem, long> _factureItemRepository;
        private readonly IRepository<Devis, long> _devisRepository;
        private readonly IRepository<DevisItem, long> _devisItemRepository;
        private readonly IRepository<Client, long> _clientRepository;
        private readonly IInfosEntrepriseAppService _infosEntrepriseAppService;

        public FileController(
            IWebHostEnvironment hostingEnvironment,
            ReportGeneratorAppService reportGeneratorAppService,
            IRepository<Facture, long> FactureRepository, 
            IRepository<FactureItem, long> factureItemRepository, 
            IRepository<Devis, long> devisRepository, 
            IRepository<DevisItem, long> devisItemRepository, 
            IRepository<FactureInfosPaiement, long> factureInfosPaiementRepository,
            IRepository<Client, long> clientRepository,
            IInfosEntrepriseAppService infosEntrepriseAppService
         )
        {
            this._hostingEnvironment = hostingEnvironment;
            _factureRepository = FactureRepository ?? throw new ArgumentNullException(nameof(FactureRepository));
            _factureItemRepository = factureItemRepository ?? throw new ArgumentNullException(nameof(factureItemRepository));
            _devisRepository = devisRepository ?? throw new ArgumentNullException(nameof(devisRepository));
            _devisItemRepository = devisItemRepository ?? throw new ArgumentNullException(nameof(devisItemRepository));
            _reportGeneratorAppService = reportGeneratorAppService ?? throw new ArgumentNullException(nameof(reportGeneratorAppService));
            _factureInfosPaiementRepository = factureInfosPaiementRepository ?? throw new ArgumentNullException(nameof(factureInfosPaiementRepository));
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
            _infosEntrepriseAppService = infosEntrepriseAppService ?? throw new ArgumentNullException(nameof(infosEntrepriseAppService));
        }

        [HttpPost]
        [Route("upload")]
        public async Task<bool> Upload(IFormFile file)
        {
            try
            {
                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, @"uploads");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                if (file.Length > 0)
                {
                    //Delete image if exist
                    string findedFile = null;
                    if ((findedFile = this.findFile(uploads)) != null)
                        System.IO.File.Delete(findedFile);



                    var filePath = Path.Combine(uploads, "id_"+ AbpSession.UserId.ToString() + Path.GetExtension(file.FileName));
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
            
        }

        [HttpGet]
        [Route("download")]
        [SwaggerResponse(200, typeof(FileContentResult))]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<FileContentResult> Download()
        {
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, @"uploads");

            var file = this.findFile(uploads);
            if (file == null)
            {
		Console.WriteLine("SO IT DOESN T FIND THE FILE");
                return null;
            }

            var filePath = Path.Combine(uploads, file);

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return new FileContentResult(fileBytes, this.GetContentType(filePath))
            {
                FileDownloadName = file

            };
    }
        [HttpDelete]
        [Route("delete")]
        public bool Delete()
        {
            try
            {
                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                //Delete image if exist
                string findedFile = null;
                if ((findedFile = this.findFile(uploads)) != null)
                    System.IO.File.Delete(findedFile);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
           

        }

        private string findFile(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath);
            var result = files.Where(x => x.Contains("id_"+ AbpSession.UserId + ".")).FirstOrDefault();
            return result;
        }
        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }



        [HttpGet]
        [Route("download-backend")]
        public async Task<ActionResult> DownloadPdfFromBackend()
        {
            var file = await _reportGeneratorAppService.GetUsersAsPdfAsync();
         	return File(file.FileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file.FileName);
        }

        [HttpGet]
        [Route("download-facture")]
        public async Task<ActionResult> DownloadPdfFromBackend(long id)
        {
            var facture = await _factureRepository.GetAllIncluding(f => f.Client, f => f.FactureItems)
                .Where(f => f.Id == id)
                .ToListAsync();
            
            var infosEnteprise = await _infosEntrepriseAppService.GetCurrentUserInfosEntreprise();
            var file = await _reportGeneratorAppService.GetFactureAsPdfAsync(ObjectMapper.Map<FactureDto>(facture.First()), infosEnteprise);
         	return File(file.FileBytes, "application/pdf", file.FileName);
        }


        [HttpGet]
        [Route("download-devis")]
        public async Task<ActionResult> DownloadDevisPdfFromBackend(long id)
        {
            var devis = await _devisRepository.GetAllIncluding(d => d.Client, d => d.DevisItems)
                .Where(d => d.Id == id)
                .ToListAsync();
            
            var infosEnteprise = await _infosEntrepriseAppService.GetCurrentUserInfosEntreprise();
           

            var file = await _reportGeneratorAppService.GetDevisAsPdfAsync(ObjectMapper.Map<DevisDto>(devis.First()), infosEnteprise);
         	return File(file.FileBytes, "application/pdf", file.FileName);
        }

    }
}
