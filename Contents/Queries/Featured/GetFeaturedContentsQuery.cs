using Application.Common.Models;
using Application.Features.Contents.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.Queries.Featured
{
    public class GetFeaturedContentsQuery : IRequest<Result<List<ContentListDto>>>
    {
        public int Take { get; set; } = 5;
        public ContentType? Type { get; set; }
        public int? CategoryId { get; set; }
    }
}
