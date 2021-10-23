using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Facturi.App
{
    public class FactureInfosPaiement : Entity<long>
    {
        public DateTime DatePaiement { get; set; } = new DateTime();

        public float MontantPaye { get; set; }

        public ModePaiementEnum ModePaiement { get; set; }


        [ForeignKey("FactureId")]
        public long FactureId { get; set;}
        public Facture Facture { get; set; }
    }
}