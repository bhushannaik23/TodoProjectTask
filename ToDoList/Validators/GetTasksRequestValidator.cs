using FluentValidation;
using ToDoList.DTOs;

namespace ToDoList.Validators
{
    public class GetTasksRequestValidator : AbstractValidator<GetTasksRequest>
    {
        public GetTasksRequestValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);
        }
    }
}
