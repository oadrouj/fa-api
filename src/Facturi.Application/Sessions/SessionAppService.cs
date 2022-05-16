using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Domain.Repositories;
using Facturi.App;
using Facturi.Sessions.Dto;
using Facturi.Sessions.Mappers;
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
            InfosEntreprise infosEntreprise=null;
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>()
                },
            };

           
          if(AbpSession.UserId.HasValue){
                if(!(await UserManager.FindByIdAsync(AbpSession.UserId.ToString())).IsActive)
                {
                    return null;
                }
                else
                {
                    output.User = UserMapper.MapToEntityDto(await GetCurrentUserAsync());

                    int checkIfIsNull = _infosEntrepriseRepo.Count(e => e.UserId == AbpSession.UserId);

                    if (checkIfIsNull != 0)
                    {
                        infosEntreprise = _infosEntrepriseRepo.FirstOrDefault(e => e.UserId == AbpSession.UserId);
                        output.EntrepriseName = infosEntreprise.RaisonSociale;

                    }
                }

               
            }
            
            return output;
        }
    }
}
