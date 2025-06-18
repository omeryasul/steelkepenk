using FluentValidation;

namespace Application.Features.Tags.Commands.Create
{
    public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
    {
        public CreateTagCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Etiket adı gereklidir.")
                .MaximumLength(100).WithMessage("Etiket adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");

            RuleFor(x => x.Color)
                .Matches(@"^#[0-9A-Fa-f]{6}$").When(x => !string.IsNullOrEmpty(x.Color))
                .WithMessage("Renk hex formatında olmalıdır (#FF5733).");
        }
    }
}