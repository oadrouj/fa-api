using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Facturi.Core.App;

namespace Facturi.Application.App.Dtos
{
    [AutoMap(typeof(Country))]
    public class CountryDto: EntityDto<long>
    {
           public string PaysName { get; set; }
    }
       
}