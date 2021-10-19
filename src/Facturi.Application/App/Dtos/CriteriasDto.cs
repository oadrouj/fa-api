using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos
{
    public class CriteriasDto
    {
        public int First { get; set; }
        public int Rows { get; set; }
        public string GlobalFilter { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        public Filtre[] Filtres { get; set; }
    }
}
