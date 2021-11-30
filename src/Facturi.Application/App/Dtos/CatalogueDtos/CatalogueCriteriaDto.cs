namespace Facturi.Application.App.Dtos.CatalogueDtos
{
    public class CatalogueCriteriaDto
    {
        public int First { get; set; }
        public int Rows { get; set; }
        public string GlobalFilter { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        public CatalogueFilter Filtres { get; set; }
    }
}