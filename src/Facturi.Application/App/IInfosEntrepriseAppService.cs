using Abp.Application.Services;
using Facturi.App.Dtos;
using Facturi.App.Dtos.ProfileDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App
{
    public interface IInfosEntrepriseAppService : IApplicationService
    {
        Task<bool> CreateInfosEntreprise(CreateInfosEntrepriseInput input);
        Task<InfosEntrepriseDto> GetByIdInfosEntreprise(long id);
        Task<GeneralInfosDto> GetGeneralInfos();
        Task<bool> UpdateGeneralInfos(GeneralInfosDto generalInfosDto);
        Task<ContactInfosDto> GetContactInfos();
        Task<bool> UpdateContactInfos(ContactInfosDto contactInfosDto);
        Task<DefaultAnnotationsDto> GetDefaultAnnotations();
        Task<bool> UpdateDefaultAnnotations(DefaultAnnotationsDto defaultAnnotationsDto);

    }
}
