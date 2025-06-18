// GetPageSeoDataQueryHandler.cs - DÜZELTME
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Features.SEO.Queries.GetPageSeoData
{
    public class GetPageSeoDataQueryHandler : IRequestHandler<GetPageSeoDataQuery, SeoMetaData?>
    {
        private readonly ISeoService _seoService;

        public GetPageSeoDataQueryHandler(ISeoService seoService)
        {
            _seoService = seoService;
        }

        public async Task<SeoMetaData?> Handle(GetPageSeoDataQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _seoService.GetPageSeoDataAsync(request.Path);
            }
            catch
            {
                return null;
            }
        }
    }
}