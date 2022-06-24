using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facturi.App.Dtos;
namespace Facturi.App.Dtos.ProfileDtos
{
    [AutoMap(typeof(InfosEntreprise))]
    public class TvaCurrencyDto: EntityDto<long>
    {
        public string? Currency { get; set; }
        public string? Tva { get; set; }
    }
}
