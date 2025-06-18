using Application.Common.Models;
using Application.Features.Contents.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.Queries.GetBySlug
{
    public class GetContentBySlugQuery : IRequest<Result<ContentDetailDto>>
    {
        public string Slug { get; set; } = string.Empty;
        public bool IncludeNavigation { get; set; } = false;
        public bool IncreaseViewCount { get; set; } = false;
        public bool OnlyPublished { get; set; } = true;

        public GetContentBySlugQuery(string slug, bool includeNavigation = false, bool increaseViewCount = false, bool onlyPublished = true)
        {
            Slug = slug;
            IncludeNavigation = includeNavigation;
            IncreaseViewCount = increaseViewCount;
            OnlyPublished = onlyPublished;
        }
    }
}
