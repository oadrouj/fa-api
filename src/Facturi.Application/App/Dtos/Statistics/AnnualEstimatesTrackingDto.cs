using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos.Statistics
{
    public  class AnnualEstimatesTrackingDto
    {
        public float[] ValidatedEstimatesSerie { get; set; }
        public float[] TransformedEstimatesSerie { get; set; }
    }

}
