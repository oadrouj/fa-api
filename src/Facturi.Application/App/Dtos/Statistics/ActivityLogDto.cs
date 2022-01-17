using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos.Statistics
{
    public class ActivityLogDto
    {
        public string LogType { get; set; }
        public string Reference { get; set; }
        public string ClientName { get; set; }
        public DateTime Date { get; set; }
        public float Amount { get; set; }
        public string Currency { get; set; }

    }
}
