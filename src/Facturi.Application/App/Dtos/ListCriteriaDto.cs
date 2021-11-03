using Facturi.Application.App.Dtos.Client;
namespace Facturi.App.Dtos
{
    public class ListCriteriaDto
    {
        public string ChampsRecherche { get; set; }
        public string ClientCategory { get; set; }
        public string ClientType { get; set; }
        public string SortField { get; set; }
        public int SortOrder { get; set; }
        public ClientFilter ClientFilter { get; set; }
    }
}
