using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Facturi.Authorization;

namespace Facturi
{
    [DependsOn(
        typeof(FacturiCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class FacturiApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<FacturiAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(FacturiApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
