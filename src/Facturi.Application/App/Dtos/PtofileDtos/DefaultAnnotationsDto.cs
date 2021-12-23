using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App.Dtos.ProfileDtos
{
    [AutoMap(typeof(InfosEntreprise))]
    public class DefaultAnnotationsDto: EntityDto<long>
    {
        public string EstimateIntroMessage { get; set; }
        public string EstimateFooter { get; set; }
        public string InvoiceIntroMessage { get; set; }
        public string InvoiceFooter { get; set; }

    }
}
