using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.Commands.Update
{
    public class UpdateContentCommandValidator : AbstractValidator<UpdateContentCommand>
    {
        public UpdateContentCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Geçerli bir içerik ID'si giriniz.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık alanı zorunludur.")
                .MaximumLength(200).WithMessage("Başlık en fazla 200 karakter olabilir.");

            RuleFor(x => x.Summary)
                .NotEmpty().WithMessage("Özet alanı zorunludur.")
                .MaximumLength(500).WithMessage("Özet en fazla 500 karakter olabilir.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("İçerik alanı zorunludur.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Geçerli bir içerik türü seçiniz.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Geçerli bir durum seçiniz.");

            RuleFor(x => x.Slug)
                .MaximumLength(250).WithMessage("Slug en fazla 250 karakter olabilir.")
                .When(x => !string.IsNullOrEmpty(x.Slug));

            RuleFor(x => x.ImageUrl)
                .Must(BeValidUri).WithMessage("Geçerli bir resim URL'si giriniz.")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));

            RuleFor(x => x.MetaTitle)
                .MaximumLength(160).WithMessage("Meta başlık en fazla 160 karakter olabilir.")
                .When(x => !string.IsNullOrEmpty(x.MetaTitle));

            RuleFor(x => x.MetaDescription)
                .MaximumLength(320).WithMessage("Meta açıklama en fazla 320 karakter olabilir.")
                .When(x => !string.IsNullOrEmpty(x.MetaDescription));

            RuleFor(x => x.MetaKeywords)
                .MaximumLength(500).WithMessage("Meta anahtar kelimeler en fazla 500 karakter olabilir.")
                .When(x => !string.IsNullOrEmpty(x.MetaKeywords));

            RuleFor(x => x.CanonicalUrl)
                .Must(BeValidUri).WithMessage("Geçerli bir canonical URL giriniz.")
                .When(x => !string.IsNullOrEmpty(x.CanonicalUrl));

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Sıralama değeri 0 veya pozitif olmalıdır.");
        }

        private bool BeValidUri(string? uri)
        {
            return Uri.TryCreate(uri, UriKind.Absolute, out _);
        }
    }
}
