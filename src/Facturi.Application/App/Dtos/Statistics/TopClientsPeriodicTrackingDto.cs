using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos.Statistics
{
    public class TopClientsPeriodicTrackingDto
    {
        public string ClientName { get; set; }
        public float Amount { get; set; }
    }
}
