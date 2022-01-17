using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos.Statistics
{
    public class PeriodicTrackingDto
    {
        public InvoicePeriodicTrackingDto InvoicePeriodicTrackingDto { get; set; }
        public EstimatePeriodicTrackingDto EstimatePeriodicTrackingDto { get; set; }
        public List<BestsellerPeriodicTrackingDto> BestsellerPeriodicTrackingDto { get; set; }
        public List<TopClientsPeriodicTrackingDto> TopClientsPeriodicTrackingDto { get; set; }
    }
}
