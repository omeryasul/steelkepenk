using Application.Features.Products.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.Queries.GetBySlug
{
    public record GetFeaturedProductsQuery(int Take = 6) : IRequest<List<ProductListDto>>;

}
