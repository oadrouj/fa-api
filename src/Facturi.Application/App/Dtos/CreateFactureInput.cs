using Abp.AutoMapper;
using System;
using System.Collections.Generic;

namespace Facturi.App
{
    [AutoMap(typeof(Facture))]
    public class CreateFactureInput
    {
        public string Reference { get; set; }
        public DateTime DateEmission { get; set; } = new DateTime();
        public int EcheancePaiement { get; set; }
        public string MessageIntroduction { get; set; }
        public string PiedDePage { get; set; }
        public float Remise { get; set; }
        public string Currency { get; set; }
        public FactureStatutEnum Statut { get; set; }
        public List<FactureItemDto> FactureItems { get; set; }
        public long ClientId { get; set; }
    }
}