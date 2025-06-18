using Application.Common.Models;
using Application.Features.PageSettings.Commands.Create;
using Application.Features.PageSettings.Commands.Update;
using Application.Features.PageSettings.Dto;
using Application.Features.PageSettings.Queries.GetAll;
using Application.Features.PageSettings.Queries.GetByKey;
using Application.Features.SEO.Commands.Create;
using Application.Features.SEO.Commands.Delete;
using Application.Features.SEO.Commands.RegenerateSitemap;
using Application.Features.SEO.Commands.Update;
using Application.Features.SEO.DTOs;
using Application.Features.SEO.Queries.GetAll;
using Application.Features.SEO.Queries.GetById;
using Application.Features.SEO.Queries.GetDefault;
using Application.Features.SEO.Queries.GetRobotsText;
using Application.Features.SEO.Queries.GetSitemap;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WEB.Api.Controllers
{
    /// <summary>
    /// Site ayarları yönetimi için API endpoints (SEO, Page Settings)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SettingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region SEO Settings

        /// <summary>
        /// Tüm SEO ayarlarını getirir (Admin)
        /// </summary>
        [HttpGet("seo")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<SeoSettingListDto>>>> GetSeoSettings(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            var query = new GetSeoSettingsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<PagedResult<SeoSettingListDto>>.SuccessResult(result));
        }

        /// <summary>
        /// Varsayılan SEO ayarlarını getirir
        /// </summary>
        [HttpGet("seo/default")]
        public async Task<ActionResult<ApiResponse<SeoSettingDetailDto>>> GetDefaultSeoSettings()
        {
            var query = new GetDefaultSeoSettingQuery();
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(ApiResponse<SeoSettingDetailDto>.ErrorResult("SEO ayarları bulunamadı."));

            return Ok(ApiResponse<SeoSettingDetailDto>.SuccessResult(result));
        }

        /// <summary>
        /// Belirli bir SEO ayarını ID ile getirir (Admin)
        /// </summary>
        [HttpGet("seo/{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<SeoSettingDetailDto>>> GetSeoSetting(int id)
        {
            var query = new GetSeoSettingByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(ApiResponse<SeoSettingDetailDto>.ErrorResult("SEO ayarı bulunamadı."));

            return Ok(ApiResponse<SeoSettingDetailDto>.SuccessResult(result));
        }

        /// <summary>
        /// Yeni SEO ayarı oluşturur (Admin)
        /// </summary>
        [HttpPost("seo")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> CreateSeoSetting([FromBody] CreateSeoSettingCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<int>.ErrorResult(result.Errors.First()));

            return CreatedAtAction(
                nameof(GetSeoSetting),
                new { id = result.Data },
                ApiResponse<int>.SuccessResult(result.Data, "SEO ayarı başarıyla oluşturuldu."));
        }

        /// <summary>
        /// SEO ayarını günceller (Admin)
        /// </summary>
        [HttpPut("seo/{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateSeoSetting(int id, [FromBody] UpdateSeoSettingCommand command)
        {
            if (id != command.Id)
                return BadRequest(ApiResponse<bool>.ErrorResult("ID uyuşmazlığı."));

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<bool>.SuccessResult(result.Data, "SEO ayarı başarıyla güncellendi."));
        }

        /// <summary>
        /// SEO ayarını siler (Admin)
        /// </summary>
        [HttpDelete("seo/{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteSeoSetting(int id)
        {
            var command = new DeleteSeoSettingCommand(id);
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<bool>.SuccessResult(result.Data, "SEO ayarı başarıyla silindi."));
        }

        /// <summary>
        /// Sitemap oluşturur (SEO)
        /// </summary>
        [HttpGet("seo/sitemap")]
        public async Task<ActionResult<string>> GenerateSitemap()
        {
            var query = new GetSitemapQuery();
            var result = await _mediator.Send(query);

            return Content(result, "application/xml");
        }

        /// <summary>
        /// Sitemap'i yeniden oluşturur (Admin)
        /// </summary>
        [HttpPost("seo/regenerate-sitemap")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> RegenerateSitemap()
        {
            var command = new RegenerateSitemapCommand();
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<string>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<string>.SuccessResult(result.Data, "Sitemap başarıyla yeniden oluşturuldu."));
        }

        /// <summary>
        /// Robots.txt içeriğini getirir (SEO)
        /// </summary>
        [HttpGet("seo/robots")]
        public async Task<ActionResult<string>> GetRobotsText()
        {
            var query = new GetRobotsTextQuery();
            var result = await _mediator.Send(query);

            return Content(result, "text/plain");
        }

        #endregion

        #region Page Settings

        /// <summary>
        /// Tüm sayfa ayarlarını getirir
        /// </summary>
        [HttpGet("page")]
        public async Task<ActionResult<ApiResponse<List<PageSettingDto>>>> GetPageSettings(
            [FromQuery] string? group = null,
            [FromQuery] bool? isActive = null)
        {
            var query = new GetAllPageSettingsQuery
            {
                Group = group,
                IsActive = isActive
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<PageSettingDto>>.SuccessResult(result));
        }

        /// <summary>
        /// Belirli bir sayfa ayarını anahtarla getirir
        /// </summary>
        [HttpGet("page/{key}")]
        public async Task<ActionResult<ApiResponse<PageSettingDto>>> GetPageSetting(string key)
        {
            var query = new GetPageSettingByKeyQuery(key);
            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return NotFound(ApiResponse<PageSettingDto>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<PageSettingDto>.SuccessResult(result.Data));
        }

        /// <summary>
        /// Grup bazında sayfa ayarlarını getirir
        /// </summary>
        [HttpGet("page/group/{group}")]
        public async Task<ActionResult<ApiResponse<List<PageSettingDto>>>> GetPageSettingsByGroup(string group)
        {
            var query = new GetAllPageSettingsQuery
            {
                Group = group,
                IsActive = true
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<PageSettingDto>>.SuccessResult(result));
        }

        /// <summary>
        /// Yeni sayfa ayarı oluşturur (Admin)
        /// </summary>
        [HttpPost("page")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> CreatePageSetting([FromBody] CreatePageSettingCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<int>.ErrorResult(result.Errors.First()));

            return CreatedAtAction(
                nameof(GetPageSetting),
                new { key = command.Key },
                ApiResponse<int>.SuccessResult(result.Data, "Sayfa ayarı başarıyla oluşturuldu."));
        }

        /// <summary>
        /// Sayfa ayarını günceller (Admin)
        /// </summary>
        [HttpPut("page/{key}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdatePageSetting(string key, [FromBody] UpdatePageSettingCommand command)
        {
            if (key != command.Key)
                return BadRequest(ApiResponse<bool>.ErrorResult("Key uyuşmazlığı."));

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<bool>.ErrorResult(result.Errors.First()));

            return Ok(ApiResponse<bool>.SuccessResult(result.Data, "Sayfa ayarı başarıyla güncellendi."));
        }

        /// <summary>
        /// Birden fazla sayfa ayarını toplu günceller (Admin)
        /// </summary>
        [HttpPut("page/bulk")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> BulkUpdatePageSettings([FromBody] Dictionary<string, string> settings)
        {
            if (settings == null || settings.Count == 0)
                return BadRequest(ApiResponse<bool>.ErrorResult("Güncellenecek ayar bulunamadı."));

            var successCount = 0;
            var errors = new List<string>();

            foreach (var setting in settings)
            {
                try
                {
                    var command = new UpdatePageSettingCommand
                    {
                        Key = setting.Key,
                        Value = setting.Value
                    };

                    var result = await _mediator.Send(command);

                    if (result.Succeeded)
                        successCount++;
                    else
                        errors.AddRange(result.Errors);
                }
                catch (Exception ex)
                {
                    errors.Add($"Key {setting.Key}: {ex.Message}");
                }
            }

            if (errors.Any())
            {
                return Ok(ApiResponse<bool>.ErrorResult(
                    $"{successCount}/{settings.Count} ayar güncellendi. Hatalar: {string.Join(", ", errors)}"));
            }

            return Ok(ApiResponse<bool>.SuccessResult(true, $"{successCount} ayar başarıyla güncellendi."));
        }

        #endregion

        #region Utility Endpoints

        /// <summary>
        /// Tüm ayarların özetini getirir (Dashboard için)
        /// </summary>
        [HttpGet("summary")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetSettingsSummary()
        {
            var seoSettings = await _mediator.Send(new GetSeoSettingsQuery { PageSize = 1 });
            var pageSettings = await _mediator.Send(new GetAllPageSettingsQuery { IsActive = true });

            var summary = new
            {
                SeoSettings = new
                {
                    Count = seoSettings.TotalCount,
                    HasDefault = seoSettings.Items.Any()
                },
                PageSettings = new
                {
                    Count = pageSettings.Count,
                    Groups = pageSettings.GroupBy(p => p.Group).Select(g => new { Group = g.Key, Count = g.Count() })
                },
                LastUpdated = DateTime.UtcNow
            };

            return Ok(ApiResponse<object>.SuccessResult(summary));
        }

        #endregion
    }
}