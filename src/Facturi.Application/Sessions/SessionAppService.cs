using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Domain.Repositories;
using Facturi.App;
using Facturi.Sessions.Dto;

namespace Facturi.Sessions
{
    public class SessionAppService : FacturiAppServiceBase, ISessionAppService
    {
        private readonly IRepository<InfosEntreprise, long> _infosEntrepriseRepo;
        public SessionAppService(
            IRepository<InfosEntreprise, long> infosEntrepriseRepo
        )
        {
            _infosEntrepriseRepo = infosEntrepriseRepo;
        }
        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>()
                },
            };

           
            if(AbpSession.UserId.HasValue && !(await UserManager.FindByIdAsync(AbpSession.UserId.ToString())).IsActive)
            {
                return null;
            }

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
            }

            if (AbpSession.UserId.HasValue)
            {
                output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
            }

            InfosEntreprise infosEntreprise;
            if ((infosEntreprise = (await _infosEntrepriseRepo.FirstOrDefaultAsync(e => e.UserId == AbpSession.UserId))) != null )
            {
                output.EntrepriseName = infosEntreprise.RaisonSociale;

            }
            return output;
        }
    }
}
