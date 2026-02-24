using FluentValidation;
using Services.Dtos.Auth;

namespace Services.Validations;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo es obligatorio")
            .EmailAddress().WithMessage("Formato de correo inválido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria");
    }
}
