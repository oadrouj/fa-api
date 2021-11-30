using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Facturi.App
{
    public class DevisItem : Entity<long>
    {
        public string Designation { get; set; }

        public DateTime Date { get; set; } = new DateTime();

        public int Quantity { get; set; }

        public string Unit { get; set; }

        public float UnitPriceHT { get; set; }

        public float Tva { get; set; }

        public float TotalTtc { get; set; }

        [ForeignKey("DevisId")]
        public long DevisId {get; set;}
        public Devis Devis { get; set; }
    }
}