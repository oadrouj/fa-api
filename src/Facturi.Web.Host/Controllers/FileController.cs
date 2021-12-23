using Facturi.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.StaticFiles;
using NSwag.Annotations;

namespace Facturi.Web.Host.Controllers
{
     [Route("file-api")]
     [ApiController]
    public class FileController : FacturiControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        public FileController(
            IWebHostEnvironment hostingEnvironment
         )
        {
            this._hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<bool> Upload(IFormFile file)
        {
            try
            {
                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
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

                    var filePath = Path.Combine(uploads, AbpSession.UserId.ToString() + Path.GetExtension(file.FileName));
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
        public async Task<ActionResult> Download()
        {
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

            var file = this.findFile(uploads);
            if (file == null)
            {
                return NotFound();
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
            var result = files.Where(x => x.Contains("\\" + AbpSession.UserId + ".")).FirstOrDefault();
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

    }
}