using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos.Statistics
{
    public class AnnualInvoicesTrackingDto
    {
        public float[] CashedInvoicesSerie { get; set; }
        public float[] LateInvoicesSerie { get; set; }
        public float[] WaitingInvoicesSerie { get; set; }

    }
}
