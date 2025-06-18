using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.Commands.Create
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ürün adı gereklidir.")
                .MaximumLength(200).WithMessage("Ürün adı en fazla 200 karakter olabilir.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Ürün açıklaması gereklidir.")
                .MaximumLength(5000).WithMessage("Ürün açıklaması en fazla 5000 karakter olabilir.");

            RuleFor(x => x.ShortDescription)
                .MaximumLength(500).WithMessage("Kısa açıklama en fazla 500 karakter olabilir.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Geçerli bir kategori seçiniz.");

            RuleFor(x => x.MetaTitle)
                .MaximumLength(60).WithMessage("Meta başlık en fazla 60 karakter olabilir.");

            RuleFor(x => x.MetaDescription)
                .MaximumLength(160).WithMessage("Meta açıklama en fazla 160 karakter olabilir.");

            RuleFor(x => x.DisplayPrice)
                .GreaterThanOrEqualTo(0).When(x => x.DisplayPrice.HasValue)
                .WithMessage("Fiyat 0 veya daha büyük olmalıdır.");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Sıralama değeri 0 veya daha büyük olmalıdır.");
        }
    }
}
