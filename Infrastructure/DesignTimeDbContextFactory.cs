using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public class InfraDbContextFactory(IConfiguration configuration) : IDesignTimeDbContextFactory<InfraDbContext>
    {
        private readonly IConfiguration _configuration = configuration;
        public InfraDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InfraDbContext>();

            var connectionString = _configuration["ConnectionStrings:DefaultConnection"];

            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(10, 6, 7)));

            return new InfraDbContext(optionsBuilder.Options);
        }
    }
}
