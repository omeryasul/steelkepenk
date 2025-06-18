using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.Commands.Create
{
    public class CreateSeoSettingCommandHandler : IRequestHandler<CreateSeoSettingCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly ISeoService _seoService;

        public CreateSeoSettingCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            ISeoService seoService)
        {
            _context = context;
            _currentUser = currentUser;
            _seoService = seoService;
        }

        public async Task<Result<int>> Handle(CreateSeoSettingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = new SeoSetting
                {
                    SiteName = request.SiteName,
                    SiteDescription = request.SiteDescription,
                    SiteKeywords = request.SiteKeywords,
                    SiteUrl = request.SiteUrl,
                    GoogleAnalyticsId = request.GoogleAnalyticsId,
                    GoogleTagManagerId = request.GoogleTagManagerId,
                    FacebookPixelId = request.FacebookPixelId,
                    GoogleSearchConsoleCode = request.GoogleSearchConsoleCode,
                    RobotsText = request.RobotsText,
                    EnableSitemap = request.EnableSitemap,
                    DefaultOgImage = request.DefaultOgImage,
                    OrganizationJsonLd = request.OrganizationJsonLd,
                    WebsiteJsonLd = request.WebsiteJsonLd,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = _currentUser.UserEmail
                };

                _context.SeoSettings.Add(entity);
                await _context.SaveChangesAsync(cancellationToken);

                // Sitemap cache'ini temizle
                await _seoService.InvalidateSitemapCacheAsync();

                return Result<int>.Success(entity.Id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"SEO ayarı oluşturulurken hata oluştu: {ex.Message}");
            }
        }
    }
}
