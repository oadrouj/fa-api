using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos.Statistics
{
    public class TotalStatisticsDto
    {
        public float IssuedInvoices { get; set; }
        public float IssuedEstimates { get; set; }
        public int Clients { get; set; }
        public int Products { get; set; }

    }
}
