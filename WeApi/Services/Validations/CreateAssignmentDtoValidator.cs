using FluentValidation;
using Services.Dtos.Assignments;

namespace Services.Validations;

public class CreateAssignmentDtoValidator : AbstractValidator<CreateAssignmentDto>
{
    private static readonly string[] ValidCategories =
    [
        "Trabajo en equipo", "Innovación", "Liderazgo", "Colaboración",
        "Compromiso", "Excelencia", "Actitud positiva", "Comunicación efectiva"
    ];

    public CreateAssignmentDtoValidator()
    {
        RuleFor(x => x.ToUserId)
            .GreaterThan(0).WithMessage("Debes seleccionar un destinatario");

        RuleFor(x => x.Points)
            .InclusiveBetween(1, 10).WithMessage("Los puntos deben estar entre 1 y 10");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("La categoría es obligatoria")
            .Must(c => ValidCategories.Contains(c)).WithMessage("Categoría inválida");

        RuleFor(x => x.Message)
            .MaximumLength(500).WithMessage("El mensaje no puede superar los 500 caracteres")
            .When(x => x.Message != null);
    }
}
