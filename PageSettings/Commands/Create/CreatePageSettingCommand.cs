using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.PageSettings.Commands.Create
{
    public class CreatePageSettingCommand : IRequest<Result<int>>
    {
        [Required]
        public string Key { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public string Group { get; set; } = "General";

        public bool IsActive { get; set; } = true;
    }
}
