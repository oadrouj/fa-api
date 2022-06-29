using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DinkToPdf;
using DinkToPdf.Contracts;
using Facturi.App.Dtos;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Facturi.Users.Dto;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using Facturi.Authorization;
using Facturi.Authorization.Accounts;
using Facturi.Authorization.Roles;
using Facturi.Authorization.Users;
using Abp.Dependency;
using Abp.Domain.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;


namespace Facturi.App
{
    public class ReportGeneratorAppService : ApplicationService, IReportGeneratorAppService
    {
        private readonly IConverter _converter;
        private readonly IRepository<User, long> _userRepository;
  		private IWebHostEnvironment _hostingEnvironment;

        public ReportGeneratorAppService(
			 IWebHostEnvironment hostingEnvironment,
			 IRepository<User, long> userRepository, 
			 IConverter converter
		)
        {
			_hostingEnvironment = hostingEnvironment;
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

		}

		public byte[] GetByteDataFacture(FactureDto factureDto)
		{
			var globalSettings = new GlobalSettings
			{
				ColorMode = ColorMode.Color,
				Orientation = Orientation.Portrait,
				PaperSize = PaperKind.A4
			};

			var pdf = new HtmlToPdfDocument
			{
				GlobalSettings = globalSettings,
			};
			var page = new ObjectSettings()
			{
				PagesCount = true,
				HeaderSettings  = { Center = Guid.NewGuid().ToString(), Line = new Random().Next(100) % 2 == 0, Right = "Page [page] of [toPage]" },
				FooterSettings = { Center = Guid.NewGuid().ToString(), Line = new Random().Next(100) % 2 == 0, Right = "Page [page] of [toPage]" },
				WebSettings = { DefaultEncoding = "utf-8", PrintMediaType=true},
				HtmlContent = GetHtmlContentFacture(factureDto),

			};
			pdf.Objects.Add(page);
			var file = _converter.Convert(pdf);
			return file;
		}


		private string GetHtmlContentFactureAndEntreprise(FactureDto facture, InfosEntrepriseDto entreprise)
		{
			var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

            		var file = this.findFile(uploads);
			var filePath = "";

            		if (file != null){
            	 		filePath = Path.Combine(uploads, file);
			}

			
			var sb = new StringBuilder();

			string cssPath = _hostingEnvironment.ContentRootPath + @"wwwroot/arial-webfont/style.css";
			sb.Append(@"<!doctype html>
			<html lang='fr'>
			<head>
			<meta charset='utf-8'>
			<title>Report</title>
			<link rel='stylesheet' href='");
			sb.Append(cssPath); 
			sb.Append(@"'>
				<style type='text/css'>
			html, body {
			margin: 0px;
			padding: 0px;
			font-family: Arial, sans-serif !important;
			}
			table thead tr th, table tr td, p, h1,h2,h3,h4,h5,h6, ul, li, span, table, div {
			font-family: Arial, sans-serif !important;
			}

			.cordonneesFacture {
				margin-top: 20px;
				position: relative;
				width: 100%;
				height: 235px;
			}.spacer{height:20px;}

				.cordonneesFacture div {
					position: absolute;
					top: 0px;
					padding: 12.65px;
				}

				.cordonneesFacture .divPour {
					left: 0px;
					width: 45%;
				}

				.cordonneesFacture .divPour .divPourLabel {
						left: 0px;
					
				}
				.pPourDe {font-size: 26px;line-height:37px !important;}

				.cordonneesFacture .divPour .divPourContent {
						left: 79px;
				}
				.cordonneesFacture .divPour .divPourContent p, 
				.cordonneesFacture .divDe .divDeContent p  {font-size:20px;}
				
				.raison-sociale{line-height:40px !important;}
				
				.adresse, .ville, .pays, .numTel, .ice{font-weight: 300 !important;letter-spacing:1px;}
				.adresse, .ville, .pays {text-transform:lowercase;}
				.adresse::first-letter, .ville::first-letter, .pays::first-letter{text-transform:capitalize;}
				
				.cordonneesFacture .divDe {
					right: 0px;
					width: 40%;
				}
				.cordonneesFacture .divDe .divDeLabel {
						left: 0px;	
				}
				.cordonneesFacture .divDe .divDeContent {
						left: 60px;	
				}

					.cordonneesFacture div p {
						margin: 2px;
					}
					.cordonneesFacture div .pPourDe {
						color: #898989;
					}
					.cordonneesFacture div .numTel {
						margin-top: 20px;
					}
			.spacer{height:10px;}

			.header-message{
				width:86%;
				max-height: 100px;
			}
				.header-message .message{
					
					font-size: 22px;
					font-weight:300 !important;
					margin-bottom: 0px;
				}
			.elementsFacture {
				width: 100%;
			}
				.elementsFacture table{
					width: 100%;
					border-collapse: collapse;
					border-spacing:0px;
					margin: auto;
				}
					.elementsFacture table thead{
						width: 100%;
					}
						.elementsFacture table thead tr{
							width: 100%;
							border-bottom: 2px solid #c9c9c9;
							border-top: 2px solid #c9c9c9;
						}
							.elementsFacture table thead tr th{
								text-align: center;
								padding: 0px 0px 0px 0px; 
								color: #131313;
								font-weight: 500;
								text-transform: uppercase;
								position: relative;
								font-size:20px;
							
							}
							.elementsFacture table thead tr th.left{
								text-align: left;
							}
							.elementsFacture table thead tr th.right{
								text-align: right;
							}
						
					.elementsFacture table tbody{
						width: 100%;
					}
						.elementsFacture table tbody tr{
							width: 100%;
						}
							.elementsFacture table tbody tr td{
								
								text-align: center;
								font-weight: 300;
								vertical-align:middle;
								font-size:20px;
								padding:15px;
								border-bottom: 1px solid #c9c9c9;


							}
							.elementsFacture table tbody tr td.left{
								text-align: left;
							}
							.elementsFacture table tbody tr td.right{
								text-align: right;
							}
							.elementsFacture table tbody tr td.tva{
							}
			.designation{
				width:300px !important;
			}
			.date{
				width:90px !important;
			}
			.quantity{
				width:80px !important;
			}
			.unite{
				width:80px !important;
			}
			.pu-ht{
				width:120px !important;
			}
			.tva{
				width:80px !important;
			}
			.pu-ttc{
				width:120px !important;
			}


			.totalFacture {
				position:absolute;
				right:0px;
				width: 60%;
				/* position: relative; */
			}

				.totalFacture #divTotal {
					position: absolute;
					top: 0px;
					right: 0px;
					padding: 12.65px;
					width: 100%;
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
								font-size: 22px;
								font-weight: 500;
								margin-top: 10px;
								
							}
			.facture-synthese{
				position:relative;
				width: 100%;
			}
			.letter-amount{
				position:absolute;
				left:0px;
				width: 40%;
				font-size: 15px;
				font-weight:400;
				font-style:italic;
			}
			/* @media print {
			.footer-message {
				position: fixed;
				bottom: 0;
			}
			} */
			</style>
			</head>
			<body>
			<div class='cordonneesFacture'>
				<div class='divPour'>
					<div class='divPourLabel'>
						<p class='pPourDe'>Pour :</p>
					</div>
				<div class='divPourContent'>
						<p class='raison-sociale'>");
							sb.Append(facture.Client.CategorieClient.Equals("PRTC") ? facture.Client.Nom : facture.Client.RaisonSociale);
							sb.Append(@"</p>");

						if(facture.Client.Adresse != null && facture.Client.Adresse.Length > 0 ){
							string adresse = facture.Client.Adresse;
							if(facture.Client.Adresse.Length > 128) adresse = facture.Client.Adresse.Substring(128);
							sb.Append(@"<p class='adresse'>");
							sb.Append(adresse);
							sb.Append(@"</p>");
						}

						if(facture.Client.Ville != null && facture.Client.Ville.Length > 0 ){
							string ville = facture.Client.Ville;
							string postCode = facture.Client.CodePostal;
							
							if(facture.Client.Ville.Length > 25) ville= facture.Client.Ville.Substring(25);
							if(facture.Client.CodePostal.Length > 10) postCode = facture.Client.CodePostal.Substring(10);
							sb.Append(@"<p class='ville'>");
							sb.Append(ville);
							sb.Append(@", ");
							sb.Append(postCode);
							sb.Append(@"</p>");
						}
						if(facture.Client.Pays != null && facture.Client.Pays.Length > 0 ){
							if(facture.Client.Pays.Length > 30) facture.Client.Pays = facture.Client.Pays.Substring(30);
							sb.Append(@"<p class='pays'>");
							sb.Append(facture.Client.Pays);
							sb.Append(@"</p>");
						}
						if(facture.Client.TelPortable != null && facture.Client.TelPortable.Length > 0 ){
							if(facture.Client.TelPortable.Length > 20) facture.Client.TelPortable = facture.Client.TelPortable.Substring(20);
							sb.Append(@"<p class='numTel'>");
							sb.Append("Tél : " + facture.Client.TelPortable);
							sb.Append(@"</p>");
						}
						if(facture.Client.ICE != null && facture.Client.ICE.Length > 0 ){
							if(facture.Client.ICE.Length > 30) facture.Client.ICE = facture.Client.ICE.Substring(30);
							sb.Append(@"<p class='ice'>");
							sb.Append("ICE : " + facture.Client.ICE);
							sb.Append(@"</p>");
						}
							
						sb.Append(@"
					</div>
				</div>
				<div class='divDe'>
				<div class='divDeLabel'>
						<p class='pPourDe'>De :</p>
					</div>
				<div class='divDeContent'>
						<p class='raison-sociale'>");
							sb.Append(entreprise.RaisonSociale);
							sb.Append(@"</p>");
							if(entreprise.Adresse != null && entreprise.Adresse.Length > 0 ){
								string adresse = entreprise.Adresse;
								
								if(entreprise.Adresse.Length > 128) adresse = entreprise.Adresse.Substring(128);
								sb.Append(@"<p class='adresse'>");
								sb.Append(adresse);
								sb.Append(@"</p>");
							}

						if(entreprise.Ville != null && entreprise.Ville.Length > 0 ){
							if(entreprise.Ville.Length > 25) entreprise.Ville = entreprise.Ville.Substring(25);
							if(entreprise.CodePostal.Length > 10) entreprise.CodePostal = entreprise.CodePostal.Substring(10);
							sb.Append(@"<p class='ville'>");
							sb.Append(entreprise.Ville);
							sb.Append(@", ");
							sb.Append(entreprise.CodePostal);
							sb.Append(@"</p>");
						}
						if(entreprise.Pays != null && entreprise.Pays.Length > 0 ){
							if(entreprise.Pays.Length > 30) entreprise.Pays = entreprise.Pays.Substring(30);
							sb.Append(@"<p class='pays'>");
							sb.Append(entreprise.Pays);
							sb.Append(@"</p>");
						}
						if(entreprise.Telephone != null && entreprise.Telephone.Length > 0 ){
							string telephone = entreprise.Telephone;

							if(entreprise.Telephone.Length > 20) telephone = entreprise.Telephone.Substring(20);
							sb.Append(@"<p class='numTel'>");
							sb.Append("Tél : " + telephone);
							sb.Append(@"</p>");
						}
							
						sb.Append(@"
					</div>
				</div>
			</div>
			<div class='spacer'></div>
			<div class='header-message'>");
				if(facture.MessageIntroduction != null && facture.MessageIntroduction.Length > 0 ){
								string invoiceIntroMessage = facture.MessageIntroduction;
								if(facture.MessageIntroduction.Length > 155) invoiceIntroMessage = facture.MessageIntroduction.Substring(0,155);
								sb.Append(@"<p class='message'>");
								sb.Append(invoiceIntroMessage);
								sb.Append(@"</p>");
				}else{
					if(entreprise.InvoiceIntroMessage != null && entreprise.InvoiceIntroMessage.Length > 0 ){
						
						string invoiceIntroMessage = entreprise.InvoiceIntroMessage;
						if(entreprise.InvoiceIntroMessage.Length > 155) invoiceIntroMessage = entreprise.InvoiceIntroMessage.Substring(0,155);
						sb.Append(@"<p class='message'>");
						sb.Append(invoiceIntroMessage);
						sb.Append(@"</p>");
					}
				}
				
				sb.Append(@"
			</div>
			<div class='spacer'></div>
			<div class='elementsFacture'>
				<table>
					<thead>
						<tr>
							<th class='left designation'><br>Désignation<br><br></th>
							<th class='date'><br>Date<br><br></th>
							<th class='quantity'><br>&nbsp;&nbsp;Qté<br><br></th>
							<th class='unite'><br>Unité<br><br></th>
							<th class='pu-ht'><br>PU HT<br><br></th>
							<th class='tva'><br>TVA<br><br></th>
							<th class='pu-ttc right'><br>Total TTC<br><br></th>
						</tr>
					</thead>
				</table>
				<table>
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
									<td class='left designation' style='width:450px !important;'>");
							sb.Append(fi.Designation);
							sb.Append(@"</td>
									<td class='date'>");
							sb.Append(fi.Date.ToString("dd/MM/yyyy"));
							sb.Append(@"</td>
									<td class='quantity'>");
							sb.Append(fi.Quantity);
							sb.Append(@"</td>
									<td class='unite'>");
							sb.Append(fi.Unit);
							sb.Append(@"</td>
									<td class='pu-ht'>");
							sb.Append(fi.UnitPriceHT);
							sb.Append(@"</td>
									<td class='tva'>");
							sb.Append(fi.Tva);
							sb.Append(@"%</td>
									<td class='right pu-ttc'>");
							sb.Append(montantTTC);
							sb.Append(@"</td>
								</tr>");
						}


						sb.Append(@"
					</tbody>
				</table>
			</div>
			<div class='facture-synthese'>");
				var totalMontanttHTNet = totalMontantHT - (facture.Remise * totalMontantHT /100) ;
				var totalMontanttTVANet = totalMontantTVA - (facture.Remise * totalMontantTVA /100);
				var totalMontanttTTCNet = totalMontanttHTNet + totalMontanttTVANet;
				sb.Append(@"

				<div class='letter-amount'>
					<div style='height:12px;'></div>");
				
						string isNegative = "";  
						var montantLetter = totalMontanttTTCNet;
						string montantLetterStr = montantLetter.ToString();  
						string currencyFull = facture.Currency;
						if (facture.Currency =="MAD") currencyFull= "Dirham";
						if (facture.Currency =="USD") currencyFull= "Dollar";
						if (facture.Currency =="EUR") currencyFull= "Euro";
						Console.WriteLine("Montant LetterStr "+ montantLetterStr);    
					
						if (montantLetterStr.Contains("-"))  
						{  
							isNegative = "Moins ";  
							montantLetterStr = montantLetterStr.Substring(1, montantLetterStr.Length - 1);  
						}  
						if (montantLetterStr == "0")  
						{  
							Console.WriteLine("Zéro "+ facture.Currency);  
							sb.Append("Zéro "+ facture.Currency);

						}  
						else  
						{  
							Console.WriteLine("The number in currency fomat is \n{0}", isNegative + ConvertToWords(montantLetterStr));  

							sb.Append("Arrêter la présente facture à la somme de </br><span style='font-weight:600;'>"+isNegative + ConvertToWords(montantLetterStr) +" " + currencyFull +" TTC </span>");

						}   
					

				sb.Append(@"</div>
				<div class='totalFacture'>
						<div id='divTotal'>  		
							<div id='divCalcul'>
									<div class='label'>
										<p>Montant Total HT</p>");
										if(facture.Remise != null && facture.Remise != 0 ){	
											sb.Append(@"<p>Remise (");
											sb.Append(facture.Remise);
											sb.Append("%)</p>");
											sb.Append(@"<p>Total HT après remise (");
											sb.Append(facture.Remise);
											sb.Append("%)</p>");

										}
										sb.Append(@"<p>TVA</p>
									</div>
									<div class='value'>
										<p>");
										sb.Append(totalMontantHT.ToString("0.00"));
										sb.Append(" "+facture.Currency);
										sb.Append(@"</p>
										<p>");

										if(facture.Remise != null && facture.Remise > 0 ){			
											sb.Append(@"<p class='remise'>");
											sb.Append((facture.Remise * totalMontantHT /100).ToString("0.00"));
											sb.Append(" "+facture.Currency);
											sb.Append(@"</p>");
											sb.Append(@"<p class='remise'>");
											sb.Append((totalMontantHT - (facture.Remise * totalMontantHT /100)).ToString("0.00"));
											sb.Append(" "+facture.Currency);
											sb.Append(@"</p>
													<p>");
										}
										sb.Append((totalMontantTVA - (facture.Remise * totalMontantTVA /100)).ToString("0.00"));
										sb.Append(" "+facture.Currency);
										sb.Append(@"</p>
									</div>
							</div>
							<hr>
							<div id='divTTC'>
									<div class='label'>
										<p>Montant Total TTC</p>
									</div>
									<div class='value'>
										<p>");
									
							sb.Append(totalMontanttTTCNet.ToString("0.00"));
							sb.Append(" "+facture.Currency);
							sb.Append(@"</p>
									</div>
							</div>
						</div>
				</div>
			</div>

			</html>");

			return sb.ToString();
		}

		private string GetHtmlContentDevisAndEntreprise(DevisDto devis, InfosEntrepriseDto entreprise)
		{
			var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

            var file = this.findFile(uploads);
			var filePath = "";

            if (file != null){
            	 filePath = Path.Combine(uploads, file);
			}

			var sb = new StringBuilder();

			string cssPath = @"file:///" + _hostingEnvironment.ContentRootPath + @"/wwwroot/arial-webfont/style.css";
			sb.Append(@"<!doctype html>
			<html lang='fr'>
			<head>
			<meta charset='utf-8'>
			<title>Report</title>
			<link rel='stylesheet' href='");
			sb.Append(cssPath); 
			sb.Append(@"'>
				<style type='text/css'>
			html, body {
			margin: 0px;
			padding: 0px;
			font-family: Arial, sans-serif !important;
			}
			table thead tr th, table tr td, p, h1,h2,h3,h4,h5,h6, ul, li, span, table, div {
			font-family: Arial, sans-serif !important;
			}

			.cordonneesFacture {
				margin-top: 20px;
				position: relative;
				width: 100%;
				height: 235px;
			}.spacer{height:20px;}

				.cordonneesFacture div {
					position: absolute;
					top: 0px;
					padding: 12.65px;
				}

				.cordonneesFacture .divPour {
					left: 0px;
					width: 45%;
				}

				.cordonneesFacture .divPour .divPourLabel {
						left: 0px;
					
				}
				.pPourDe {font-size: 26px;line-height:37px !important;}

				.cordonneesFacture .divPour .divPourContent {
						left: 78px;
				}
				.cordonneesFacture .divPour .divPourContent p, 
				.cordonneesFacture .divDe .divDeContent p  {font-size:20px;}
				
				.raison-sociale{line-height:42px !important;}
				.adresse, .ville, .pays, .numTel, .ice{font-weight: 300 !important;letter-spacing:1px;}
				.adresse, .ville, .pays {text-transform:lowercase;}
				.adresse::first-letter, .ville::first-letter, .pays::first-letter{text-transform:capitalize;}
				
				.cordonneesFacture .divDe {
					right: 0px;
					width: 40%;
				}
				.cordonneesFacture .divDe .divDeLabel {
						left: 0px;	
				}
				.cordonneesFacture .divDe .divDeContent {
						left: 60px;	
				}

					.cordonneesFacture div p {
						margin: 2px;
					}
					.cordonneesFacture div .pPourDe {
						color: #898989;
					}
					.cordonneesFacture div .numTel {
						margin-top: 20px;
					}
			.spacer{height:10px;}

			.header-message{
				width:86%;
				max-height: 100px;
			}
				.header-message .message{
					
					font-size: 24px;
					font-weight:300;
					margin-bottom: 0px;
				}
			.elementsFacture {
				width: 100%;
			}
				.elementsFacture table{
					width: 100%;
					border-collapse: collapse;
					border-spacing:0px;
					margin: auto;
				}
					.elementsFacture table thead{
						width: 100%;
					}
						.elementsFacture table thead tr{
							width: 100%;
							border-bottom: 2px solid #c9c9c9;
							border-top: 2px solid #c9c9c9;
						}
							.elementsFacture table thead tr th{
								text-align: center;
								padding: 0px 0px 0px 0px; 
								color: #131313;
								font-weight: 500;
								text-transform: uppercase;
								position: relative;
								font-size:20px;
							
							}
							.elementsFacture table thead tr th.left{
								text-align: left;
							}
							.elementsFacture table thead tr th.right{
								text-align: right;
							}
						
					.elementsFacture table tbody{
						width: 100%;
					}
						.elementsFacture table tbody tr{
							width: 100%;
						}
							.elementsFacture table tbody tr td{
								
								text-align: center;
								font-weight: 300;
								vertical-align:middle;
								font-size:20px;
								padding:15px;
								border-bottom: 1px solid #c9c9c9;


							}
							.elementsFacture table tbody tr td.left{
								text-align: left;
							}
							.elementsFacture table tbody tr td.right{
								text-align: right;
							}
							.elementsFacture table tbody tr td.tva{
							}
			.designation{
				width:300px !important;
			}
			.date{
				width:90px !important;
			}
			.quantity{
				width:80px !important;
			}
			.unite{
				width:80px !important;
			}
			.pu-ht{
				width:120px !important;
			}
			.tva{
				width:80px !important;
			}
			.pu-ttc{
				width:120px !important;
			}


			.totalFacture {
				position:absolute;
				right:0px;
				width: 60%;
				/* position: relative; */
			}

				.totalFacture #divTotal {
					position: absolute;
					top: 0px;
					right: 0px;
					padding: 12.65px;
					width: 100%;
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
								font-size: 22px;
								font-weight: 500;
								margin-top: 10px;
							}
			.facture-synthese{
				position:relative;
				width: 100%;
			}
			.letter-amount{
				position:absolute;
				left:0px;
				width: 40%;
				font-size: 15px;
				font-weight:400;
				font-style:italic;
			}
			/* @media print {
			.footer-message {
				position: fixed;
				bottom: 0;
			}
			} */
			</style>
			</head>
			<body>
			<div class='cordonneesFacture'>
				<div class='divPour'>
					<div class='divPourLabel'>
						<p class='pPourDe'>Pour :</p>
					</div>
				<div class='divPourContent'>
						<p class='raison-sociale'>");

							sb.Append(devis.Client.CategorieClient.Equals("PRTC") ? devis.Client.Nom : devis.Client.RaisonSociale);
							sb.Append(@"</p>");

						if(devis.Client.Adresse != null && devis.Client.Adresse.Length > 0 ){
							string adresse = devis.Client.Adresse;
							if(devis.Client.Adresse.Length > 128) adresse = devis.Client.Adresse.Substring(128);
							sb.Append(@"<p class='adresse'>");
							sb.Append(adresse);
							sb.Append(@"</p>");
						}

						if(devis.Client.Ville != null && devis.Client.Ville.Length > 0 ){
							string ville = devis.Client.Ville;
							string postCode = devis.Client.CodePostal;
							
							if(devis.Client.Ville.Length > 25) ville= devis.Client.Ville.Substring(25);
							if(devis.Client.CodePostal.Length > 10) postCode = devis.Client.CodePostal.Substring(10);
							sb.Append(@"<p class='ville'>");
							sb.Append(ville);
							sb.Append(@", ");
							sb.Append(postCode);
							sb.Append(@"</p>");
						}
						if(devis.Client.Pays != null && devis.Client.Pays.Length > 0 ){
							if(devis.Client.Pays.Length > 30) devis.Client.Pays = devis.Client.Pays.Substring(30);
							sb.Append(@"<p class='pays'>");
							sb.Append(devis.Client.Pays);
							sb.Append(@"</p>");
						}
						if(devis.Client.TelPortable != null && devis.Client.TelPortable.Length > 0 ){
							if(devis.Client.TelPortable.Length > 20) devis.Client.TelPortable = devis.Client.TelPortable.Substring(20);
							sb.Append(@"<p class='numTel'>");
							sb.Append("Tél : " + devis.Client.TelPortable);
							sb.Append(@"</p>");
						}
						if(devis.Client.ICE != null && devis.Client.ICE.Length > 0 ){
							if(devis.Client.ICE.Length > 30) devis.Client.ICE = devis.Client.ICE.Substring(30);
							sb.Append(@"<p class='ice'>");
							sb.Append("ICE : " + devis.Client.ICE);
							sb.Append(@"</p>");
						}
							
						sb.Append(@"
					</div>
				</div>
				<div class='divDe'>
				<div class='divDeLabel'>
						<p class='pPourDe'>De :</p>
					</div>
				<div class='divDeContent'>
						<p class='raison-sociale'>");
							sb.Append(entreprise.RaisonSociale);
							sb.Append(@"</p>");
							if(entreprise.Adresse != null && entreprise.Adresse.Length > 0 ){
								string adresse = entreprise.Adresse;
								
								if(entreprise.Adresse.Length > 128) adresse = entreprise.Adresse.Substring(128);
								sb.Append(@"<p class='adresse'>");
								sb.Append(adresse);
								sb.Append(@"</p>");
							}

						if(entreprise.Ville != null && entreprise.Ville.Length > 0 ){
							if(entreprise.Ville.Length > 25) entreprise.Ville = entreprise.Ville.Substring(25);
							if(entreprise.CodePostal.Length > 10) entreprise.CodePostal = entreprise.CodePostal.Substring(10);
							sb.Append(@"<p class='ville'>");
							sb.Append(entreprise.Ville);
							sb.Append(@", ");
							sb.Append(entreprise.CodePostal);
							sb.Append(@"</p>");
						}
						if(entreprise.Pays != null && entreprise.Pays.Length > 0 ){
							if(entreprise.Pays.Length > 30) entreprise.Pays = entreprise.Pays.Substring(30);
							sb.Append(@"<p class='pays'>");
							sb.Append(entreprise.Pays);
							sb.Append(@"</p>");
						}
						if(entreprise.Telephone != null && entreprise.Telephone.Length > 0 ){
							string telephone = entreprise.Telephone;

							if(entreprise.Telephone.Length > 20) telephone = entreprise.Telephone.Substring(20);
							sb.Append(@"<p class='numTel'>");
							sb.Append("Tél : " + telephone);
							sb.Append(@"</p>");
						}
							
						sb.Append(@"
					</div>
				</div>
			</div>
			<div class='spacer'></div>
			<div class='header-message'>");
			if(devis.MessageIntroduction != null && devis.MessageIntroduction.Length > 0 ){
							string invoiceIntroMessage = devis.MessageIntroduction;
							if(devis.MessageIntroduction.Length > 155) invoiceIntroMessage = devis.MessageIntroduction.Substring(0,155);
							sb.Append(@"<p class='message'>");
							sb.Append(invoiceIntroMessage);
							sb.Append(@"</p>");
			}else{
				if(entreprise.EstimateIntroMessage != null && entreprise.EstimateIntroMessage.Length > 0 ){
							string invoiceIntroMessage = entreprise.EstimateIntroMessage;
							if(entreprise.EstimateIntroMessage.Length > 155) invoiceIntroMessage = entreprise.EstimateIntroMessage.Substring(0,155);
							sb.Append(@"<p class='message'>");
							sb.Append(invoiceIntroMessage);
							sb.Append(@"</p>");
				}
			}

				sb.Append(@"
			</div>
			<div class='spacer'></div>
			<div class='elementsFacture'>
				<table>
					<thead>
						<tr>
							<th class='left designation'><br>Désignation<br><br></th>
							<th class='date'><br>Date<br><br></th>
							<th class='quantity'><br>&nbsp;&nbsp;Qté<br><br></th>
							<th class='unite'><br>Unité<br><br></th>
							<th class='pu-ht'><br>PU HT<br><br></th>
							<th class='tva'><br>TVA<br><br></th>
							<th class='pu-ttc right'><br>Total TTC<br><br></th>
						</tr>
					</thead>
				</table>
				<table>
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
									<td class='left designation' style='width:450px !important;'>");
							sb.Append(fi.Designation);
							sb.Append(@"</td>
									<td class='date'>");
							sb.Append(fi.Date.ToString("dd/MM/yyyy"));
							sb.Append(@"</td>
									<td class='quantity'>");
							sb.Append(fi.Quantity);
							sb.Append(@"</td>
									<td class='unite'>");
							sb.Append(fi.Unit);
							sb.Append(@"</td>
									<td class='pu-ht'>");
							sb.Append(fi.UnitPriceHT);
							sb.Append(@"</td>
									<td class='tva'>");
							sb.Append(fi.Tva);
							sb.Append(@"%</td>
									<td class='right pu-ttc'>");
							sb.Append(montantTTC);
							sb.Append(@"</td>
								</tr>");
						}


						sb.Append(@"
					</tbody>
				</table>
			</div>
			<div class='facture-synthese'>");
				var totalMontanttHTNet = totalMontantHT - (devis.Remise * totalMontantHT /100) ;
				var totalMontanttTVANet = totalMontantTVA - (devis.Remise * totalMontantTVA /100);
				var totalMontanttTTCNet = totalMontanttHTNet + totalMontanttTVANet;
				sb.Append(@"						
				<div class='letter-amount'>
					<div style='height:12px;'></div>");
				
						string isNegative = "";  
						var montantLetter = totalMontanttTTCNet;
						string montantLetterStr = montantLetter.ToString();  
						Console.WriteLine("Montant LetterStr "+ montantLetterStr);    
					
						string currencyFull = devis.Currency;
						if (devis.Currency =="MAD") currencyFull= "Dirham";
						if (devis.Currency =="USD") currencyFull= "Dollar";
						if (devis.Currency =="EUR") currencyFull= "Euro";

						if (montantLetterStr.Contains("-"))  
						{  
							isNegative = "Moins ";  
							montantLetterStr = montantLetterStr.Substring(1, montantLetterStr.Length - 1);  
						}  
						if (montantLetterStr == "0")  
						{  
							Console.WriteLine("Zéro "+ devis.Currency);  
							sb.Append("Zéro "+ devis.Currency);

						}  
						else  
						{  
							Console.WriteLine("The number in currency fomat is \n{0}", isNegative + ConvertToWords(montantLetterStr));  						
							sb.Append("Arrêter le présent devis à la somme de </br><span style='font-weight:600;'>"+isNegative + ConvertToWords(montantLetterStr) +" " + currencyFull +" TTC </span>");

						}   
					

				sb.Append(@"</div>
				<div class='totalFacture'>
						<div id='divTotal'>  		
							<div id='divCalcul'>
									<div class='label'>
										<p>Montant Total HT</p>");
										if(devis.Remise != null && devis.Remise != 0 ){	
											sb.Append(@"<p>Remise (");
											sb.Append(devis.Remise);
											sb.Append("%)</p>");
											sb.Append(@"<p>Total HT après remise (");
											sb.Append(devis.Remise);
											sb.Append("%)</p>");

										}
										sb.Append(@"<p>TVA</p>
									</div>
									<div class='value'>
										<p>");
										sb.Append(totalMontantHT.ToString("0.00"));
										sb.Append(" "+devis.Currency);
										sb.Append(@"</p>
										<p>");

										if(devis.Remise != null && devis.Remise > 0 ){			
											sb.Append(@"<p class='remise'>");
											sb.Append((devis.Remise * totalMontantHT /100).ToString("0.00"));
											sb.Append(" "+devis.Currency);
											sb.Append(@"</p>");
											sb.Append(@"<p class='remise'>");
											sb.Append((totalMontantHT - (devis.Remise * totalMontantHT /100)).ToString("0.00"));
											sb.Append(" "+devis.Currency);
											sb.Append(@"</p>
													<p>");
										}
										sb.Append((totalMontantTVA - (devis.Remise * totalMontantTVA /100)).ToString("0.00"));
										sb.Append(" "+devis.Currency);
										sb.Append(@"</p>
									</div>
							</div>
							<hr>
							<div id='divTTC'>
									<div class='label'>
										<p>Montant Total TTC</p>
									</div>
									<div class='value'>
										<p>");
										
							sb.Append(totalMontanttTTCNet.ToString("0.00"));
							sb.Append(" "+devis.Currency);
							sb.Append(@"</p>
									</div>
							</div>
						</div>
				</div>
			</div>

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
				WebSettings = { DefaultEncoding = "utf-8" }
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
			</style>
			</head>
			<body>
			<div class='headerFacture'>
				<div class='divImg'>
					<img src='");
						sb.Append(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/ReportPages", "logo.png"));
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
							<th class='left' style='border-bottom: 2px solid #c9c9c9;border-top: 2px solid #c9c9c9;'><br>Designation<br><br></th>
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
							sb.Append(fi.Designation);
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
						sb.Append(totalMontantHT + totalMontantTVA - (devis.Remise * totalMontantHT /100));
						sb.Append(@" MAD</p>
								</div>
						</div>
					</div>
			</div>
			</body>
			</html>");

			return sb.ToString();
		}


 		public async Task<FileDto> GetUsersAsPdfAsync() {
            var users = await _userRepository.GetAllListAsync();
            var html = ConvertUserListToHtmlTable(users);
			var doc = new HtmlToPdfDocument()
				{
					GlobalSettings = {
						PaperSize = PaperKind.A4,
						Orientation = Orientation.Portrait
					},
					Objects = {
						new ObjectSettings()
						{
							HtmlContent = html
						}
					}
				};
			return new FileDto("UserList.pdf", _converter.Convert(doc));
		}

        private string ConvertUserListToHtmlTable(List<User> users)                                                                       { 
            var header1 = "<th>Username</th>";
            var header2 = "<th>Name</th>";
            var header3 = "<th>Surname</th>";
            var header4 = "<th>Email Address</th>";
            var headers = $"<tr>{header1}{header2}{header3}{header4}</tr>";
            var rows = new StringBuilder();
            foreach (var user in users)
            {
             var column1 = $"<td>{user.UserName}</td>";
             var column2 = $"<td>{user.Name}</td>";
             var column3 = $"<td>{user.Surname}</td>";
             var column4 = $"<td>{user.EmailAddress}</td>";
             var row = $"<tr>{column1}{column2}{column3}{column4}</tr>";
             rows.Append(row);
            }


          	return $"<table>{headers}{rows.ToString()}</table>";
        }

		public async Task<FileDto> GetFactureAsPdfAsync(FactureDto factureDto, InfosEntrepriseDto infosEnteprise) {

			var html = GetHtmlContentFactureAndEntreprise(factureDto, infosEnteprise);
			var globalSettings = new GlobalSettings
			{
				ColorMode = ColorMode.Color,
				Orientation = Orientation.Portrait,
				PaperSize = PaperKind.A4
			};

			var pdf = new HtmlToPdfDocument
				{
					GlobalSettings = globalSettings,
				};

				//INIT THE FOOTER HTML FILE FROM FOOTER BLANK BECAUSE IT S OVERRIDED EACH TIME
				string footerPath = _hostingEnvironment.WebRootPath + @"/templates/footer.html";
				string footerPathBlank = _hostingEnvironment.WebRootPath + @"/templates/footer-blank.html";
				string textInit = File.ReadAllText(footerPathBlank);
				File.WriteAllText(footerPath, textInit);
				//

				var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

				var file = this.findFile(uploads);
				var filePath = "";

				if (file != null){
					filePath = Path.Combine(uploads, file);
				}

			  	string cssArialPath = _hostingEnvironment.ContentRootPath + @"/wwwroot/arial-webfont/ARIALLGT_footer.woff";
				cssArialPath = cssArialPath.Replace("\\", "/");

				/* cssArialPath = cssArialPath.Replace(":/", "://"); */


				string factureRef = "";
				for (int i = 0; i < 5 - factureDto.Reference.ToString().Length; i++)
				{
					factureRef += "0";
				}
				factureRef += factureDto.Reference.ToString();


				//INIT THE HEADER HTML FILE FROM FOOTER BLAN°°°K BECAUSE IT S OVERRIDED EACH TIME
				string headerPath = _hostingEnvironment.WebRootPath + @"/templates/header.html";
				string headerPathBlank = _hostingEnvironment.WebRootPath + @"/templates/header-blank.html";
				string textHeaderInit = File.ReadAllText(headerPathBlank);
				File.WriteAllText(headerPath, textHeaderInit);
				//

				
				

				
				string textFooter = File.ReadAllText(footerPath);
				string textHeader = File.ReadAllText(headerPath);
			
				if(factureDto.PiedDePage != null && factureDto.PiedDePage.Length > 0 ){
						string	 invoiceFooter = factureDto.PiedDePage;
						if(factureDto.PiedDePage.Length > 140) {
							invoiceFooter = factureDto.PiedDePage.Substring(0,140);
						}
						textFooter = textFooter.Replace("@[INVOICE-FOOTER-MESSAGE]", invoiceFooter);
						textFooter = textFooter.Replace("@[CSS-ARIAL-PATH]", cssArialPath);
						File.WriteAllText(footerPath, textFooter);
				}else{
					if(infosEnteprise.InvoiceFooter != null && infosEnteprise.InvoiceFooter.Length > 0 ){
						string	 invoiceFooter = infosEnteprise.InvoiceFooter;
						if(infosEnteprise.InvoiceFooter.Length > 140) {
							invoiceFooter = infosEnteprise.InvoiceFooter.Substring(0,140);
						}
						textFooter = textFooter.Replace("@[INVOICE-FOOTER-MESSAGE]", invoiceFooter);
						textFooter = textFooter.Replace("@[CSS-ARIAL-PATH]", cssArialPath);
						File.WriteAllText(footerPath, textFooter);
					}
				}
				
				

				textHeader = textHeader.Replace("@[REF-FACTURE]", factureRef);
				textHeader = textHeader.Replace("@[IMAGE-LOGO]", filePath);
				textHeader = textHeader.Replace("@[DATE-FACTURE]", factureDto.DateEmission.ToString("dd/MM/yyyy"));
				textHeader = textHeader.Replace("@[ECHEANCE-PAIEMENT]", factureDto.DateEmission.AddDays(factureDto.EcheancePaiement).ToString("dd/MM/yyyy"));
				//textHeader = text.Replace("@[IMAGE-LOGO]", filePath);
				File.WriteAllText(headerPath, textHeader);
				
			

			var page = new ObjectSettings()
			{
				PagesCount = true,
				HeaderSettings  = { HtmUrl = headerPath}, 
				FooterSettings  = { HtmUrl = footerPath}, 
			
				/* FooterSettings = { Center = infosEnteprise.InvoiceFooter, Line = false, Spacing = 2, Spacing = 1.8, FontSize = 16, FontName = Helvetica, Line = false, Right = "Page [page] of [toPage]" }, */
				WebSettings = { DefaultEncoding = "utf-8", PrintMediaType=true},
				HtmlContent = html,

			};
			pdf.Objects.Add(page);

			var doc = _converter.Convert(pdf);
		
			
			return new FileDto("Facture-"+factureDto.Reference+"-"+GetTimestamp(new DateTime())+".pdf", doc);
		}


		public async Task<FileDto> GetDevisAsPdfAsync(DevisDto devisDto, InfosEntrepriseDto infosEnteprise) {

			var html = GetHtmlContentDevisAndEntreprise(devisDto, infosEnteprise);
			var globalSettings = new GlobalSettings
			{
				ColorMode = ColorMode.Color,
				Orientation = Orientation.Portrait,
				PaperSize = PaperKind.A4
			};

			var pdf = new HtmlToPdfDocument
				{
					GlobalSettings = globalSettings,
				};

				//INIT THE FOOTER HTML FILE FROM FOOTER BLANK BECAUSE IT S OVERRIDED EACH TIME
				string footerPath = _hostingEnvironment.WebRootPath + @"/templates/Devis/footer.html";
				string footerPathBlank = _hostingEnvironment.WebRootPath + @"/templates/Devis/footer-blank.html";
				string textInit = File.ReadAllText(footerPathBlank);
				File.WriteAllText(footerPath, textInit);
				//

				var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

				var file = this.findFile(uploads);
				var filePath = "";

				if (file != null){
					filePath = Path.Combine(uploads, file);
				}

			  	string cssArialPath = _hostingEnvironment.ContentRootPath + @"/wwwroot/arial-webfont/ARIALLGT_footer.woff";
				cssArialPath = cssArialPath.Replace("\\", "/");

				/* cssArialPath = cssArialPath.Replace(":/", "://"); */


				string factureRef = "";
				for (int i = 0; i < 5 - devisDto.Reference.ToString().Length; i++)
				{
					factureRef += "0";
				}
				factureRef += devisDto.Reference.ToString();


				//INIT THE HEADER HTML FILE FROM FOOTER BLAN°°°K BECAUSE IT S OVERRIDED EACH TIME
				string headerPath = _hostingEnvironment.WebRootPath + @"/templates/Devis/header.html";
				string headerPathBlank = _hostingEnvironment.WebRootPath + @"/templates/Devis/header-blank.html";
				string textHeaderInit = File.ReadAllText(headerPathBlank);
				File.WriteAllText(headerPath, textHeaderInit);
				//

				
				

				
				string textFooter = File.ReadAllText(footerPath);
				string textHeader = File.ReadAllText(headerPath);
			
				if(devisDto.PiedDePage != null && devisDto.PiedDePage.Length > 0 ){
					string	 invoiceFooter = devisDto.PiedDePage;
					if(devisDto.PiedDePage.Length > 140) {
						invoiceFooter = devisDto.PiedDePage.Substring(0,140);
					}
					textFooter = textFooter.Replace("@[INVOICE-FOOTER-MESSAGE]", invoiceFooter);
					textFooter = textFooter.Replace("@[CSS-ARIAL-PATH]", cssArialPath);
					File.WriteAllText(footerPath, textFooter);
				}else{
					if(infosEnteprise.EstimateFooter != null && infosEnteprise.EstimateFooter.Length > 0 ){
						string	 invoiceFooter = infosEnteprise.EstimateFooter;
						if(infosEnteprise.EstimateFooter.Length > 140) {
							invoiceFooter = infosEnteprise.EstimateFooter.Substring(0,140);
						}
						textFooter = textFooter.Replace("@[INVOICE-FOOTER-MESSAGE]", invoiceFooter);
						textFooter = textFooter.Replace("@[CSS-ARIAL-PATH]", cssArialPath);
						File.WriteAllText(footerPath, textFooter);
					}
				}
				
				

				textHeader = textHeader.Replace("@[REF-FACTURE]", factureRef);
				textHeader = textHeader.Replace("@[IMAGE-LOGO]", filePath);
				textHeader = textHeader.Replace("@[DATE-FACTURE]", devisDto.DateEmission.ToString("dd/MM/yyyy"));
				textHeader = textHeader.Replace("@[ECHEANCE-PAIEMENT]", devisDto.DateEmission.AddDays(devisDto.EcheancePaiement).ToString("dd/MM/yyyy"));
				//textHeader = text.Replace("@[IMAGE-LOGO]", filePath);
				File.WriteAllText(headerPath, textHeader);
				
			

			var page = new ObjectSettings()
			{
				PagesCount = true,
				HeaderSettings  = { HtmUrl = headerPath}, 
				FooterSettings  = { HtmUrl = footerPath}, 
			
				/* FooterSettings = { Center = infosEnteprise.InvoiceFooter, Line = false, Spacing = 2, Spacing = 1.8, FontSize = 16, FontName = Helvetica, Line = false, Right = "Page [page] of [toPage]" }, */
				WebSettings = { DefaultEncoding = "utf-8", PrintMediaType=true},
				HtmlContent = html,

			};
			pdf.Objects.Add(page);

			var doc = _converter.Convert(pdf);
		
			
			return new FileDto("Devis-"+devisDto.Reference+"-"+GetTimestamp(new DateTime())+".pdf", doc);
		}


		private string findFile(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath);
            var result = files.Where(x => x.Contains("id_"+ AbpSession.UserId + ".")).FirstOrDefault();
            return result;
        }


		private string GetHtmlContentFacture(FactureDto facture)
		{
			var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

            var file = this.findFile(uploads);
			var filePath = "";

            if (file != null){
            	 filePath = Path.Combine(uploads, file);
			}

			
			Console.WriteLine("filePath");
			Console.WriteLine(filePath);
			var sb = new StringBuilder();

			  string cssPath = @"file:///" + _hostingEnvironment.ContentRootPath + @"\wwwroot\arial-webfont\style.css";
			sb.Append(@"<!doctype html>
			<html lang='fr'>
			<head>
			<meta charset='utf-8'>
			<title>Report</title>
			<link rel='stylesheet' href='");
			sb.Append("C:/ProjectsDev/evidence/api/src/Facturi.Web.Host/wwwroot/arial-webfont/style.css"); 
			sb.Append(@"'>
				<style type='text/css'>
			html, body {
			margin: 0px;
			padding: 0px;
			font-family: Arial, sans-serif !important;
			}
			table thead tr th, table tr td, p, h1,h2,h3,h4,h5,h6, ul, li, span, table, div {
			font-family: Arial, sans-serif !important;
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
				}

				.headerFacture .divImg {
					left: 0px;
					min-width:50%;

				}
					.headerFacture .divImg img {
						height:100px;
						width:auto;
						max-width:300px;
					}

				.headerFacture .divInfosFacture {
					min-width:30%;
					right: 0px;
				}
					.headerFacture .divInfosFacture p {
						text-align: left;
						margin: 2px;
						font-weight: 400;
						font-size: 24px;
					}
					.headerFacture .divInfosFacture .pDate {
						font-size: 24px;
						color: #898989;
						font-weight:400;
					}
					.headerFacture .divInfosFacture .pDateEmission {
						margin-top:10px;
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
					width: 45%;
				}

				.cordonneesFacture .divPour .divPourLabel {
						left: 0px;
					
				}
				.pPourDe {font-size: 26px;line-height:37px !important;}

				.cordonneesFacture .divPour .divPourContent {
						left: 78px;
				}
				.cordonneesFacture .divPour .divPourContent p {font-size:20px;}
				.raison-sociale{line-height:37px !important;}
				.adresse, .ville, .pays, .numTel{font-weight: 300 !important;letter-spacing:1px; text-transform:lowercase;}
				.adresse::first-letter, .ville::first-letter, .pays::first-letter{text-transform:capitalize;}
				.cordonneesFacture .divDe {
					right: 0px;
					width: 40%;
				}

					.cordonneesFacture div p {
						margin: 2px;
					}
					.cordonneesFacture div .pPourDe {
						color: #898989;
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
						sb.Append(filePath); 
						//sb.Append(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot","uploads", "id_"+ Abp));
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
					<p class='pDate pDateEmission'>Date d’émission &nbsp;: ");
						sb.Append(facture.DateEmission.ToString("dd/MM/yyyy"));
						sb.Append(@"</p>
					<p class='pDate'>Date d’échéance : ");
						sb.Append(facture.DateEmission.AddDays(facture.EcheancePaiement).ToString("dd/MM/yyyy"));
						sb.Append(@"</p>
				</div>
			</div>
			<div class='cordonneesFacture'>
				<div class='divPour'>
					<div class='divPourLabel'>
						<p class='pPourDe'>Pour :</p>
					</div>
				<div class='divPourContent'>
						<p class='raison-sociale'>");
							sb.Append(facture.Client.CategorieClient.Equals("PRTC") ? facture.Client.Nom : facture.Client.RaisonSociale);
							sb.Append(@"</p>
						<p class='adresse'>");
							sb.Append(facture.Client.Adresse);
							sb.Append(@"</p>
						<p class='ville'>");
							sb.Append(facture.Client.Ville);
							sb.Append(@" ");
							sb.Append(facture.Client.CodePostal);
							sb.Append(@"</p>
						<p class='pays'>");
							sb.Append(facture.Client.Pays);
							sb.Append(@"</p>
						<p class='numTel'>");
							sb.Append(facture.Client.TelPortable);
							sb.Append(@"</p>
					</div>
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
							<th class='left' style='border-bottom: 2px solid #c9c9c9;border-top: 2px solid #c9c9c9;'><br>Designation<br><br></th>
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
									<td class='left designation'>");
							sb.Append(fi.Designation);
							sb.Append(@"</td>
									<td class='date'>");
							sb.Append(fi.Date.ToString("dd/MM/yyyy"));
							sb.Append(@"</td>
									<td class='quantity'>");
							sb.Append(fi.Quantity);
							sb.Append(@"</td>
									<td class='unit'>");
							sb.Append(fi.Unit);
							sb.Append(@"</td>
									<td class='pu-ht'>");
							sb.Append(fi.UnitPriceHT);
							sb.Append(@" MAD</td>
									<td class='tva'>");
							sb.Append(fi.Tva);
							sb.Append(@"%</td>
									<td class='right pu-ttc'>");
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
						sb.Append(totalMontantHT + totalMontantTVA - (facture.Remise * totalMontantHT /100));
						sb.Append(@" MAD</p>
								</div>
						</div>
					</div>
			</div>
			</body>
			</html>");

			return sb.ToString();
		}



		private static String ones(String Number)  
		{  
			int _Number = Convert.ToInt32(Number);  
			String name = "";  
			switch (_Number)  
			{  
		
				case 1:  
					name = "Un";  
					break;  
				case 2:  
					name = "Deux";  
					break;  
				case 3:  
					name = "Trois";  
					break;  
				case 4:  
					name = "Quatre";  
					break;  
				case 5:  
					name = "Cinq";  
					break;  
				case 6:  
					name = "Six";  
					break;  
				case 7:  
					name = "Sept";  
					break;  
				case 8:  
					name = "Huit";  
					break;  
				case 9:  
					name = "Neuf";  
					break;  
			}  
			return name;  
		}

		private static String tens(String Number)  
		{  
			int _Number = Convert.ToInt32(Number);  
			String name = null;  
			switch (_Number)  
			{  
				case 10:  
					name = "Dix";  
					break;  
				case 11:  
					name = "Onze";  
					break;  
				case 12:  
					name = "Douze";  
					break;  
				case 13:  
					name = "Treize";  
					break;  
				case 14:  
					name = "Quatorze";  
					break;  
				case 15:  
					name = "Quinze";  
					break;  
				case 16:  
					name = "Seize";  
					break;  
				case 17:  
					name = "Dix-Sept";  
					break;  
				case 18:  
					name = "Dix-Huit";  
					break;  
				case 19:  
					name = "Dix-Neuf";  
					break;  
				case 20:  
					name = "Vingt";  
					break;  
				case 30:  
					name = "Trente";  
					break;  
				case 40:  
					name = "Quarante";  
					break;  
				case 50:  
					name = "Cinquante";  
					break;  
				case 60:  
					name = "Soixante";  
					break;  
				case 70:  
					name = "Soixante Dix";  
					break;  
					case 71:  
					name = "Soixante et Onze";  
					break;
					case 72:  
					name = "Soixante Douze";  
					break;
					case 73:  
					name = "Soixante Treize";  
					break;
					case 74:  
					name = "Soixante Quatorze";  
					break;
					case 75:  
					name = "Soixante Quinze";  
					break;
					case 76:  
					name = "Soixante Seize";  
					break;
					case 77:  
					name = "Soixante Dix-sept";  
					break;
					case 78:  
					name = "Soixante Dix-huit";  
					break;
					case 79:  
					name = "Soixante Dix-neuf";  
					break;
				case 80:  
					name = "Quatre Vingt";  
					break;  
				case 90:  
					name = "Quatre Vingt Dix";  
					break; 
					case 91:  
					name = "Quatre Vingt et Onze";  
					break;
					case 92:  
					name = "Quatre Vingt Douze";  
					break;
					case 93:  
					name = "Quatre Vingt Treize";  
					break;
					case 94:  
					name = "Quatre Vingt Quatorze";  
					break;
					case 95:  
					name = "Quatre Vingt Quinze";  
					break;
					case 96:  
					name = "Quatre Vingt Seize";  
					break;
					case 97:  
					name = "Quatre Vingt Dix-sept";  
					break;
					case 98:  
					name = "Quatre Vingt Dix-huit";  
					break;
					case 99:  
					name = "Quatre Vingt Dix-neuf";  
					break;
					
				default:  
					if (_Number > 0)  
					{  
						name = tens(Number.Substring(0, 1) + "0") + " " + ones(Number.Substring(1));  
					}  
					break;  
			}  
			return name;  
		}
		

		private static String ConvertWholeNumber(String Number)  
		{  
			string word = "";  
			try  
			{  
				bool beginsZero = false;//tests for 0XX    
				bool isDone = false;//test if already translated    
				double dblAmt = (Convert.ToDouble(Number));  
				//if ((dblAmt > 0) && number.StartsWith("0"))    
				if (dblAmt > 0)  
				{//test for zero or digit zero in a nuemric    
					beginsZero = Number.StartsWith("0");  
		
					int numDigits = Number.Length;  
					int pos = 0;//store digit grouping    
					String place = "";//digit grouping name:hundres,thousand,etc...    
					switch (numDigits)  
					{  
						case 1://ones' range    
		
							word = ones(Number);  
							isDone = true;  
							break;  
						case 2://tens' range    
							word = tens(Number);  
							isDone = true;  
							break;  
						case 3://hundreds' range    
							pos = (numDigits % 3) + 1;  
							place = " Cent ";  
							break;  
						case 4://thousands' range    
						case 5:  
						case 6:  
							pos = (numDigits % 4) + 1;  
							place = " Mille ";  
							break;  
						case 7://millions' range    
						case 8:  
						case 9:  
							pos = (numDigits % 7) + 1;  
							place = " Million ";  
							break;  
						case 10://Billions's range    
						case 11:  
						case 12:  
		
							pos = (numDigits % 10) + 1;  
							place = " Billion ";  
							break;  
						//add extra case options for anything above Billion...    
						default:  
							isDone = true;  
							break;  
					}  
					if (!isDone)  
					{//if transalation is not done, continue...(Recursion comes in now!!)    
						if (Number.Substring(0, pos) != "0" && Number.Substring(pos) != "0")  
						{  
							try  
							{  
								word = ConvertWholeNumber(Number.Substring(0, pos)) + place + ConvertWholeNumber(Number.Substring(pos));  
							}  
							catch { }  
						}  
						else  
						{  
							word = ConvertWholeNumber(Number.Substring(0, pos)) + ConvertWholeNumber(Number.Substring(pos));  
						}  
		
						//check for trailing zeros    
						//if (beginsZero) word = " and " + word.Trim();    
					}  
					//ignore digit grouping names    
					if (word.Trim().Equals(place.Trim())) word = "";  
				}  
			}  
			catch { }  
			return word.Trim();  
		}


		private static String ConvertToWords(String numb)  
		{  
			String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";  
			String endStr = "";  
			try  
			{  
				int decimalPlace = numb.IndexOf(".");  
				if (decimalPlace > 0)  
				{  
					wholeNo = numb.Substring(0, decimalPlace);  
					points = numb.Substring(decimalPlace + 1);  
					if (Convert.ToInt32(points) > 0)  
					{  
						andStr = "and";// just to separate whole numbers from points/cents    
						endStr = "Paisa " + endStr;//Cents    
						pointStr = ConvertDecimals(points);  
					}  
				}  
				val = String.Format("{0} {1}{2} {3}", ConvertWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);  
			}  
			catch { }  
			return val;  
		}


		private static String ConvertDecimals(String number)  
		{  
			String cd = "", digit = "", engOne = "";  
			for (int i = 0; i < number.Length; i++)  
			{  
				digit = number[i].ToString();  
				if (digit.Equals("0"))  
				{  
					engOne = "Zero";  
				}  
				else  
				{  
					engOne = ones(digit);  
				}  
				cd += " " + engOne;  
			}  
			return cd;  
		}

		public static String GetTimestamp(DateTime value)
		{
			return value.ToString("yyMMddHHmmss");
		}

	}
}
