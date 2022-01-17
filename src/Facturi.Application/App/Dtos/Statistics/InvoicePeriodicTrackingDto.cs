using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos.Statistics
{
    public class InvoicePeriodicTrackingDto
    {
        public float TotalInvoicesAmount { get; set; }
        public float CashedInvoicesAmount { get; set; }

        public float PendingInvoicesAmount { get; set; }

        public float LateInvoicesAmount { get; set; }

}
}
