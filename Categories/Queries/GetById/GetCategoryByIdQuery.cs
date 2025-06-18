using Application.Features.Categories.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Categories.Queries.GetById
{
    public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDetailDto?>;

}
