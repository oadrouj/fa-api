using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;

namespace Facturi.App
{
    [AutoMap(typeof(FactureItem))]
    public class FactureItemDto
    {
        public string Designation { get; set; }

        public DateTime Date { get; set; } 
        public int Quantity { get; set; }

        public string Unit { get; set; }

        public float UnitPriceHT { get; set; }

        public float Tva { get; set; }

        public float TotalTtc { get; set; }
        public long? CatalogueId { get; set; }

    }
}