using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.PageSettings.Dto;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PageSettings.Queries.GetByKey
{
    public class GetPageSettingByKeyQueryHandler : IRequestHandler<GetPageSettingByKeyQuery, Result<PageSettingDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly ILogger<GetPageSettingByKeyQueryHandler> _logger;

        public GetPageSettingByKeyQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IMemoryCache cache,
            ILogger<GetPageSettingByKeyQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Result<PageSettingDto>> Handle(GetPageSettingByKeyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔍 Handler başlatıldı - Key: {Key}", request.Key);

                // DEBUG: CACHE'İ TAMAMEN BYPASS ET
                // string cacheKey = $"PageSetting_{request.Key}";
                // if (_cache.TryGetValue(cacheKey, out PageSettingDto cachedDto))
                // {
                //     return Result<PageSettingDto>.Success(cachedDto);
                // }

                _logger.LogInformation("🔍 Veritabanı sorgusu başlatılıyor - Key: {Key}", request.Key);

                var entity = await _context.PageSettings
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Key == request.Key && x.IsActive, cancellationToken);

                _logger.LogInformation("🔍 Veritabanı sorgu sonucu - Key: {Key}, Found: {Found}",
                    request.Key, entity != null);

                if (entity == null)
                {
                    _logger.LogWarning("❌ Entity bulunamadı - Key: {Key}", request.Key);

                    // DEBUG: Tüm PageSettings'leri listele
                    var allSettings = await _context.PageSettings
                        .AsNoTracking()
                        .Take(10)
                        .ToListAsync(cancellationToken);

                    _logger.LogInformation("🔍 Mevcut PageSettings (ilk 10):");
                    foreach (var setting in allSettings)
                    {
                        _logger.LogInformation("   - Key: '{Key}', Value: '{Value}', IsActive: {IsActive}",
                            setting.Key, setting.Value?.Substring(0, Math.Min(20, setting.Value?.Length ?? 0)), setting.IsActive);
                    }

                    return Result<PageSettingDto>.Failure($"Page setting with key '{request.Key}' not found.");
                }

                _logger.LogInformation("✅ Entity bulundu - Key: {Key}, Value: {Value}",
                    request.Key, entity.Value?.Substring(0, Math.Min(30, entity.Value?.Length ?? 0)));

                // MANUEL MAPPING - Entity'deki field'lara göre ayarla
                var dto = new PageSettingDto
                {
                    Id = entity.Id,
                    Key = entity.Key,
                    Value = entity.Value ?? string.Empty,
                    Category = entity.Group ?? string.Empty, // Group'u Category olarak map et
                    Description = entity.Description ?? string.Empty,
                    Group = entity.Group ?? string.Empty,
                    IsActive = entity.IsActive,
                    CreatedDate = entity.CreatedDate,
                    UpdatedDate = entity.UpdatedDate
                };

                _logger.LogInformation("✅ DTO oluşturuldu - Key: {Key}, Value: {Value}",
                    dto.Key, dto.Value?.Substring(0, Math.Min(30, dto.Value?.Length ?? 0)));

                // Cache'e ekleme - geçici olarak devre dışı
                // _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(5));

                return Result<PageSettingDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Handler'da hata oluştu - Key: {Key}", request.Key);
                return Result<PageSettingDto>.Failure($"Error retrieving page setting: {ex.Message}");
            }
        }
    }
}