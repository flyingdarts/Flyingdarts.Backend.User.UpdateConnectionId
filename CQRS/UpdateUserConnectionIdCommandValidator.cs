using FluentValidation;

public class UpdateUserConnectionIdCommandValidator : AbstractValidator<UpdateUserConnectionIdCommand>
{
    public UpdateUserConnectionIdCommandValidator()
    {
        RuleFor(x=>x.UserId).NotEmpty().NotNull();
    }
}