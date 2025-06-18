using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.SEO.Commands.RegenerateSitemap;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.Commands.Update
{
    public class RegenerateSitemapCommandHandler : IRequestHandler<RegenerateSitemapCommand, Result<string>>
    {
        private readonly ISeoService _seoService;

        public RegenerateSitemapCommandHandler(ISeoService seoService)
        {
            _seoService = seoService;
        }

        public async Task<Result<string>> Handle(RegenerateSitemapCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _seoService.InvalidateSitemapCacheAsync();
                var sitemap = await _seoService.GenerateSitemapAsync();

                return Result<string>.Success(sitemap);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Sitemap oluşturulurken hata oluştu: {ex.Message}");
            }
        }
    }
}
