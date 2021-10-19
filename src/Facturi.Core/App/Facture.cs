using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Facturi.App
{
    public class Facture : AuditedEntity<long>
    {
        public int Reference { get; set; }
        public DateTime DateEmission { get; set; } = new DateTime();
        public int EcheancePaiement { get; set; }
        public string MessageIntroduction { get; set; }
        public string PiedDePage { get; set; }
        public float Remise { get; set; }

        public FactureStatutEnum Statut { get; set; }

        public ICollection<FactureItem> FactureItems { get; set; }

        [ForeignKey("ClientId")]
        public long ClientId { get; set; }
        public Client Client { get; set; }
    }
}
