using Application.Features.ContactMessages.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Queries.GetStats
{
    public record GetContactMessageStatsQuery : IRequest<ContactMessageStatsDto>;

}
