using Abp.AutoMapper;
using System;

namespace Facturi.App
{
    [AutoMap(typeof(DevisItem))]
    public class DevisItemDto
    {
        public string Description { get; set; }

        public DateTime Date { get; set; } = new DateTime();

        public int Quantity { get; set; }

        public string Unit { get; set; }

        public int UnitPriceHT { get; set; }

        public float Tva { get; set; }

        public float TotalTtc { get; set; }
    }
}