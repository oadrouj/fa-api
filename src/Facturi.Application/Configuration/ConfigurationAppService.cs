using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Facturi.Configuration.Dto;

namespace Facturi.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : FacturiAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
