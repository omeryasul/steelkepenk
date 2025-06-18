using Application.Common.Models;
using Application.Features.Contents.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.Queries.GetById
{
    public class GetContentByIdQuery : IRequest<Result<ContentDetailDto>>
    {
        public int Id { get; set; }
        public bool IncludeNavigation { get; set; } = false;
        public bool IncreaseViewCount { get; set; } = false;

        public GetContentByIdQuery(int id, bool includeNavigation = false, bool increaseViewCount = false)
        {
            Id = id;
            IncludeNavigation = includeNavigation;
            IncreaseViewCount = increaseViewCount;
        }
    }
}
