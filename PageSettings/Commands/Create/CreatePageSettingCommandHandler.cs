using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.PageSettings.Commands.Create
{
    public class CreatePageSettingCommandHandler : IRequestHandler<CreatePageSettingCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public CreatePageSettingCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(CreatePageSettingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if key already exists
                var existingSetting = await _context.PageSettings
                    .FirstOrDefaultAsync(x => x.Key == request.Key, cancellationToken);

                if (existingSetting != null)
                {
                    return Result<int>.Failure("Bu anahtar zaten mevcut.");
                }

                var entity = new PageSetting
                {
                    Key = request.Key,
                    Value = request.Value,
                    Description = request.Description,
                    Group = request.Group,
                    IsActive = request.IsActive,
                    CreatedDate = DateTime.UtcNow
                };

                _context.PageSettings.Add(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<int>.Success(entity.Id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Ayar oluşturulurken hata oluştu: {ex.Message}");
            }
        }
    }
}
