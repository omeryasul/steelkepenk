using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace WEBPROJE.WEBUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Account")] // Bu satır eklendi
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("Login")] // Route attribute eklendi
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            // Zaten giriş yapmışsa admin paneline yönlendir
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Admin");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("Login")] // Route attribute eklendi
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

                    if (result.Succeeded)
                    {
                        var user = await _userManager.FindByEmailAsync(model.Email);
                        if (user != null && user.IsAdmin)
                        {
                            _logger.LogInformation("Admin user logged in.");
                            return RedirectToLocal(returnUrl ?? "/Admin");
                        }
                        else
                        {
                            await _signInManager.SignOutAsync();
                            ModelState.AddModelError(string.Empty, "Bu hesap admin paneline erişim yetkisine sahip değil.");
                            return View(model);
                        }
                    }

                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "Hesap kilitlenmiş.");
                    }
                    else if (result.IsNotAllowed)
                    {
                        ModelState.AddModelError(string.Empty, "Giriş yapma izni yok.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Geçersiz email veya şifre.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Login sırasında hata oluştu");
                    ModelState.AddModelError(string.Empty, "Giriş sırasında bir hata oluştu.");
                }
            }

            return View(model);
        }

        [HttpPost("Logout")] // Route attribute eklendi
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout sırasında hata oluştu");
                return RedirectToAction("Login");
            }
        }

        [HttpGet("AccessDenied")] // Route attribute eklendi
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Admin", new { area = "Admin" });
        }
    }

    public class LoginViewModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public bool RememberMe { get; set; }
    }
}