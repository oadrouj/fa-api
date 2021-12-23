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
    public class ContactInfosDto: EntityDto<long>
    {
        public string Adresse { get; set; }
        public string Pays { get; set; }
        public string Ville{ get; set; }
        public string CodePostal { get; set; }
        public string Telephone { get; set; }
        public string AdresseMail { get; set; }
    }
}
