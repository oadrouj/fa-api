using System;
namespace Facturi.App.Dtos
{
    public class Filter
    {
        public long Client { get; set; }
        public DateTime[] DateEmission { get; set; }
        public int EcheancePaiement { get; set; }
        public float MontantTtc { get; set; } = -1;
        public int Statut { get; set; } = -1;
    }
}