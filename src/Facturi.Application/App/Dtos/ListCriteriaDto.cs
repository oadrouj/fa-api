using Facturi.Application.App.Dtos.Client;
namespace Facturi.App.Dtos
{
    public class ListCriteriaDto
    {
        public int First { get; set; }
        public int Rows { get; set; }
        public string GlobalFilter { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        public ClientFilter ClientFilter { get; set; }
    }
}
