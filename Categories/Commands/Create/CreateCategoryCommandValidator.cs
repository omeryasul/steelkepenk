using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Categories.Commands.Create
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Kategori adı gereklidir.")
                .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");

            RuleFor(x => x.MetaTitle)
                .MaximumLength(60).WithMessage("Meta başlık en fazla 60 karakter olabilir.");

            RuleFor(x => x.MetaDescription)
                .MaximumLength(160).WithMessage("Meta açıklama en fazla 160 karakter olabilir.");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Sıralama değeri 0 veya daha büyük olmalıdır.");
        }
    }
}
