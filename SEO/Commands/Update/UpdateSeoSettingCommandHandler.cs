using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.Commands.Update
{
    public class UpdateSeoSettingCommandHandler : IRequestHandler<UpdateSeoSettingCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly ISeoService _seoService;

        public UpdateSeoSettingCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            ISeoService seoService)
        {
            _context = context;
            _currentUser = currentUser;
            _seoService = seoService;
        }

        public async Task<Result<bool>> Handle(UpdateSeoSettingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.SeoSettings
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    return Result<bool>.Failure("SEO ayarı bulunamadı.");

                entity.SiteName = request.SiteName;
                entity.SiteDescription = request.SiteDescription;
                entity.SiteKeywords = request.SiteKeywords;
                entity.SiteUrl = request.SiteUrl;
                entity.GoogleAnalyticsId = request.GoogleAnalyticsId;
                entity.GoogleTagManagerId = request.GoogleTagManagerId;
                entity.FacebookPixelId = request.FacebookPixelId;
                entity.GoogleSearchConsoleCode = request.GoogleSearchConsoleCode;
                entity.RobotsText = request.RobotsText;
                entity.EnableSitemap = request.EnableSitemap;
                entity.DefaultOgImage = request.DefaultOgImage;
                entity.OrganizationJsonLd = request.OrganizationJsonLd;
                entity.WebsiteJsonLd = request.WebsiteJsonLd;
                entity.UpdatedDate = DateTime.UtcNow;
                entity.UpdatedBy = _currentUser.UserEmail;

                await _context.SaveChangesAsync(cancellationToken);

                // Sitemap cache'ini temizle
                await _seoService.InvalidateSitemapCacheAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"SEO ayarı güncellenirken hata oluştu: {ex.Message}");
            }
        }
    }
}
