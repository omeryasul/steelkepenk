using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PageSettings.Commands.Update
{
    public class UpdatePageSettingCommandHandler : IRequestHandler<UpdatePageSettingCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public UpdatePageSettingCommandHandler(IApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Result<bool>> Handle(UpdatePageSettingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.PageSettings
                    .FirstOrDefaultAsync(x => x.Key == request.Key, cancellationToken);

                if (entity == null)
                {
                    entity = new Domain.Entities.PageSetting
                    {
                        Key = request.Key,
                        Value = request.Value,
                        Description = request.Description,
                        Group = request.Group ?? "General",
                        IsActive = request.IsActive ?? true,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = "AdminPanel",
                        UpdatedDate = DateTime.UtcNow
                    };

                    await _context.PageSettings.AddAsync(entity, cancellationToken);
                }
                else
                {
                    entity.Value = request.Value;
                    if (!string.IsNullOrEmpty(request.Description))
                        entity.Description = request.Description;
                    if (!string.IsNullOrEmpty(request.Group))
                        entity.Group = request.Group;
                    if (request.IsActive.HasValue)
                        entity.IsActive = request.IsActive.Value;

                    entity.UpdatedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync(cancellationToken);

                // ❗❗ BURASI ÖNEMLİ: CACHE’İ TEMİZLE
                string cacheKey = $"PageSetting_{request.Key}";
                _cache.Remove(cacheKey);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Ayar güncellenirken hata oluştu: {ex.Message}");
            }
        }
    }
}
