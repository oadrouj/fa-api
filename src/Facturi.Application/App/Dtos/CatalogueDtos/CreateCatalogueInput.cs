using System;
using Abp.AutoMapper;
using Facturi.Core.App;

namespace Facturi.Application.App.Dtos.CatalogueDtos
{
    [AutoMap(typeof(Catalogue))]
    public class CreateCatalogueInput
    {
        public string CatalogueType { get; set; }
        public string Designation { get; set; }
        public string Description { get; set; }
        public float HtPrice { get; set; }
        public string Unity { get; set; }
        public int Tva { get; set; }
        public float MinimalQuantity { get; set; }

    }
}