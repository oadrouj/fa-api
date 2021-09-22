using Abp.Application.Services;
using Facturi.App.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App
{
    public interface IInfosEntrepriseAppService : IApplicationService
    {
        Task CreateInfosEntreprise(CreateInfosEntrepriseInput input);

        Task<InfosEntrepriseDto> GetByIdInfosEntreprise(long id);
    }
}
