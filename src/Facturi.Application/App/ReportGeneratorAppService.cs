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

		public byte[] GetByteDataFacture(FactureDto factureDto)
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
				WebSettings = { DefaultEncoding = "utf-8"}
			};

			var pdf = new HtmlToPdfDocument
			{
				GlobalSettings = globalSettings,
				Objects = { objetSettings }
			};

			var file = _converter.Convert(pdf);
			return file;
		}

		private string GetHtmlContentFacture(FactureDto facture)
		{
			var sb = new StringBuilder();
			
			sb.Append(@"<!doctype html>
<html lang='fr'>
<head>
  <meta charset='utf-8'>
  <title>Report</title>
	<style type='text/css'>
html, body {
  margin: 0px;
  padding: 0px;
}
@font-face {
    font-family: 'Frutiger';
    font-style: normal;
    font-weight: normal;
    src: local('Frutiger'), url('Frutiger.woff') format('woff');
}

@font-face {
    font-family: 'Frutiger';
    font-style: normal;
    font-weight: bold;
    src: local('Frutiger Bold'), url('Frutiger_bold.woff') format('woff');
}
body {
	font-family: Frutiger;
}
.headerFacture {
	position: relative;
	width: 100%;
	min-height: 145px;
}

	.headerFacture div {
		position: absolute;
		top: 0px;
		padding: 12.65px;
		width: 53%;
	}

	.headerFacture .divImg {
		left: 0px;
	}
		.headerFacture .divImg img {
			width: 88.55px;
		}

	.headerFacture .divInfosFacture {
		right: 0px;
	}
		.headerFacture .divInfosFacture p {
			text-align: right;
			margin: 2px;
		    font-weight: bold;
    		font-size: 43px;
		}
		.headerFacture .divInfosFacture .pDate {
		    font-size: 29px;
		    color: #c9c9c9;
		}
.cordonneesFacture {
	margin-top: 20px;
	position: relative;
	width: 100%;
	min-height: 235px;
}

	.cordonneesFacture div {
		position: absolute;
		top: 0px;
		padding: 12.65px;
	}

	.cordonneesFacture .divPour {
		left: 0px;
		width: 50%;
	}
	.cordonneesFacture .divDe {
		right: 0px;
		width: 40%;
	}

		.cordonneesFacture div p {
			margin: 2px;
		    font-weight: bold;
		    font-size: 20px;
		}
		.cordonneesFacture div .pPourDe {
		    color: #c9c9c9;
		}
		.cordonneesFacture div .numTel {
		    margin-top: 20px;
		}
.elementsFacture {
	width: 100%;
}
	.elementsFacture table{
		width: 98%;
		border-collapse: separate;
    	border-spacing: 0 20px;
    	margin: auto;
	}
		.elementsFacture table thead{
			width: 100%;
		}
			.elementsFacture table thead tr{
				width: 100%;
			}
				.elementsFacture table thead tr th{
					text-align: center;
					padding: 0px 0px 0px 0px; 
					color: #0D6939;
					border-bottom: 2px solid #c9c9c9;
					border-top: 2px solid #c9c9c9;
					font-weight: bold;
					text-transform: uppercase;
					position: relative;
				}
				.elementsFacture table thead tr th.left{
					text-align: left;
				}
				.elementsFacture table thead tr th.right{
					text-align: right;
				}
				.elementsFacture table thead tr th:before{
					position: absolute;
					top: 0;
					left: 0;
					width: 100%;
					border-top: 2px solid #c9c9c9;
				}
				.elementsFacture table thead tr th:after{
					position: absolute;
					bottom: 0;
					left: 0;
					width: 100%;
					border-top: 2px solid #c9c9c9;
				}
		.elementsFacture table tbody{
			width: 100%;
		}
			.elementsFacture table tbody tr{
				width: 100%;
			}
				.elementsFacture table tbody tr td{
					max-width: 180px;
					text-align: center;
					font-weight: bold;
					position: relative;
				}
				.elementsFacture table tbody tr td.left{
					text-align: left;
				}
				.elementsFacture table tbody tr td.right{
					text-align: right;
				}
				
.totalFacture {
	width: 100%;
	position: relative;
}

	.totalFacture #divTotal {
		position: absolute;
		top: 0px;
		right: 0px;
		padding: 12.65px;
		width: 65%;
	}
		.totalFacture #divTotal hr {
			width: 60%;
			height: 0px;
			border-top: 2px solid #c9c9c9;
			text-align:right;
			margin-left: 40%;
		}

		.totalFacture #divTotal #divCalcul {
			width: 100%;
		}

		.totalFacture #divTotal #divTTC {
			width: 100%;
		}
			.totalFacture #divTotal div.label {
				width: 60%;
				float: left;
			}
			.totalFacture #divTotal div.value {
				width: 40%;
				float: left;
			}
				.totalFacture p {
					text-align: right;
					font-size: 20px;
					font-weight: bold;
					margin-top: 10px;
					text-transform: uppercase;
				}
</style >
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


		public byte[] GetByteDataDevis(DevisDto devis)
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
				HtmlContent = GetHtmlContentDevis(devis),
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

		private string GetHtmlContentDevis(DevisDto devis)
		{
			var sb = new StringBuilder();

			sb.Append(@"<!doctype html>
<html lang='fr'>
<head>
  <meta charset='utf-8'>
  <title>Report</title>
	<style type='text/css'>
html, body {
  margin: 0px;
  padding: 0px;
}
@font-face {
    font-family: 'Frutiger';
    font-style: normal;
    font-weight: normal;
    src: local('Frutiger'), url('Frutiger.woff') format('woff');
}

@font-face {
    font-family: 'Frutiger';
    font-style: normal;
    font-weight: bold;
    src: local('Frutiger Bold'), url('Frutiger_bold.woff') format('woff');
}
body {
	font-family: Frutiger;
}
.headerFacture {
	position: relative;
	width: 100%;
	min-height: 145px;
}

	.headerFacture div {
		position: absolute;
		top: 0px;
		padding: 12.65px;
		width: 53%;
	}

	.headerFacture .divImg {
		left: 0px;
	}
		.headerFacture .divImg img {
			width: 88.55px;
		}

	.headerFacture .divInfosFacture {
		right: 0px;
	}
		.headerFacture .divInfosFacture p {
			text-align: right;
			margin: 2px;
		    font-weight: bold;
    		font-size: 43px;
		}
		.headerFacture .divInfosFacture .pDate {
		    font-size: 29px;
		    color: #c9c9c9;
		}
.cordonneesFacture {
	margin-top: 20px;
	position: relative;
	width: 100%;
	min-height: 235px;
}

	.cordonneesFacture div {
		position: absolute;
		top: 0px;
		padding: 12.65px;
	}

	.cordonneesFacture .divPour {
		left: 0px;
		width: 50%;
	}
	.cordonneesFacture .divDe {
		right: 0px;
		width: 40%;
	}

		.cordonneesFacture div p {
			margin: 2px;
		    font-weight: bold;
		    font-size: 20px;
		}
		.cordonneesFacture div .pPourDe {
		    color: #c9c9c9;
		}
		.cordonneesFacture div .numTel {
		    margin-top: 20px;
		}
.elementsFacture {
	width: 100%;
}
	.elementsFacture table{
		width: 98%;
		border-collapse: separate;
    	border-spacing: 0 20px;
    	margin: auto;
	}
		.elementsFacture table thead{
			width: 100%;
		}
			.elementsFacture table thead tr{
				width: 100%;
			}
				.elementsFacture table thead tr th{
					text-align: center;
					padding: 0px 0px 0px 0px; 
					color: #0D6939;
					border-bottom: 2px solid #c9c9c9;
					border-top: 2px solid #c9c9c9;
					font-weight: bold;
					text-transform: uppercase;
					position: relative;
				}
				.elementsFacture table thead tr th.left{
					text-align: left;
				}
				.elementsFacture table thead tr th.right{
					text-align: right;
				}
				.elementsFacture table thead tr th:before{
					position: absolute;
					top: 0;
					left: 0;
					width: 100%;
					border-top: 2px solid #c9c9c9;
				}
				.elementsFacture table thead tr th:after{
					position: absolute;
					bottom: 0;
					left: 0;
					width: 100%;
					border-top: 2px solid #c9c9c9;
				}
		.elementsFacture table tbody{
			width: 100%;
		}
			.elementsFacture table tbody tr{
				width: 100%;
			}
				.elementsFacture table tbody tr td{
					max-width: 180px;
					text-align: center;
					font-weight: bold;
					position: relative;
				}
				.elementsFacture table tbody tr td.left{
					text-align: left;
				}
				.elementsFacture table tbody tr td.right{
					text-align: right;
				}
				
.totalFacture {
	width: 100%;
	position: relative;
}

	.totalFacture #divTotal {
		position: absolute;
		top: 0px;
		right: 0px;
		padding: 12.65px;
		width: 65%;
	}
		.totalFacture #divTotal hr {
			width: 60%;
			height: 0px;
			border-top: 2px solid #c9c9c9;
			text-align:right;
			margin-left: 40%;
		}

		.totalFacture #divTotal #divCalcul {
			width: 100%;
		}

		.totalFacture #divTotal #divTTC {
			width: 100%;
		}
			.totalFacture #divTotal div.label {
				width: 60%;
				float: left;
			}
			.totalFacture #divTotal div.value {
				width: 40%;
				float: left;
			}
				.totalFacture p {
					text-align: right;
					font-size: 20px;
					font-weight: bold;
					margin-top: 10px;
					text-transform: uppercase;
				}
</style >
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
