// Controllers/HomeController.cs - Güvenli versiyonu
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Caching.Memory;
using WEBPROJE.WEBUI.Models;
using WEBPROJE.WEBUI.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace WEBPROJE.WEBUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _cache;
        private readonly IAntiforgery _antiforgery;
        private readonly IWebHostEnvironment _environment;

        public HomeController(
            ILogger<HomeController> logger,
            IMemoryCache cache,
            IAntiforgery antiforgery,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _cache = cache;
            _antiforgery = antiforgery;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Cache key güvenli oluşturma
                var cacheKey = "home_data_v1";

                if (!_cache.TryGetValue(cacheKey, out HomeViewModel model))
                {
                    model = await LoadHomeDataAsync();

                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                        SlidingExpiration = TimeSpan.FromMinutes(10),
                        Priority = CacheItemPriority.Normal
                    };

                    _cache.Set(cacheKey, model, cacheOptions);
                }

                // Script nonce oluştur (CSP için)
                ViewBag.ScriptNonce = GenerateNonce();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Contact()
        {
            var model = new ContactFormViewModel
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            ViewBag.ScriptNonce = GenerateNonce();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactFormViewModel model)
        {
            try
            {
                // Bot koruması: Honeypot kontrolü
                if (!string.IsNullOrEmpty(model.Website))
                {
                    _logger.LogWarning("Bot detected in contact form. IP: {IP}", GetClientIpAddress());
                    return RedirectToAction(nameof(Contact));
                }

                // Rate limiting: Süre kontrolü
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (now - model.Timestamp < 3)
                {
                    ModelState.AddModelError("", "Lütfen daha yavaş gönderin.");
                    return View(model);
                }

                // Rate limiting: IP bazlı kontrol
                var clientIp = GetClientIpAddress();
                var rateLimitKey = $"contact_rate_limit_{clientIp}";

                if (_cache.TryGetValue(rateLimitKey, out int requestCount))
                {
                    if (requestCount >= 3) // Saatte maksimum 3 istek
                    {
                        ModelState.AddModelError("", "Çok fazla istek gönderdiniz. Lütfen daha sonra tekrar deneyin.");
                        return View(model);
                    }
                    _cache.Set(rateLimitKey, requestCount + 1, TimeSpan.FromHours(1));
                }
                else
                {
                    _cache.Set(rateLimitKey, 1, TimeSpan.FromHours(1));
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Ek güvenlik validasyonu
                if (!IsValidContactRequest(model))
                {
                    ModelState.AddModelError("", "Geçersiz istek.");
                    return View(model);
                }

                // İçerik sanitization
                model.FirstName = SanitizeInput(model.FirstName);
                model.LastName = SanitizeInput(model.LastName);
                model.Subject = SanitizeInput(model.Subject);
                model.Message = SanitizeInput(model.Message);

                // E-posta gönderimi (güvenli implementation)
                var emailSent = await SendContactEmailAsync(model, clientIp);

                if (emailSent)
                {
                    TempData["Success"] = "Mesajınız başarıyla gönderildi. En kısa sürede sizinle iletişime geçeceğiz.";

                    // Log success
                    _logger.LogInformation("Contact form submitted successfully. Email: {Email}, IP: {IP}",
                        model.Email, clientIp);
                }
                else
                {
                    TempData["Error"] = "Mesaj gönderilirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                }

                return RedirectToAction(nameof(Contact));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form. IP: {IP}", GetClientIpAddress());
                TempData["Error"] = "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult About()
        {
            // About page data'sını güvenli şekilde yükle
            ViewBag.ScriptNonce = GenerateNonce();
            return View();
        }

        // CSRF Token yenileme endpoint'i
        [HttpGet]
        public IActionResult RefreshCsrfToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            return Json(new { token = tokens.RequestToken });
        }

        #region Private Helper Methods

        private async Task<HomeViewModel> LoadHomeDataAsync()
        {
            // Veri yükleme implementasyonu
            // Bu method'da XSS koruması için tüm string değerler encode edilmeli
            return new HomeViewModel
            {
                // Model properties...
            };
        }

        private string GenerateNonce()
        {
            var bytes = new byte[16];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        private string GetClientIpAddress()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Proxy arkasındaysa gerçek IP'yi al
            if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                var forwardedIps = HttpContext.Request.Headers["X-Forwarded-For"].ToString();
                if (!string.IsNullOrEmpty(forwardedIps))
                {
                    ipAddress = forwardedIps.Split(',')[0].Trim();
                }
            }
            else if (HttpContext.Request.Headers.ContainsKey("X-Real-IP"))
            {
                ipAddress = HttpContext.Request.Headers["X-Real-IP"].ToString();
            }

            return ipAddress ?? "Unknown";
        }

        private bool IsValidContactRequest(ContactFormViewModel model)
        {
            // Ek güvenlik kontrolleri

            // Spam kontrolü: Çok fazla link varsa reddet
            var linkCount = System.Text.RegularExpressions.Regex.Matches(
                model.Message, @"https?://", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Count;

            if (linkCount > 2)
            {
                _logger.LogWarning("Potential spam detected: too many links. Email: {Email}, IP: {IP}",
                    model.Email, GetClientIpAddress());
                return false;
            }

            // Şüpheli keyword kontrolü
            var suspiciousKeywords = new[]
            {
                "viagra", "casino", "lottery", "winner", "congratulations",
                "click here", "act now", "limited time", "free money"
            };

            var messageText = model.Message.ToLower();
            if (suspiciousKeywords.Any(keyword => messageText.Contains(keyword)))
            {
                _logger.LogWarning("Potential spam detected: suspicious keywords. Email: {Email}, IP: {IP}",
                    model.Email, GetClientIpAddress());
                return false;
            }

            return true;
        }

        private string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // XSS koruması
            input = System.Web.HttpUtility.HtmlEncode(input);

            // SQL injection koruması (ek güvenlik)
            input = input.Replace("'", "&#39;")
                        .Replace("\"", "&#34;")
                        .Replace("<", "&lt;")
                        .Replace(">", "&gt;");

            return input.Trim();
        }

        private async Task<bool> SendContactEmailAsync(ContactFormViewModel model, string clientIp)
        {
            try
            {
                // E-posta gönderimi implementasyonu
                // SMTP ayarları environment variable'lardan alınmalı

                var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");
                var smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
                var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
                var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS");
                var toEmail = Environment.GetEnvironmentVariable("CONTACT_EMAIL");

                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(toEmail))
                {
                    _logger.LogError("SMTP configuration not found");
                    return false;
                }

                // E-posta içeriği oluştur (HTML encode edilmiş)
                var emailBody = $@"
                    <h3>İletişim Formu Mesajı</h3>
                    <p><strong>Ad Soyad:</strong> {model.FirstName} {model.LastName}</p>
                    <p><strong>E-posta:</strong> {model.Email}</p>
                    <p><strong>Telefon:</strong> {model.Phone ?? "Belirtilmemiş"}</p>
                    <p><strong>Konu:</strong> {model.Subject}</p>
                    <p><strong>Mesaj:</strong></p>
                    <p>{model.Message.Replace("\n", "<br>")}</p>
                    <hr>
                    <small>IP Adresi: {clientIp}</small>
                    <small>Tarih: {DateTime.Now:dd.MM.yyyy HH:mm:ss}</small>
                ";

                // SMTP ile e-posta gönder
                using var client = new System.Net.Mail.SmtpClient(smtpHost, smtpPort);
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass);

                var mailMessage = new System.Net.Mail.MailMessage
                {
                    From = new System.Net.Mail.MailAddress(smtpUser, "Website İletişim Formu"),
                    Subject = $"İletişim Formu: {model.Subject}",
                    Body = emailBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);
                mailMessage.ReplyToList.Add(new System.Net.Mail.MailAddress(model.Email, $"{model.FirstName} {model.LastName}"));

                await client.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send contact email");
                return false;
            }
        }

        #endregion
    }
}