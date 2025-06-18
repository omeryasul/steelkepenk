// Controllers/AdminController.cs (MVC)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WEBPROJE.WEBUI.Models;
using MediatR;
using Application.Features.Categories.Queries.GetAll;
using Application.Features.Products.Queries.GetAll;
using Application.Features.ContactMessages.Queries.GetAll;
using Application.Features.PageSettings.Queries.GetByKey;
using Application.Features.PageSettings.Commands.Update;
using Application.Features.Categories.DTOs;
using Application.Features.ContactMessages.DTOs;
using Application.Features.Products.DTOs;
using Application.Features.Categories.Commands.Create;
using Application.Features.Categories.Commands.Update;
using Application.Features.Categories.Commands.Delete;
using Application.Features.Products.Commands.Create;
using Application.Features.Products.Commands.Update;
using Application.Features.Products.Commands.Delete;
using Application.Features.Products.ToggleFeatured;
using Application.Features.ContactMessages.Commands.Delete;
using Application.Features.ContactMessages.Commands.Update;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEBPROJE.WEBUI.Models.WEBPROJE.WEBUI.Models;
using Application.Features.ContactMessages.Queries.GetById;


namespace WEBPROJE.WEBUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Dashboard

        // Dashboard Ana Sayfası
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel();

            try
            {
                // Dashboard verilerini oku
                var productsResult = await _mediator.Send(new GetProductsQuery { PageSize = 1 });
                model.TotalProducts = productsResult.Succeeded ? productsResult.Data.TotalCount : 0;

                var categoriesResult = await _mediator.Send(new GetCategoriesQuery { PageSize = 1 });
                model.TotalCategories = categoriesResult.TotalCount;

                var messagesResult = await _mediator.Send(new GetContactMessagesQuery { PageSize = 1 });
                model.TotalContents = messagesResult.TotalCount;

                // Page Settings sayısını hesapla (basit bir sayım)
                model.TotalPageSettings = 15; // Sabit değer veya dinamik hesaplama

                model.SystemStatus = new SystemStatusViewModel
                {
                    DatabaseConnection = true,
                    FileSystemAccess = true,
                    MemoryUsage = true,
                    Version = "1.0.0",
                    LastBackup = DateTime.Now.AddDays(-1)
                };
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Dashboard verileri yüklenirken hata oluştu: " + ex.Message;
            }

            return View(model);
        }

        #endregion

        #region Categories

        // Kategoriler Sayfası
        [HttpGet("Categories")]
        public async Task<IActionResult> Categories()
        {
            var model = new List<CategoryListDto>();

            try
            {
                var result = await _mediator.Send(new GetCategoriesQuery { PageSize = 100 });
                model = result.Data.ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kategoriler yüklenirken hata oluştu: " + ex.Message;
            }

            return View(model);
        }

        // Kategori Ekleme
        [HttpPost("Categories/Create")]
        public async Task<IActionResult> CreateCategory(CreateCategoryCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Kategori başarıyla eklendi.";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Errors.FirstOrDefault() ?? "Kategori eklenirken hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kategori eklenirken hata oluştu: " + ex.Message;
            }

            return RedirectToAction(nameof(Categories));
        }

        // Kategori Güncelleme
        [HttpPost("Categories/Update")]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Errors.FirstOrDefault() ?? "Kategori güncellenirken hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kategori güncellenirken hata oluştu: " + ex.Message;
            }

            return RedirectToAction(nameof(Categories));
        }

        // Kategori Silme
        // Kategori Silme - POST body'den ID alacak şekilde
        [HttpPost("Categories/Delete")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteCategoryCommand(id));
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Kategori basa silindi.";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Errors.FirstOrDefault() ?? "Kategori silinirken hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kategori silinirken hata oluştu: " + ex.Message;
            }

            return RedirectToAction(nameof(Categories));
        }
        #endregion

        #region Products

        // Ürünler Sayfası
        [HttpGet("Urunler")]
        public async Task<IActionResult> Products()
        {
            var model = new List<ProductListDto>();

            try
            {
                var result = await _mediator.Send(new GetProductsQuery { PageSize = 50 });
                if (result.Succeeded)
                {
                    model = result.Data.Data.ToList();
                }

                // Kategorileri ViewBag'e ekle
                var categoriesResult = await _mediator.Send(new GetCategoriesQuery { PageSize = 100 });
                ViewBag.Categories = categoriesResult.Data.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ürünler yüklenirken hata oluştu: " + ex.Message;
                ViewBag.Categories = new List<SelectListItem>();
            }

            return View(model);
        }

        [HttpPost("Products/Create")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductCommand command)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Eksik veya hatalı alanlar var.";
                return RedirectToAction(nameof(Products));
            }

            try
            {
                // 📁 Görsel yükleme işlemi
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        command.MainImage = "/images/products/" + fileName; // DTO içinde varsa
                    }
                }

                var result = await _mediator.Send(command);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Ürün başarıyla eklendi.";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Errors.FirstOrDefault() ?? "Ürün eklenirken hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ürün eklenirken hata oluştu: " + ex.Message;
            }

            return RedirectToAction(nameof(Products));
        }

        [HttpGet("Products/GetProduct/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                // Tüm ürünleri getir ve istenen ID'yi bul
                var productsResult = await _mediator.Send(new GetProductsQuery { PageSize = 1000 });

                if (!productsResult.Succeeded)
                {
                    return Json(new { succeeded = false, message = "Ürünler getirilemedi." });
                }

                var product = productsResult.Data.Data.FirstOrDefault(p => p.Id == id);

                if (product == null)
                {
                    return Json(new { succeeded = false, message = "Ürün bulunamadı." });
                }

                // ProductListDto'dan gelen tüm alanları response'a map et
                var response = new
                {
                    id = product.Id,
                    name = product.Name,
                    slug = product.Slug,
                    categoryId = product.CategoryId,
                    shortDescription = product.ShortDescription,
                    // ProductListDto'da description alanı yok, bu alanları boş bırakıyoruz
                    description = product.ShortDescription ?? "", // Geçici olarak shortDescription kullan
                    price = product.Price,
                    discountPrice = product.DiscountPrice,
                    displayPrice = product.DisplayPrice,
                    priceNote = product.PriceNote,
                    status = (int)product.Status,
                    isFeatured = product.IsFeatured,
                    sortOrder = product.SortOrder,
                    // Bu alanlar ProductListDto'da yok - boş bırak veya default değerler ver
                    features = "",
                    specifications = "",
                    usageAreas = "",
                    advantages = "",
                    metaTitle = "",
                    metaDescription = "",
                    metaKeywords = "",
                    // ProductListDto'dan gelen ek alanlar
                    featuredImage = product.FeaturedImage,
                    mainImage = product.MainImage,
                    viewCount = product.ViewCount,
                    createdDate = product.CreatedDate,
                    updatedAt = product.UpdatedDate,
                    categoryName = product.CategoryName,
                    categorySlug = product.CategorySlug,
                    tags = product.Tags
                };

                return Json(new { succeeded = true, data = response });
            }
            catch (Exception ex)
            {
                return Json(new { succeeded = false, message = "Hata oluştu: " + ex.Message });
            }
        }


        // Ürün Güncelleme Sayfası
        [HttpGet("Products/Edit/{id}")]
        public async Task<IActionResult> EditProduct(int id)
        {
            try
            {
                // Bu kısım ürün detay sayfası için - gerekli ise implement edilecek
                return RedirectToAction(nameof(Products));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ürün yüklenirken hata oluştu: " + ex.Message;
                return RedirectToAction(nameof(Products));
            }
        }

        // Ürün Güncelleme
        [HttpPost("Products/Update")]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductCommand command)
        {
            try
            {
                // 📁 Yeni görsel yüklendiyse işle
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        command.MainImage = "/images/products/" + fileName; // DTO içinde varsa
                    }
                }

                var result = await _mediator.Send(command);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Errors.FirstOrDefault() ?? "Ürün güncellenirken hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ürün güncellenirken hata oluştu: " + ex.Message;
            }

            return RedirectToAction(nameof(Products));
        }


        // Ürün Silme
        [HttpPost("Products/Delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteProductCommand(id));
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Ürün başarıyla silindi.";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Errors.FirstOrDefault() ?? "Ürün silinirken hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ürün silinirken hata oluştu: " + ex.Message;
            }

            return RedirectToAction(nameof(Products));
        }

        // Ürün Öne Çıkarma Toggle
        [HttpPost("Products/ToggleFeatured/{id}")]
        public async Task<IActionResult> ToggleProductFeaturedCommand(int id)
        {
            try
            {
                var result = await _mediator.Send(new ToggleProductFeaturedCommand(id));
                if (result.Succeeded)
                {
                    return Json(new { succeeded = true, message = "Ürün durumu başarıyla güncellendi." });
                }
                else
                {
                    return Json(new { succeeded = false, message = result.Errors.FirstOrDefault() ?? "İşlem başarısız." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { succeeded = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        #endregion

        #region Messages

        // Mesajlar Sayfası
        [HttpGet("Messages")]
        public async Task<IActionResult> Messages()
        {
            var model = new List<ContactMessageListDto>();

            try
            {
                var result = await _mediator.Send(new GetContactMessagesQuery
                {
                    PageNumber = 1,
                    PageSize = 50,
                    SortBy = "CreatedDate",
                    SortDescending = true
                });

                model = result.Data.ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Mesajlar yüklenirken hata oluştu: " + ex.Message;
            }

            return View(model); // Bu satır doğru view'a gitmeli
        }

        // Mesajı Okundu Olarak İşaretle
        [HttpPost("Messages/MarkAsRead/{id}")]
        public async Task<IActionResult> MarkMessageAsRead(int id)
        {
            try
            {
                var result = await _mediator.Send(new MarkAsReadCommand(id));
                if (result.Succeeded)
                {
                    return Json(new { succeeded = true, message = "Mesaj okundu olarak işaretlendi." });
                }
                else
                {
                    return Json(new { succeeded = false, message = result.Errors.FirstOrDefault() ?? "İşlem başarısız." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { succeeded = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        // Mesaj Silme
        [HttpPost("Messages/Delete/{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteContactMessageCommand(id));
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Mesaj başarıyla silindi.";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Errors.FirstOrDefault() ?? "Mesaj silinirken hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Mesaj silinirken hata oluştu: " + ex.Message;
            }

            return RedirectToAction(nameof(Messages));
        }
        // Mesaj Detayları
        [HttpGet("Messages/Details/{id}")]
        public async Task<IActionResult> GetMessageDetails(int id)
        {
            try
            {
                var message = await _mediator.Send(new GetContactMessageByIdQuery(id));

                if (message != null)
                {
                    return Json(new { succeeded = true, data = message });
                }
                else
                {
                    return Json(new { succeeded = false, message = "Mesaj bulunamadı." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { succeeded = false, message = "Hata oluştu: " + ex.Message });
            }
        }
        [HttpPost("Messages/MarkAllAsRead")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var result = await _mediator.Send(new MarkAllAsReadCommand());
                if (result.Succeeded)
                {
                    return Json(new { succeeded = true, message = "Tüm mesajlar okundu olarak işaretlendi." });
                }
                else
                {
                    return Json(new { succeeded = false, message = result.Errors.FirstOrDefault() ?? "İşlem başarısız." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { succeeded = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        #endregion

        #region Homepage Settings

        // Ana Sayfa Ayarları - Genişletilmiş
        [HttpGet("Homepage")]
        public async Task<IActionResult> Homepage()
        {
            var model = new HomePageSettingsViewModel();

            try
            {
                // Mevcut Hero ayarları
                var heroTitleResult = await _mediator.Send(new GetPageSettingByKeyQuery("HeroTitle"));
                var heroSubtitleResult = await _mediator.Send(new GetPageSettingByKeyQuery("HeroSubtitle"));
                var heroDescriptionResult = await _mediator.Send(new GetPageSettingByKeyQuery("HeroDescription"));
                var heroPhoneResult = await _mediator.Send(new GetPageSettingByKeyQuery("HeroPhone"));
                var heroWhatsAppResult = await _mediator.Send(new GetPageSettingByKeyQuery("HeroWhatsApp"));

                // Hero istatistikleri
                var heroStat1NumberResult = await _mediator.Send(new GetPageSettingByKeyQuery("HeroStat1Number"));
                var heroStat1LabelResult = await _mediator.Send(new GetPageSettingByKeyQuery("HeroStat1Label"));
                var heroStat2NumberResult = await _mediator.Send(new GetPageSettingByKeyQuery("HeroStat2Number"));
                var heroStat2LabelResult = await _mediator.Send(new GetPageSettingByKeyQuery("HeroStat2Label"));
                var heroStat3NumberResult = await _mediator.Send(new GetPageSettingByKeyQuery("HeroStat3Number"));
                var heroStat3LabelResult = await _mediator.Send(new GetPageSettingByKeyQuery("HeroStat3Label"));

                // Emergency ayarları
                var emergencyTitleResult = await _mediator.Send(new GetPageSettingByKeyQuery("EmergencyTitle"));
                var emergencyDescriptionResult = await _mediator.Send(new GetPageSettingByKeyQuery("EmergencyDescription"));
                var emergencyPhoneResult = await _mediator.Send(new GetPageSettingByKeyQuery("EmergencyPhone"));

                // Company ayarları
                var companyTitleResult = await _mediator.Send(new GetPageSettingByKeyQuery("CompanyTitle"));
                var companyDescriptionResult = await _mediator.Send(new GetPageSettingByKeyQuery("CompanyDescription"));
                var companyPhoneResult = await _mediator.Send(new GetPageSettingByKeyQuery("CompanyPhone"));
                var companyPhoneTextResult = await _mediator.Send(new GetPageSettingByKeyQuery("CompanyPhoneText"));
                var companyExperienceResult = await _mediator.Send(new GetPageSettingByKeyQuery("CompanyExperience"));

                // CTA ayarları
                var ctaTitleResult = await _mediator.Send(new GetPageSettingByKeyQuery("CtaTitle"));
                var ctaDescriptionResult = await _mediator.Send(new GetPageSettingByKeyQuery("CtaDescription"));
                var ctaFeature1Result = await _mediator.Send(new GetPageSettingByKeyQuery("CtaFeature1"));
                var ctaFeature2Result = await _mediator.Send(new GetPageSettingByKeyQuery("CtaFeature2"));
                var ctaFeature3Result = await _mediator.Send(new GetPageSettingByKeyQuery("CtaFeature3"));

                // Bottom CTA ayarları
                var bottomCtaTitleResult = await _mediator.Send(new GetPageSettingByKeyQuery("BottomCtaTitle"));
                var bottomCtaDescriptionResult = await _mediator.Send(new GetPageSettingByKeyQuery("BottomCtaDescription"));

                // Model'i doldur
                model.HeroTitle = heroTitleResult.Succeeded && heroTitleResult.Data != null ? heroTitleResult.Data.Value : "HIZLI SERVİS KESİNTİSİZ DESTEK";
                model.HeroSubtitle = heroSubtitleResult.Succeeded && heroSubtitleResult.Data != null ? heroSubtitleResult.Data.Value : "OTOMATİK KEPENEK SİSTEMLERİ";
                model.HeroDescription = heroDescriptionResult.Succeeded && heroDescriptionResult.Data != null ? heroDescriptionResult.Data.Value : "Otomatik kepenek sistemlerinizde oluşan tüm arızaları en hızlı şekilde çözmek için hizmetinizdeyiz.";
                model.HeroPhone = heroPhoneResult.Succeeded && heroPhoneResult.Data != null ? heroPhoneResult.Data.Value : "+905336619312";
                model.HeroWhatsApp = heroWhatsAppResult.Succeeded && heroWhatsAppResult.Data != null ? heroWhatsAppResult.Data.Value : "+905336619312";

                // İstatistikler
                model.HeroStat1Number = heroStat1NumberResult.Succeeded && heroStat1NumberResult.Data != null ? heroStat1NumberResult.Data.Value : "500+";
                model.HeroStat1Label = heroStat1LabelResult.Succeeded && heroStat1LabelResult.Data != null ? heroStat1LabelResult.Data.Value : "Mutlu Müşteri";
                model.HeroStat2Number = heroStat2NumberResult.Succeeded && heroStat2NumberResult.Data != null ? heroStat2NumberResult.Data.Value : "15+";
                model.HeroStat2Label = heroStat2LabelResult.Succeeded && heroStat2LabelResult.Data != null ? heroStat2LabelResult.Data.Value : "Yıl Tecrübe";
                model.HeroStat3Number = heroStat3NumberResult.Succeeded && heroStat3NumberResult.Data != null ? heroStat3NumberResult.Data.Value : "7/24";
                model.HeroStat3Label = heroStat3LabelResult.Succeeded && heroStat3LabelResult.Data != null ? heroStat3LabelResult.Data.Value : "Destek";

                // Emergency
                model.EmergencyTitle = emergencyTitleResult.Succeeded && emergencyTitleResult.Data != null ? emergencyTitleResult.Data.Value : "ACİL DURUM SERVİSİ";
                model.EmergencyDescription = emergencyDescriptionResult.Succeeded && emergencyDescriptionResult.Data != null ? emergencyDescriptionResult.Data.Value : "7/24 acil servis hizmeti sunuyoruz";
                model.EmergencyPhone = emergencyPhoneResult.Succeeded && emergencyPhoneResult.Data != null ? emergencyPhoneResult.Data.Value : "+905336619312";

                // Company
                model.CompanyTitle = companyTitleResult.Succeeded && companyTitleResult.Data != null ? companyTitleResult.Data.Value : "Uzman Ekibimizle Güvenilir Hizmet";
                model.CompanyDescription = companyDescriptionResult.Succeeded && companyDescriptionResult.Data != null ? companyDescriptionResult.Data.Value : "15 yıllık tecrübemizle otomatik kepenek sistemlerinde uzmanız.";
                model.CompanyPhone = companyPhoneResult.Succeeded && companyPhoneResult.Data != null ? companyPhoneResult.Data.Value : "+905336619312";
                model.CompanyPhoneText = companyPhoneTextResult.Succeeded && companyPhoneTextResult.Data != null ? companyPhoneTextResult.Data.Value : "7/24 Destek Hattı";
                model.CompanyExperience = companyExperienceResult.Succeeded && companyExperienceResult.Data != null ? companyExperienceResult.Data.Value : "15+";

                // CTA
                model.CtaTitle = ctaTitleResult.Succeeded && ctaTitleResult.Data != null ? ctaTitleResult.Data.Value : "Hızlı Çözüm İçin Bizi Arayın";
                model.CtaDescription = ctaDescriptionResult.Succeeded && ctaDescriptionResult.Data != null ? ctaDescriptionResult.Data.Value : "Profesyonel ekibimizle 7/24 hizmetinizdeyiz";
                model.CtaFeature1 = ctaFeature1Result.Succeeded && ctaFeature1Result.Data != null ? ctaFeature1Result.Data.Value : "7/24 Hizmet";
                model.CtaFeature2 = ctaFeature2Result.Succeeded && ctaFeature2Result.Data != null ? ctaFeature2Result.Data.Value : "Hızlı Çözüm";
                model.CtaFeature3 = ctaFeature3Result.Succeeded && ctaFeature3Result.Data != null ? ctaFeature3Result.Data.Value : "Güvenli Servis";

                // Bottom CTA
                model.BottomCtaTitle = bottomCtaTitleResult.Succeeded && bottomCtaTitleResult.Data != null ? bottomCtaTitleResult.Data.Value : "Kepenek Arızanız mı Var?";
                model.BottomCtaDescription = bottomCtaDescriptionResult.Succeeded && bottomCtaDescriptionResult.Data != null ? bottomCtaDescriptionResult.Data.Value : "Hemen arayın, 30 dakikada geliyoruz";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ana sayfa ayarları yüklenirken hata oluştu: " + ex.Message;
            }

            return View(model);
        }
        // Ana Sayfa Ayarları Kaydet
        [HttpPost("Homepage")]
        public async Task<IActionResult> Homepage(HomePageSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Hero Alanları
                await _mediator.Send(new UpdatePageSettingCommand { Key = "HeroTitle", Value = model.HeroTitle });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "HeroSubtitle", Value = model.HeroSubtitle });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "HeroDescription", Value = model.HeroDescription });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "HeroPhone", Value = model.HeroPhone });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "HeroWhatsApp", Value = model.HeroWhatsApp });

                // Hero İstatistikleri
                await _mediator.Send(new UpdatePageSettingCommand { Key = "HeroStat1Number", Value = model.HeroStat1Number });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "HeroStat1Label", Value = model.HeroStat1Label });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "HeroStat2Number", Value = model.HeroStat2Number });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "HeroStat2Label", Value = model.HeroStat2Label });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "HeroStat3Number", Value = model.HeroStat3Number });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "HeroStat3Label", Value = model.HeroStat3Label });

                // Emergency
                await _mediator.Send(new UpdatePageSettingCommand { Key = "EmergencyTitle", Value = model.EmergencyTitle });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "EmergencyDescription", Value = model.EmergencyDescription });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "EmergencyPhone", Value = model.EmergencyPhone });

                // Company
                await _mediator.Send(new UpdatePageSettingCommand { Key = "CompanyTitle", Value = model.CompanyTitle });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "CompanyDescription", Value = model.CompanyDescription });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "CompanyPhone", Value = model.CompanyPhone });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "CompanyPhoneText", Value = model.CompanyPhoneText });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "CompanyExperience", Value = model.CompanyExperience });

                // CTA
                await _mediator.Send(new UpdatePageSettingCommand { Key = "CtaTitle", Value = model.CtaTitle });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "CtaDescription", Value = model.CtaDescription });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "CtaFeature1", Value = model.CtaFeature1 });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "CtaFeature2", Value = model.CtaFeature2 });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "CtaFeature3", Value = model.CtaFeature3 });

                // Bottom CTA
                await _mediator.Send(new UpdatePageSettingCommand { Key = "BottomCtaTitle", Value = model.BottomCtaTitle });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "BottomCtaDescription", Value = model.BottomCtaDescription });

                TempData["SuccessMessage"] = "Ana sayfa ayarları başarıyla kaydedildi.";
                return RedirectToAction(nameof(Homepage));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kaydetme sırasında hata oluştu: " + ex.Message;
                return View(model);
            }
        }


        #endregion

        #region SEO Settings

        // SEO Ayarları
        [HttpGet("Seo")]
        public async Task<IActionResult> Seo()
        {
            var model = new SeoSettingsViewModel();

            try
            {
                var siteNameResult = await _mediator.Send(new GetPageSettingByKeyQuery("SiteName"));
                var siteDescriptionResult = await _mediator.Send(new GetPageSettingByKeyQuery("SiteDescription"));
                var siteKeywordsResult = await _mediator.Send(new GetPageSettingByKeyQuery("SiteKeywords"));
                var googleAnalyticsResult = await _mediator.Send(new GetPageSettingByKeyQuery("GoogleAnalytics"));

                model.SiteName = siteNameResult.Succeeded && siteNameResult.Data != null ?
                    siteNameResult.Data.Value : "Furkan Kepenek - Otomatik Kepenek Tamiri İstanbul";
                model.SiteDescription = siteDescriptionResult.Succeeded && siteDescriptionResult.Data != null ?
                    siteDescriptionResult.Data.Value : "İstanbul'da otomatik kepenek sistemleri, tamiri ve bakım hizmetleri.";
                model.SiteKeywords = siteKeywordsResult.Succeeded && siteKeywordsResult.Data != null ?
                    siteKeywordsResult.Data.Value : "otomatik kepenek, kepenek tamiri, istanbul kepenek";
                model.GoogleAnalytics = googleAnalyticsResult.Succeeded && googleAnalyticsResult.Data != null ?
                    googleAnalyticsResult.Data.Value : "";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "SEO ayarları yüklenirken hata oluştu: " + ex.Message;
            }

            return View(model);
        }

        // SEO Ayarları Kaydet
        [HttpPost("Seo")]
        public async Task<IActionResult> Seo(SeoSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _mediator.Send(new UpdatePageSettingCommand { Key = "SiteName", Value = model.SiteName });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "SiteDescription", Value = model.SiteDescription });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "SiteKeywords", Value = model.SiteKeywords });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "GoogleAnalytics", Value = model.GoogleAnalytics });

                TempData["SuccessMessage"] = "SEO ayarları başarıyla kaydedildi.";
                return RedirectToAction(nameof(Seo));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kaydetme sırasında hata oluştu: " + ex.Message;
                return View(model);
            }
        }
        #region About Settings

        // Hakkımızda Ayarları
        [HttpGet("About")]
        public async Task<IActionResult> About()
        {
            try
            {
                ViewBag.AboutText1 = await GetPageSetting("AboutText1") ?? "Furkan Kepenek, kepenek ve kapı sistemlerinin temeli 2015 yılında hizmetine başlamış olup geçen süre zarfında firmaların birçok projesinde satış ve çözüm ortağı olmuştur.";
                ViewBag.AboutText2 = await GetPageSetting("AboutText2") ?? "Furkan Kepenek otomatik kapı sistemleri; sektöründeki teknolojik gelişmeleri yakından takip ederek müşterilerine en son teknolojiyi sunmakla birlikte ekonomik, estetik, uzun ömürlü hizmet anlayışı ile birçok kurumsal firmanında aralarında bulunduğu müşterilerine en uygun çözümü sunmaktadır.";
                ViewBag.AboutText3 = await GetPageSetting("AboutText3") ?? "Türkiye'nin seçkin firmaları ve perakende kullanıcılarının, firmamızı tercih etmelerindeki neden; Daha hızlı, daha kaliteli, daha güvenilir Teklif verirken, üretimde, montajda, hizmette en iyi zamanlama. Tecrübeli ve eğitimli kadro ile hizmet veriyor olmamız.";
                ViewBag.AboutText4 = await GetPageSetting("AboutText4") ?? "Satış ve satış sonrası kepenek arıza servis hizmetlerinde yüksek müşteri memnuniyeti. Esnek üretim ve çalışma yapısıyla her projeye uygun çözümler sunan yaklaşımla sektördeki varlığını devam ettirmektedir.";
                ViewBag.AboutText5 = await GetPageSetting("AboutText5") ?? "\"Sorun\" değil, \"çözüm\" yaratmaya yönelik yaklaşım. Büyük ölçekli projeleri başarı ile sonuçlandırabilmemiz Furkan Kepenek olarak ilk tercihler arasında olmamızı sağlıyor.";
            }
            catch (Exception ex)
            {
                // Hata durumunda varsayılan metinleri göster
            }

            return View();
        }

        private async Task<string> GetPageSetting(string key)
        {
            try
            {
                var result = await _mediator.Send(new GetPageSettingByKeyQuery(key));
                return result.Succeeded && result.Data != null ? result.Data.Value : null;
            }
            catch
            {
                return null;
            }
        }
        // Hakkımızda Ayarları Kaydet
        [HttpPost("About")]
        public async Task<IActionResult> About(AboutSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _mediator.Send(new UpdatePageSettingCommand { Key = "AboutText1", Value = model.AboutText1 });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "AboutText2", Value = model.AboutText2 });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "AboutText3", Value = model.AboutText3 });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "AboutText4", Value = model.AboutText4 });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "AboutText5", Value = model.AboutText5 });

                TempData["SuccessMessage"] = "Hakkımızda ayarları başarıyla kaydedildi.";
                return RedirectToAction(nameof(About));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kaydetme sırasında hata oluştu: " + ex.Message;
                return View(model);
            }
        }

        #endregion
        #endregion

        #region Contact Settings

        // İletişim Ayarları
        [HttpGet("ContactSettings")]
        public async Task<IActionResult> ContactSettings()
        {
            var model = new ContactSettingsViewModel();

            try
            {
                var phoneResult = await _mediator.Send(new GetPageSettingByKeyQuery("ContactPhone"));
                var whatsappResult = await _mediator.Send(new GetPageSettingByKeyQuery("ContactWhatsApp"));
                var emailResult = await _mediator.Send(new GetPageSettingByKeyQuery("ContactEmail"));
                var addressResult = await _mediator.Send(new GetPageSettingByKeyQuery("ContactAddress"));
                var workingHoursResult = await _mediator.Send(new GetPageSettingByKeyQuery("WorkingHours"));

                model.Phone = phoneResult.Succeeded && phoneResult.Data != null ? phoneResult.Data.Value : "+905336619312";
                model.WhatsApp = whatsappResult.Succeeded && whatsappResult.Data != null ? whatsappResult.Data.Value : "+905336619312";
                model.Email = emailResult.Succeeded && emailResult.Data != null ? emailResult.Data.Value : "info@furkankepenek.com";
                model.Address = addressResult.Succeeded && addressResult.Data != null ? addressResult.Data.Value : "İstanbul, Türkiye";
                model.WorkingHours = workingHoursResult.Succeeded && workingHoursResult.Data != null ? workingHoursResult.Data.Value : "Pazartesi - Pazar: 08:00 - 22:00";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "İletişim ayarları yüklenirken hata oluştu: " + ex.Message;
            }

            return View(model);
        }
        #region Contact Page Settings

        // İletişim Sayfası Ayarları
        [HttpGet("ContactPage")]
        public async Task<IActionResult> ContactPage()
        {
            var model = new ContactPageSettingsViewModel();

            try
            {
                var contactTitleResult = await _mediator.Send(new GetPageSettingByKeyQuery("ContactTitle"));
                var contactSubtitleResult = await _mediator.Send(new GetPageSettingByKeyQuery("ContactSubtitle"));
                var contactDescriptionResult = await _mediator.Send(new GetPageSettingByKeyQuery("ContactDescription"));
                var contactPhoneResult = await _mediator.Send(new GetPageSettingByKeyQuery("ContactPhone"));
                var contactWhatsAppResult = await _mediator.Send(new GetPageSettingByKeyQuery("ContactWhatsApp"));
                var contactEmailResult = await _mediator.Send(new GetPageSettingByKeyQuery("ContactEmail"));
                var contactAddressResult = await _mediator.Send(new GetPageSettingByKeyQuery("ContactAddress"));
                var contactWorkingHoursResult = await _mediator.Send(new GetPageSettingByKeyQuery("WorkingHours"));
                var contactMapEmbedResult = await _mediator.Send(new GetPageSettingByKeyQuery("ContactMapEmbed"));

                model.ContactTitle = contactTitleResult.Succeeded && contactTitleResult.Data != null ?
                    contactTitleResult.Data.Value : "Bizimle İletişime Geçin";
                model.ContactSubtitle = contactSubtitleResult.Succeeded && contactSubtitleResult.Data != null ?
                    contactSubtitleResult.Data.Value : "İletişim Bilgilerimiz";
                model.ContactDescription = contactDescriptionResult.Succeeded && contactDescriptionResult.Data != null ?
                    contactDescriptionResult.Data.Value : "Sorularınız ve talepleriniz için bizimle iletişime geçebilirsiniz.";
                model.ContactPhone = contactPhoneResult.Succeeded && contactPhoneResult.Data != null ?
                    contactPhoneResult.Data.Value : "+905336619312";
                model.ContactWhatsApp = contactWhatsAppResult.Succeeded && contactWhatsAppResult.Data != null ?
                    contactWhatsAppResult.Data.Value : "+905336619312";
                model.ContactEmail = contactEmailResult.Succeeded && contactEmailResult.Data != null ?
                    contactEmailResult.Data.Value : "info@furkankepenek.com";
                model.ContactAddress = contactAddressResult.Succeeded && contactAddressResult.Data != null ?
                    contactAddressResult.Data.Value : "İstanbul, Türkiye";
                model.WorkingHours = contactWorkingHoursResult.Succeeded && contactWorkingHoursResult.Data != null ?
                    contactWorkingHoursResult.Data.Value : "Pazartesi - Pazar: 08:00 - 22:00";
                model.ContactMapEmbed = contactMapEmbedResult.Succeeded && contactMapEmbedResult.Data != null ?
                    contactMapEmbedResult.Data.Value : "";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "İletişim sayfası ayarları yüklenirken hata oluştu: " + ex.Message;
            }

            return View(model);
        }

        [HttpPost("ContactPage")]
        public async Task<IActionResult> ContactPage(ContactPageSettingsViewModel model)
        {
            // Debug için model state kontrol et
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToList();

                TempData["ErrorMessage"] = "Model doğrulama hatası: " + string.Join(", ", errors.Select(e => $"{e.Field}: {string.Join(", ", e.Errors)}"));
                return View(model);
            }

            try
            {
                // Debug: Hangi değerler geldiğini kontrol et
                System.Diagnostics.Debug.WriteLine($"ContactTitle: {model.ContactTitle}");
                System.Diagnostics.Debug.WriteLine($"ContactPhone: {model.ContactPhone}");

                await _mediator.Send(new UpdatePageSettingCommand { Key = "ContactTitle", Value = model.ContactTitle });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "ContactSubtitle", Value = model.ContactSubtitle });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "ContactDescription", Value = model.ContactDescription });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "ContactPhone", Value = model.ContactPhone });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "ContactWhatsApp", Value = model.ContactWhatsApp });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "ContactEmail", Value = model.ContactEmail });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "ContactAddress", Value = model.ContactAddress });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "WorkingHours", Value = model.WorkingHours });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "ContactMapEmbed", Value = model.ContactMapEmbed ?? "" });

                TempData["SuccessMessage"] = "İletişim sayfası ayarları başarıyla kaydedildi.";
                return RedirectToAction(nameof(ContactPage));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kaydetme sırasında hata oluştu: " + ex.Message;
                return View(model);
            }
        }
        #endregion

        // İletişim Ayarları Kaydet
        [HttpPost("ContactSettings")]
        public async Task<IActionResult> ContactSettings(ContactSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _mediator.Send(new UpdatePageSettingCommand { Key = "ContactPhone", Value = model.Phone });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "ContactWhatsApp", Value = model.WhatsApp });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "ContactEmail", Value = model.Email });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "ContactAddress", Value = model.Address });
                await _mediator.Send(new UpdatePageSettingCommand { Key = "WorkingHours", Value = model.WorkingHours });

                TempData["SuccessMessage"] = "İletişim ayarları başarıyla kaydedildi.";
                return RedirectToAction(nameof(ContactSettings));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kaydetme sırasında hata oluştu: " + ex.Message;
                return View(model);
            }
        }

        #endregion

        #region Utility Methods

        // Kategori listesi JSON olarak döndür (AJAX için)
        [HttpGet("GetCategories")]
        public async Task<JsonResult> GetCategories()
        {
            try
            {
                var result = await _mediator.Send(new GetCategoriesQuery { PageSize = 100 });
                var categories = result.Data.Select(c => new { value = c.Id, text = c.Name }).ToList();
                return Json(categories);
            }
            catch
            {
                return Json(new List<object>());
            }
        }

        #endregion
    }
}