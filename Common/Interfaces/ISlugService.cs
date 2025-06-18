// ISlugService.cs - Application/Common/Interfaces
namespace Application.Common.Interfaces
{
    public interface ISlugService
    {
        Task<string> GenerateUniqueSlugAsync(string input, string entityType, int? excludeId = null);
        string GenerateSlug(string input);
        Task<string> EnsureUniqueSlugAsync(string slug, string entityType, int? excludeId = null);
        string EnsureUniqueSlug(string slug, string entityType, int? excludeId = null);
        bool IsValidSlug(string slug);
    }
}