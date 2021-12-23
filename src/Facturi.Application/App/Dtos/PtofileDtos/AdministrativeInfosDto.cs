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
    public class AdministrativeInfosDto: EntityDto<long>
    {
        public string StatutJuridique { get; set; }
        public string ICE { get; set; }
        public string IF { get; set; }
        public string TP { get; set; }
    }
}
