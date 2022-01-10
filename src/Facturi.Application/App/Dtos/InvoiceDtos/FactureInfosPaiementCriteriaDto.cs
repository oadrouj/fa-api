using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos.InvoiceDtos
{
    public class FactureInfosPaiementCriteriaDto
    {
        public long FactureId { get; set; }
        public int First { get; set; }
        public int Rows { get; set; }
    }
}
