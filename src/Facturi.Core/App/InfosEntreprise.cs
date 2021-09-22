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

        public string Adresse { get; set; }

        public string CodePostal { get; set; }

        public string Ville { get; set; }

        public string Pays { get; set; }

        public string Telephone { get; set; }

        public string AdresseMail { get; set; }

        public string ICE { get; set; }

        public string IF { get; set; }

        public string TP { get; set; }

        [ForeignKey("UserId")]
        public long UserId { get; set; }
        public User User { get; set; }
    }
}
