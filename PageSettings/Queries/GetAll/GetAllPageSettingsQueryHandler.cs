using Application.Common.Interfaces;
using Application.Features.PageSettings.Dto;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.PageSettings.Queries.GetAll
{
    public class GetAllPageSettingsQueryHandler : IRequestHandler<GetAllPageSettingsQuery, List<PageSettingDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllPageSettingsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PageSettingDto>> Handle(GetAllPageSettingsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.PageSettings.AsNoTracking();

                if (!string.IsNullOrEmpty(request.Group))
                {
                    query = query.Where(x => x.Group == request.Group);
                }

                if (request.IsActive.HasValue)
                {
                    query = query.Where(x => x.IsActive == request.IsActive.Value);
                }

                var entities = await query
                    .OrderBy(x => x.Group)
                    .ThenBy(x => x.Key)
                    .ToListAsync(cancellationToken);

                return _mapper.Map<List<PageSettingDto>>(entities);
            }
            catch
            {
                return new List<PageSettingDto>();
            }
        }
    }
}
