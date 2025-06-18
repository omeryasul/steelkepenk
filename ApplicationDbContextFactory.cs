using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var apiProjectPath = FindWebApiProject();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);

            return new ApplicationDbContext(builder.Options);
        }

        private string FindWebApiProject()
        {
            var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());

            // Üst dizinlerde solution klasörünü bul
            while (currentDir != null && !currentDir.GetFiles("*.sln").Any())
            {
                currentDir = currentDir.Parent;
            }

            if (currentDir == null)
                throw new DirectoryNotFoundException("Solution bulunamadı");

            // Presentation/WEB.Api klasörünü bul
            var webApiPath = Path.Combine(currentDir.FullName, "Presentation", "WEB.Api");

            if (!Directory.Exists(webApiPath))
                throw new DirectoryNotFoundException($"WEB.Api projesi bulunamadı: {webApiPath}");

            return webApiPath;
        }
    }
}