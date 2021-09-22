using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Facturi.App.Dtos
{
    [AutoMap(typeof(Client))]
    public class UpdateClientInput : EntityDto<long>
    {
        public int Reference { get; set; }
        public string CategorieClient { get; set; }
        public string Nom { get; set; }
        public string SecteurActivite { get; set; }
        public string ICE { get; set; }
        public string RaisonSociale { get; set; }
        public string Adresse { get; set; }
        public string Ville { get; set; }
        public string Pays { get; set; }
        public string CodePostal { get; set; }
        public string Email { get; set; }
        public string TelFix { get; set; }
        public string TelPortable { get; set; }
        public string SiteWeb { get; set; }
        public string DeviseFacturation { get; set; }
        public float RemisePermanente { get; set; }
        public int DelaiPaiement { get; set; }
    }
}
