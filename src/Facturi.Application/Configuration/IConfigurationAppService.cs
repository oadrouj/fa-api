using System.Threading.Tasks;
using Facturi.Configuration.Dto;

namespace Facturi.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
