using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos.Statistics
{
    public class PeriodicTrackingInputDto
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
    }
}
