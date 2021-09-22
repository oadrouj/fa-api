using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Facturi.Configuration;
using Facturi.Web;

namespace Facturi.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class FacturiDbContextFactory : IDesignTimeDbContextFactory<FacturiDbContext>
    {
        public FacturiDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<FacturiDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            FacturiDbContextConfigurer.Configure(builder, configuration.GetConnectionString(FacturiConsts.ConnectionStringName));

            return new FacturiDbContext(builder.Options);
        }
    }
}
