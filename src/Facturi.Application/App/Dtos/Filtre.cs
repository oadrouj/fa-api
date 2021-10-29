using System;
namespace Facturi.App.Dtos
{
    public class Filter
    {
        public long Client { get; set; }
        public DateTime[] DateEmission { get; set; }
        public int EcheancePaiement { get; set; }
        public double MontantTtc { get; set; }
        public FactureStatutEnum Statut { get; set; }
    }
}