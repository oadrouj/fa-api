using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Facturi.Core.App;

namespace Facturi.Application.App.Dtos.CatalogueDtos
{
    [AutoMap(typeof(Catalogue))]
    public class CatalogueForAutoCompleteDto: EntityDto
    {
        public string Designation { get; set; }
        public DateTime AddedDate { get; set; }
        public float HtPrice { get; set; }
        public string Unity { get; set; }
        public int Tva { get; set; }
        public float MinimalQuantity { get; set; }
    }
}