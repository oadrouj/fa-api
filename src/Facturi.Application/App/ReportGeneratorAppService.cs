using Abp.Application.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App
{
    public class ReportGeneratorAppService : ApplicationService, IReportGeneratorAppService
    {
        private readonly IConverter _converter;

        public ReportGeneratorAppService(IConverter converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public async Task<byte[]> GetByteDataFacture()
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4
            };

            var objetSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = this.GetHtmlContentFacture(),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ReportPages\Css", "Facture.css") }
            };

            var pdf = new HtmlToPdfDocument
            {
                GlobalSettings = globalSettings,
                Objects = { objetSettings }
            };

            var file = _converter.Convert(pdf);
            return file;
        }

        private string GetHtmlContentFacture()
        {
            var sb = new StringBuilder();

			sb.Append(@"<!doctype html>
<html lang='fr'>
<head>
  <meta charset='utf-8'>
  <title>Titre de la page</title>
</head>
<body>
  <div class='headerFacture'>
	  <div class='divImg'>
	  	<img src='");
			sb.Append(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ReportPages", "logo.png"));
			sb.Append(@"'>
	  </div>
	  <div class='divInfosFacture'>
	  	<p>Devis D00001</p>
	  	<p class='pDate'>Date d’émission : 01/01/2021</p>
	  	<p class='pDate'>Date d’échéance : 31/01/2021</p>
	  </div>
  </div>
  <div class='cordonneesFacture'>
	  <div class='divPour'>
	  	<p class='pPourDe'>Pour :</p>
	  	<p>Omar ATTIOUI</p>
	  	<p class='adresse'>12 Rue de Casablanca 12 Rue de Casablanca 12 Rue de Casablanca 12 Rue de Casablanca</p>
	  	<p>Casablanca 2000</p>
	  	<p>Maroc</p>
	  	<p class='numTel'>0666666666</p>
	  </div>
	  <div class='divDe'>
	  	<p class='pPourDe'>De :</p>
	  	<p>Facturi</p>
	  </div>
  </div>
  <div class='elementsFacture'>
	  <table>
	  	<thead>
	  		<tr>
	  			<th class='left' style='border-bottom: 2px solid #c9c9c9;border-top: 2px solid #c9c9c9;'><br>Description<br><br></th>
	  			<th style='border-bottom: 2px solid #c9c9c9;border-top: 2px solid #c9c9c9;'><br>Date<br><br></th>
	  			<th style='border-bottom: 2px solid #c9c9c9;border-top: 2px solid #c9c9c9;'><br>Quantité<br><br></th>
	  			<th style='border-bottom: 2px solid #c9c9c9;border-top: 2px solid #c9c9c9;'><br>Unité<br><br></th>
	  			<th class='right' style='border-bottom: 2px solid #c9c9c9;border-top: 2px solid #c9c9c9;'><br>PU HT<br><br></th>
	  			<th class='right' style='border-bottom: 2px solid #c9c9c9;border-top: 2px solid #c9c9c9;'><br>Total HT<br><br></th>
	  			<th style='border-bottom: 2px solid #c9c9c9;border-top: 2px solid #c9c9c9;'><br>TVA<br><br></th>
	  			<th class='right' style='border-bottom: 2px solid #c9c9c9;border-top: 2px solid #c9c9c9;'><br>Total TTC<br><br></th>
	  		</tr>
	  	</thead>
	  	<tbody>
	  		<tr>
	  			<td class='left'>Consultation</td>
	  			<td>01/01/2021</td>
	  			<td>8</td>
	  			<td>HEURE</td>
	  			<td class='right'>100000,00 MAD</td>
	  			<td class='right'>800000,00 MAD</td>
	  			<td>20%</td>
	  			<td class='right'>1000000,00 MAD</td>
	  		</tr>
	  		<tr>
	  			<td class='left'>Tryfa mezyana Tryfa mezyana Tryfa mezyana</td>
	  			<td>01/01/2021</td>
	  			<td>4</td>
	  			<td>PIECE</td>
	  			<td class='right'>400,00 MAD</td>
	  			<td class='right'>1600,00 MAD</td>
	  			<td>20%</td>
	  			<td class='right'>2000,00 MAD</td>
	  		</tr>
	  		<tr>
	  			<td class='left'>Consultation</td>
	  			<td>01/01/2021</td>
	  			<td>8</td>
	  			<td>HEURE</td>
	  			<td class='right'>100000,00 MAD</td>
	  			<td class='right'>800000,00 MAD</td>
	  			<td>20%</td>
	  			<td class='right'>1000000,00 MAD</td>
	  		</tr>
	  		<tr>
	  			<td class='left'>Tryfa mezyana Tryfa mezyana Tryfa mezyana</td>
	  			<td>01/01/2021</td>
	  			<td>4</td>
	  			<td>PIECE</td>
	  			<td class='right'>400,00 MAD</td>
	  			<td class='right'>1600,00 MAD</td>
	  			<td>20%</td>
	  			<td class='right'>2000,00 MAD</td>
	  		</tr>
	  		<tr>
	  			<td class='left'>Consultation</td>
	  			<td>01/01/2021</td>
	  			<td>8</td>
	  			<td>HEURE</td>
	  			<td class='right'>100000,00 MAD</td>
	  			<td class='right'>800000,00 MAD</td>
	  			<td>20%</td>
	  			<td class='right'>1000000,00 MAD</td>
	  		</tr>
	  		<tr>
	  			<td class='left'>Tryfa mezyana Tryfa mezyana Tryfa mezyana</td>
	  			<td>01/01/2021</td>
	  			<td>4</td>
	  			<td>PIECE</td>
	  			<td class='right'>400,00 MAD</td>
	  			<td class='right'>1600,00 MAD</td>
	  			<td>20%</td>
	  			<td class='right'>2000,00 MAD</td>
	  		</tr>
	  	</tbody>
	  </table>
  </div>
  <div class='totalFacture'>
	  	<div id='divTotal'>  		
			  <div id='divCalcul'>
				  	<div class='label'>
					  	<p>MONTANT TOTAL HT</p>
					  	<p>REMISE</p>
					  	<p>TVA</p>
				  	</div>
				  	<div class='value'>
					  	<p>800,00 MAD</p>
					  	<p>200,00 MAD</p>
					  	<p>120,00 MAD</p>
				  	</div>
			  </div>
			  <hr>
			  <div id='divTTC'>
				  	<div class='label'>
					  	<p>MONTANT ESTIMé TTC</p>
				  	</div>
				  	<div class='value'>
					  	<p>400,00 MAD</p>
				  	</div>
			  </div>
	  	</div>
  </div>
</body>
</html>");

            return sb.ToString();
        }

    }
}
