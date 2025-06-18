using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SEO.Commands.Create
{
    public class CreateSeoSettingCommandValidator : AbstractValidator<CreateSeoSettingCommand>
    {
        public CreateSeoSettingCommandValidator()
        {
            RuleFor(x => x.SiteName)
                .NotEmpty().WithMessage("Site adı gereklidir.")
                .MaximumLength(100).WithMessage("Site adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.SiteDescription)
                .NotEmpty().WithMessage("Site açıklaması gereklidir.")
                .MaximumLength(160).WithMessage("Site açıklaması en fazla 160 karakter olabilir.");

            RuleFor(x => x.SiteKeywords)
                .MaximumLength(500).WithMessage("Site anahtar kelimeleri en fazla 500 karakter olabilir.");

            RuleFor(x => x.SiteUrl)
                .Must(BeValidUrl).When(x => !string.IsNullOrEmpty(x.SiteUrl))
                .WithMessage("Geçerli bir URL giriniz.");

            RuleFor(x => x.GoogleAnalyticsId)
                .Matches(@"^UA-\d+-\d+$|^G-[A-Z0-9]+$").When(x => !string.IsNullOrEmpty(x.GoogleAnalyticsId))
                .WithMessage("Geçerli bir Google Analytics ID giriniz.");

            RuleFor(x => x.GoogleTagManagerId)
                .Matches(@"^GTM-[A-Z0-9]+$").When(x => !string.IsNullOrEmpty(x.GoogleTagManagerId))
                .WithMessage("Geçerli bir Google Tag Manager ID giriniz.");

            RuleFor(x => x.RobotsText)
                .MaximumLength(2000).WithMessage("Robots.txt içeriği en fazla 2000 karakter olabilir.");
        }

        private bool BeValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
