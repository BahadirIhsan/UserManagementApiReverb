using FluentValidation;
using UserManagementApiReverb.BusinessLayer.DTOs.User;
using UserManagementApiReverb.DataAccessLayer;

namespace UserManagementApiReverb.BusinessLayer.FluentValidation;

public class UserRequestRegisterValidator : AbstractValidator<UserRequestRegister>
{
    private readonly AppDbContext _db;
    public UserRequestRegisterValidator(AppDbContext db)
    {
        _db = db;
        
        RuleFor(u => u.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long")
            .MaximumLength(50).WithMessage("Username cannot exceed 50 characters")
            .Must(username => !_db.Users.Any(u => u.UserName == username))
            .WithMessage("This UserName already exists");

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email format is invalid")
            .Must(email => !_db.Users.Any(u => u.Email.ToLower() == email.ToLower()))
            .WithMessage("This email address already exists");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least 1 uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least 1 lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least 1 number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least 1 special character");

        RuleFor(u => u.FirstName)
            .NotEmpty().WithMessage("First name is required");

        RuleFor(u => u.LastName)
            .NotEmpty().WithMessage("Last name is required");
    }
}