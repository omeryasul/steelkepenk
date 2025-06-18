using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.Commands.Delete
{
    public class DeleteContentCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }

        public DeleteContentCommand(int id)
        {
            Id = id;
        }
    }
}
