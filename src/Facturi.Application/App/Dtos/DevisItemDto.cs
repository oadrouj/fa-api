using Abp.AutoMapper;
using System;

namespace Facturi.App
{
    [AutoMap(typeof(DevisItem))]
    public class DevisItemDto
    {
        public string Designation { get; set; }

        public DateTime Date { get; set; } = new DateTime();

        public int Quantity { get; set; }

        public string Unit { get; set; }

        public float UnitPriceHT { get; set; }

        public float Tva { get; set; }

        public float TotalTtc { get; set; }
        public long? CatalogueId { get; set; }
    }
}