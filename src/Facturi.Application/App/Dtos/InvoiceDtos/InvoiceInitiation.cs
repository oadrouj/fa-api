using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos.InvoiceDtos
{
    public class InvoiceInitiationDto
    {
        public int LastReference { get; set; }
        public string InvoiceIntroMessage { get; set; }
        public string InvoiceFooter { get; set; }
    }
}
