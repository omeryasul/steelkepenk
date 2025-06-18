using Application.Features.ContactMessages.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Queries.GetById
{
    public record GetContactMessageByIdQuery(int Id) : IRequest<ContactMessageDetailDto?>;

}
