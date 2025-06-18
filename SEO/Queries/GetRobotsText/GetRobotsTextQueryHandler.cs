using Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.Queries.GetRobotsText
{
    public class GetRobotsTextQueryHandler : IRequestHandler<GetRobotsTextQuery, string>
    {
        private readonly ISeoService _seoService;

        public GetRobotsTextQueryHandler(ISeoService seoService)
        {
            _seoService = seoService;
        }

        public async Task<string> Handle(GetRobotsTextQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _seoService.GenerateRobotsTextAsync();
            }
            catch
            {
                return "User-agent: *\nDisallow:";
            }
        }
    }
}
