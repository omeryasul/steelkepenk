﻿using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Categories.Commands.Delete
{
    public record DeleteCategoryCommand(int Id) : IRequest<Result<bool>>;

}
