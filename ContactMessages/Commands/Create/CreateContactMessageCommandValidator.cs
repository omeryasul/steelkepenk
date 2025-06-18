using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Commands.Create
{
    public class CreateContactMessageCommandValidator : AbstractValidator<CreateContactMessageCommand>
    {
        public CreateContactMessageCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad gereklidir.")
                .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad gereklidir.")
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta gereklidir.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
                .MaximumLength(100).WithMessage("E-posta en fazla 100 karakter olabilir.");

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Telefon en fazla 20 karakter olabilir.");

            RuleFor(x => x.Company)
                .MaximumLength(100).WithMessage("Şirket adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Konu gereklidir.")
                .MaximumLength(200).WithMessage("Konu en fazla 200 karakter olabilir.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Mesaj gereklidir.")
                .MaximumLength(2000).WithMessage("Mesaj en fazla 2000 karakter olabilir.");
        }
    }
}
