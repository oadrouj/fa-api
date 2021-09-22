using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Facturi.Controllers
{
    public abstract class FacturiControllerBase: AbpController
    {
        protected FacturiControllerBase()
        {
            LocalizationSourceName = FacturiConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
