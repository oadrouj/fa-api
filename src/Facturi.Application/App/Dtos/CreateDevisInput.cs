using Abp.AutoMapper;
using System;
using System.Collections.Generic;

namespace Facturi.App
{
    [AutoMap(typeof(Devis))]
    public class CreateDevisInput
    {
        public string Reference { get; set; }
        
        public DateTime DateEmission { get; set; } = new DateTime();
        public int EcheancePaiement { get; set; }
        public string MessageIntroduction { get; set; }
        public string PiedDePage { get; set; }
        public float Remise { get; set; }
        public string Currency { get; set; }

        public DevisStatutEnum Statut { get; set; }

        public List<DevisItemDto> DevisItems { get; set; }

        public long ClientId { get; set; }
    }
}