using Application.Common.Models;
using Application.Features.Contents.DTOs;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.Queries.GetAll
{
    public class GetContentsQuery : IRequest<Result<PagedResult<ContentListDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public ContentStatus? Status { get; set; }
        public ContentType? Type { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsFeatured { get; set; }
        public string? SortBy { get; set; } = "CreatedDate";
        public string? SortDirection { get; set; } = "desc";
    }
}
