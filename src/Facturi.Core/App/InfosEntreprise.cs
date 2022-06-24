using Abp.Domain.Entities.Auditing;
using Facturi.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace Facturi.App
{
    [Table("AppInfosEntreprise")]
    public class InfosEntreprise : AuditedEntity<long>
    {
        public string RaisonSociale { get; set; }
        public string SecteurActivite { get; set; }
        public string StatutJuridique { get; set; }
        public string Adresse { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
        public string Pays { get; set; }
        public string Telephone { get; set; }
        public string AdresseMail { get; set; }
        public string? Currency { get; set; }
        public string? Tva { get; set; }

        [ForeignKey("UserId")]
        public long UserId { get; set; }
        public User User { get; set; }
        public bool? HasLogo { get; set; } = false;
        public string EstimateIntroMessage { get; set; }
        public string EstimateFooter { get; set; }
        public string InvoiceIntroMessage { get; set; }
        public string InvoiceFooter { get; set; }
        public float MonthTargetAmount { get; set; } = 0;
    }
}
