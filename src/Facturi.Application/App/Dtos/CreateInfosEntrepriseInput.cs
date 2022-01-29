using Abp.AutoMapper;

namespace Facturi.App.Dtos
{
    [AutoMap(typeof(InfosEntreprise))]
    public class CreateInfosEntrepriseInput
    {
        public string RaisonSociale { get; set; }
        public string SecteurActivite { get; set; }
        //public string StatutJuridique { get; set; }
        //public string Adresse { get; set; }
        //public string CodePostal { get; set; }
        //public string Ville { get; set; }
        //public string Pays { get; set; }
        //public string Telephone { get; set; }
        //public string AdresseMail { get; set; }
    }
}
