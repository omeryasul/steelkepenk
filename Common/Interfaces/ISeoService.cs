// ISeoService.cs - DÜZELTME
using Application.Common.Models;

namespace Application.Common.Interfaces
{
    public interface ISeoService
    {
        Task<string> GenerateSitemapAsync();
        Task<string> GenerateRobotsTextAsync();
        Task<SeoMetaData> GetPageSeoDataAsync(string path);
        Task InvalidateSitemapCacheAsync();
        string GenerateStructuredData(object data, string type);
    }
}