using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Facturi.App
{
    public class FactureItem : Entity<long>
    {
        public string Designation { get; set; }

        public DateTime Date { get; set; } = new DateTime();

        public int Quantity { get; set; }

        public string Unit { get; set; }

        public float UnitPriceHT { get; set; }

        public float Tva { get; set; }

        public float TotalTtc { get; set; }

        [ForeignKey("FactureId")]
        public long FactureId { get; set;}
        public Facture Facture { get; set; }
        public long CatalogueId { get; set; }

    }
}