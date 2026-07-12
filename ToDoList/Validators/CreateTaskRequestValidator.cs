using FluentValidation;
using ToDoList.DTOs;

namespace ToDoList.Validators
{
    public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
    {
        public CreateTaskRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.DueDate)
                .Must(date => date == null || date >= DateTime.Today)
                .WithMessage("Due date cannot be in the past.");
        }
    }
}
