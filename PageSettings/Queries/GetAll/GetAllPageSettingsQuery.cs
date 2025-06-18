using Application.Features.PageSettings.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.PageSettings.Queries.GetAll
{
    public class GetAllPageSettingsQuery : IRequest<List<PageSettingDto>>
    {
        public string Group { get; set; }
        public bool? IsActive { get; set; }
    }
}
