using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Facturi.Authorization.Roles;
using Facturi.Authorization.Users;
using Facturi.MultiTenancy;
using Facturi.App;

namespace Facturi.EntityFrameworkCore
{
    public class FacturiDbContext : AbpZeroDbContext<Tenant, Role, User, FacturiDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public DbSet<InfosEntreprise> InfosEntreprises { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Devis> Devis { get; set; }
        public DbSet<DevisItem> DevisItems { get; set; }
        public DbSet<Facture> Facture { get; set; }
        public DbSet<FactureItem> FactureItems { get; set; }

        public FacturiDbContext(DbContextOptions<FacturiDbContext> options)
            : base(options)
        {
        }
    }
}
