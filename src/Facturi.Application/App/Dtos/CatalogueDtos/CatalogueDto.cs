using Facturi.Core.App;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;

namespace Facturi.Application.App.Dtos.CatalogueDtos
{
    [AutoMap(typeof(Catalogue), typeof(UpdateCatalogueInput))]
    public class CatalogueDto: AuditedEntityDto<long>
    {
        public int Reference { get; set; }
        
        public string CatalogueType { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public string Designation { get; set; }
        public string Description { get; set; }
        public float HtPrice { get; set; }
        public string Unity { get; set; }
        public int Tva { get; set; }
        public float MinimalQuantity { get; set; }
        public float TtcPrice { get; set; }
        public float TotalSalesTTC { get; set; }
        public float TotalUnitsSold { get; set; }
    }
}