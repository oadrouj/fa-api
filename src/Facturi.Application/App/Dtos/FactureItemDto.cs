using Abp.AutoMapper;
using System;

namespace Facturi.App
{
    [AutoMap(typeof(FactureItem))]
    public class FactureItemDto
    {
        public string Description { get; set; }

        public DateTime Date { get; set; } = new DateTime();

        public int Quantity { get; set; }

        public string Unit { get; set; }

        public float UnitPriceHT { get; set; }

        public float Tva { get; set; }

        public float TotalTtc { get; set; }
    }
}