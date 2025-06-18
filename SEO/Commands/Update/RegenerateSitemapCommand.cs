// RegenerateSitemapCommand.cs - DÜZELTME
using Application.Common.Models;
using MediatR;

namespace Application.Features.SEO.Commands.RegenerateSitemap
{
    public record RegenerateSitemapCommand() : IRequest<Result<string>>;
}