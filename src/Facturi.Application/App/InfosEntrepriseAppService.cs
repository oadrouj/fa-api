using Abp.Application.Services;
using Abp.Domain.Repositories;
using Facturi.App.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.App
{
    public class InfosEntrepriseAppService : ApplicationService, IInfosEntrepriseAppService
    {
        private readonly IRepository<InfosEntreprise, long> _infosEntrepriseRepository;

        public InfosEntrepriseAppService(IRepository<InfosEntreprise, long>  infosEntrepriseRepository)
        {
            _infosEntrepriseRepository = infosEntrepriseRepository ?? throw new ArgumentNullException(nameof(infosEntrepriseRepository));
        }

        public async Task CreateInfosEntreprise(CreateInfosEntrepriseInput input)
        {
            var infosEntreprise = ObjectMapper.Map<InfosEntreprise>(input);
            await _infosEntrepriseRepository.InsertAsync(infosEntreprise);
        }

        public async Task<InfosEntrepriseDto> GetByIdInfosEntreprise(long id)
        {
            var infosEntreprise = await _infosEntrepriseRepository.GetAsync(id);

            return ObjectMapper.Map<InfosEntrepriseDto>(infosEntreprise);
        }
    }
}
