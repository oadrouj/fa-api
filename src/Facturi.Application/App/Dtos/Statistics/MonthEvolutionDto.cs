using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos.Statistics
{
    public class MonthEvolutionDto
    {
        public float TotalInvoicedAmountEvolved { get; set; }
        public float TotaEstimatedAmountEvolved { get; set; }
        public int TotalClientsEvolved { get; set; }
        public float TotalCatalogsEvolved { get; set; }
    }
}
