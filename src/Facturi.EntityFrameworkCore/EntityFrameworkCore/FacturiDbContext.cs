using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Facturi.Authorization.Roles;
using Facturi.Authorization.Users;
using Facturi.MultiTenancy;
using Facturi.App;
using Facturi.Core.App;

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
        public DbSet<FactureInfosPaiement> FactureInfosPaiements { get; set; }
        public DbSet<Catalogue> Catalogues {get; set; }
        public DbSet<Country> Countries {get; set; }

        public FacturiDbContext(DbContextOptions<FacturiDbContext> options)
            : base(options)
        {
        }
    }
}
