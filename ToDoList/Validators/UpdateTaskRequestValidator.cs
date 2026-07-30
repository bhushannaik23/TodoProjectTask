namespace ToDoList.Validators;
using FluentValidation;
using ToDoList.DTOs;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow);

        RuleFor(x => x.StatusId)
            .GreaterThan(0);
    }
}
