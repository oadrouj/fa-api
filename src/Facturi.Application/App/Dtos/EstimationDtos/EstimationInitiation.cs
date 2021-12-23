using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos.EstimationDtos
{
    public class EstimationInitiationDto
    {
        public int LastReference { get; set; }
        public string EstimateIntroMessage { get; set; }
        public string EstimateFooter { get; set; }
    }
}
