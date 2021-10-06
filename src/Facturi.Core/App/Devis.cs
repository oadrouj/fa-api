using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Facturi.App
{
    public class Devis : AuditedEntity<long>
    {
        public int Reference { get; set; }
        public DateTime DateEmission { get; set; } = new DateTime();
        public int EcheancePaiement { get; set; }
        public string MessageIntroduction { get; set; }
        public string PiedDePage { get; set; }
        public float Remise { get; set; }

        public DevisStatutEnum Statut { get; set; }

        public ICollection<DevisItem> DevisItems { get; set; }

        [ForeignKey("ClientId")]
        public long ClientId { get; set; }
        public Client Client { get; set; }
    }
}
