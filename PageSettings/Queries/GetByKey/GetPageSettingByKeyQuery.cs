using Application.Common.Models;
using Application.Features.PageSettings.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.PageSettings.Queries.GetByKey
{
    public class GetPageSettingByKeyQuery : IRequest<Result<PageSettingDto>>
    {
        public string Key { get; set; }

        public GetPageSettingByKeyQuery(string key)
        {
            Key = key;
        }
    }
}
