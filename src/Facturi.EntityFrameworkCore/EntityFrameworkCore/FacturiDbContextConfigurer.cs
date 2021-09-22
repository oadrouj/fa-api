using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Facturi.EntityFrameworkCore
{
    public static class FacturiDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<FacturiDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<FacturiDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
