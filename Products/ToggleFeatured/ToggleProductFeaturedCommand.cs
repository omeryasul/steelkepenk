using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.ToggleFeatured
{
    public record ToggleProductFeaturedCommand(int Id) : IRequest<Result<bool>>;

}
