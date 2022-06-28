using Abp.Application.Services.Dto;
using Facturi.Users.Dto;
using Abp.AutoMapper;
namespace Facturi.App.Dtos
{
    [AutoMap(typeof(InfosEntreprise))]
    public class InfosEntrepriseDto : AuditedEntityDto<long>
    {
        public string RaisonSociale { get; set; }
        public string SecteurActivite { get; set; }
        public string Adresse { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
        public string Pays { get; set; }
        public string Telephone { get; set; }
        public string AdresseMail { get; set; }
        public long UserId { get; set; }
        public UserDto User { get; set; }
        public string? Tva { get; set; }
        public string? Currency { get; set; }
        public string EstimateIntroMessage { get; set; }
        public string EstimateFooter { get; set; }
        public string InvoiceIntroMessage { get; set; }
        public string InvoiceFooter { get; set; }
    }
}
