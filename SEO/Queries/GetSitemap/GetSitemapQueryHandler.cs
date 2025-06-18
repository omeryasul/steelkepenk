using Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.Queries.GetSitemap
{
    public class GetSitemapQueryHandler : IRequestHandler<GetSitemapQuery, string>
    {
        private readonly ISeoService _seoService;

        public GetSitemapQueryHandler(ISeoService seoService)
        {
            _seoService = seoService;
        }

        public async Task<string> Handle(GetSitemapQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _seoService.GenerateSitemapAsync();
            }
            catch
            {
                return "<?xml version=\"1.0\" encoding=\"UTF-8\"?><urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"></urlset>";
            }
        }
    }
}
