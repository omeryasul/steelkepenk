using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContactMessages.Commands.Update
{
    public class ReplyToContactMessageCommandValidator : AbstractValidator<ReplyToContactMessageCommand>
    {
        public ReplyToContactMessageCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Geçerli bir mesaj ID'si gereklidir.");

            RuleFor(x => x.AdminReply)
                .NotEmpty().WithMessage("Yanıt mesajı gereklidir.")
                .MaximumLength(2000).WithMessage("Yanıt mesajı en fazla 2000 karakter olabilir.");
        }
    }
}
