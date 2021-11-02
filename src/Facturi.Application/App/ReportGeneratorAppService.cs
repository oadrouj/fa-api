using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DinkToPdf;
using DinkToPdf.Contracts;
using Facturi.App.Dtos;
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

		public byte[] GetByteDataFacture(Facture factureDto)
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
				HtmlContent = GetHtmlContentFacture(factureDto),
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

		private string GetHtmlContentFacture(Facture factureDto)
		{
			var facture = ObjectMapper.Map<Facture>(factureDto);
			var sb = new StringBuilder();
			
			sb.Append(@"<!doctype html>
<html lang='fr'>
<head>
  <meta charset='utf-8'>
  <title>Report</title>
	<link href='");
			sb.Append(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ReportPages\Css", "Facture.css"));
			sb.Append(@"' rel='stylesheet' type='text/css'>
</head>
<body>
  <div class='headerFacture'>
	  <div class='divImg'>
	  	<img src='");
			sb.Append(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ReportPages", "logo.png"));
			sb.Append(@"'>
	  </div>
	  <div class='divInfosFacture'>
	  	<p>Facture F");
			for (int i = 0; i < 5 - facture.Reference.ToString().Length; i++)
			{
				sb.Append('0');
			}
			sb.Append(facture.Reference);
			sb.Append(@"</p>
	  	<p class='pDate'>Date d’émission : ");
			sb.Append(facture.DateEmission.ToString("dd/MM/yyyy"));
			sb.Append(@"</p>
	  	<p class='pDate'>Date d’échéance : ");
			sb.Append(facture.DateEmission.AddDays(facture.EcheancePaiement).ToString("dd/MM/yyyy"));
			sb.Append(@"</p>
	  </div>
  </div>
  <div class='cordonneesFacture'>
	  <div class='divPour'>
	  	<p class='pPourDe'>Pour :</p>
	  	<p>");
			sb.Append(facture.Client.CategorieClient.Equals("PRTC") ? facture.Client.Nom : facture.Client.RaisonSociale);
			sb.Append(@"</p>
	  	<p class='adresse'>");
			sb.Append(facture.Client.Adresse);
			sb.Append(@"</p>
	  	<p>");
			sb.Append(facture.Client.Ville);
			sb.Append(@" ");
			sb.Append(facture.Client.CodePostal);
			sb.Append(@"</p>
	  	<p>");
			sb.Append(facture.Client.Pays);
			sb.Append(@"</p>
	  	<p class='numTel'>");
			sb.Append(facture.Client.TelPortable);
			sb.Append(@"</p>
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
	  			<th style='border-bottom: 2px solid #c9c9c9;border-top: 2px solid #c9c9c9;'><br>TVA<br><br></th>
	  			<th class='right' style='border-bottom: 2px solid #c9c9c9;border-top: 2px solid #c9c9c9;'><br>Total TTC<br><br></th>
	  		</tr>
	  	</thead>
	  	<tbody>");

			float totalMontantHT = 0;
			float totalMontantTVA = 0;
			foreach (var fi in facture.FactureItems)
			{
				float montantHT = fi.UnitPriceHT * fi.Quantity;
				float montantTVA = montantHT * fi.Tva / 100;
				float montantTTC = montantHT + montantTVA;
				totalMontantHT += montantHT;
				totalMontantTVA += montantTVA;
				sb.Append(@"<tr>
	  					<td class='left'>");
				sb.Append(fi.Description);
				sb.Append(@"</td>
	  					<td>");
				sb.Append(fi.Date.ToString("dd/MM/yyyy"));
				sb.Append(@"</td>
	  					<td>");
				sb.Append(fi.Quantity);
				sb.Append(@"</td>
	  					<td>");
				sb.Append(fi.Unit);
				sb.Append(@"</td>
	  					<td class='right'>");
				sb.Append(fi.UnitPriceHT);
				sb.Append(@" MAD</td>
	  					<td>");
				sb.Append(fi.Tva);
				sb.Append(@"%</td>
	  					<td class='right'>");
				sb.Append(montantTTC);
				sb.Append(@" MAD</td>
	  				</tr>");
			}


			sb.Append(@"
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
					  	<p>");
			sb.Append(totalMontantHT);
			sb.Append(@" MAD</p>
					  	<p>");
			sb.Append(facture.Remise);
			sb.Append(@" MAD</p>
					  	<p>");
			sb.Append(totalMontantTVA);
			sb.Append(@" MAD</p>
				  	</div>
			  </div>
			  <hr>
			  <div id='divTTC'>
				  	<div class='label'>
					  	<p>MONTANT ESTIMé TTC</p>
				  	</div>
				  	<div class='value'>
					  	<p>");
			sb.Append(totalMontantHT + totalMontantTVA - facture.Remise);
			sb.Append(@" MAD</p>
				  	</div>
			  </div>
	  	</div>
  </div>
</body>
</html>");

			return sb.ToString();
		}


		public byte[] GetByteDataDevis(Devis devisDto)
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
				HtmlContent = GetHtmlContentDevis(devisDto),
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

		private string GetHtmlContentDevis(Devis devisDto)
		{
			var devis = ObjectMapper.Map<Devis>(devisDto);
			var sb = new StringBuilder();

			sb.Append(@"<!doctype html>
<html lang='fr'>
<head>
  <meta charset='utf-8'>
  <title>Report</title>
	<link href='");
			sb.Append(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ReportPages\Css", "Facture.css"));
			sb.Append(@"' rel='stylesheet' type='text/css'>
</head>
<body>
  <div class='headerFacture'>
	  <div class='divImg'>
	  	<img src='");
			sb.Append(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ReportPages", "logo.png"));
			sb.Append(@"'>
	  </div>
	  <div class='divInfosFacture'>
	  	<p>Devis D");
			for (int i = 0; i < 5 - devis.Reference.ToString().Length; i++)
			{
				sb.Append('0');
			}
			sb.Append(devis.Reference);
			sb.Append(@"</p>
	  	<p class='pDate'>Date d’émission : ");
			sb.Append(devis.DateEmission.ToString("dd/MM/yyyy"));
			sb.Append(@"</p>
	  	<p class='pDate'>Date d’échéance : ");
			sb.Append(devis.DateEmission.AddDays(devis.EcheancePaiement).ToString("dd/MM/yyyy"));
			sb.Append(@"</p>
	  </div>
  </div>
  <div class='cordonneesFacture'>
	  <div class='divPour'>
	  	<p class='pPourDe'>Pour :</p>
	  	<p>");
			sb.Append(devis.Client.CategorieClient.Equals("PRTC") ? devis.Client.Nom : devis.Client.RaisonSociale);
			sb.Append(@"</p>
	  	<p class='adresse'>");
			sb.Append(devis.Client.Adresse);
			sb.Append(@"</p>
	  	<p>");
			sb.Append(devis.Client.Ville);
			sb.Append(@" ");
			sb.Append(devis.Client.CodePostal);
			sb.Append(@"</p>
	  	<p>");
			sb.Append(devis.Client.Pays);
			sb.Append(@"</p>
	  	<p class='numTel'>");
			sb.Append(devis.Client.TelPortable);
			sb.Append(@"</p>
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
	  			<th style='border-bottom: 2px solid #c9c9c9;border-top: 2px solid #c9c9c9;'><br>TVA<br><br></th>
	  			<th class='right' style='border-bottom: 2px solid #c9c9c9;border-top: 2px solid #c9c9c9;'><br>Total TTC<br><br></th>
	  		</tr>
	  	</thead>
	  	<tbody>");

			float totalMontantHT = 0;
			float totalMontantTVA = 0;
			foreach (var fi in devis.DevisItems)
			{
				float montantHT = fi.UnitPriceHT * fi.Quantity;
				float montantTVA = montantHT * fi.Tva / 100;
				float montantTTC = montantHT + montantTVA;
				totalMontantHT += montantHT;
				totalMontantTVA += montantTVA;
				sb.Append(@"<tr>
	  					<td class='left'>");
				sb.Append(fi.Description);
				sb.Append(@"</td>
	  					<td>");
				sb.Append(fi.Date.ToString("dd/MM/yyyy"));
				sb.Append(@"</td>
	  					<td>");
				sb.Append(fi.Quantity);
				sb.Append(@"</td>
	  					<td>");
				sb.Append(fi.Unit);
				sb.Append(@"</td>
	  					<td class='right'>");
				sb.Append(fi.UnitPriceHT);
				sb.Append(@" MAD</td>
	  					<td>");
				sb.Append(fi.Tva);
				sb.Append(@"%</td>
	  					<td class='right'>");
				sb.Append(montantTTC);
				sb.Append(@" MAD</td>
	  				</tr>");
			}


			sb.Append(@"
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
					  	<p>");
			sb.Append(totalMontantHT);
			sb.Append(@" MAD</p>
					  	<p>");
			sb.Append(devis.Remise);
			sb.Append(@" MAD</p>
					  	<p>");
			sb.Append(totalMontantTVA);
			sb.Append(@" MAD</p>
				  	</div>
			  </div>
			  <hr>
			  <div id='divTTC'>
				  	<div class='label'>
					  	<p>MONTANT ESTIMé TTC</p>
				  	</div>
				  	<div class='value'>
					  	<p>");
			sb.Append(totalMontantHT + totalMontantTVA - devis.Remise);
			sb.Append(@" MAD</p>
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
