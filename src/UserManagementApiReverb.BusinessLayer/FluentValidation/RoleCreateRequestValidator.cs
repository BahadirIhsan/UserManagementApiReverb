using FluentValidation;
using UserManagementApiReverb.BusinessLayer.DTOs.Role;
using UserManagementApiReverb.DataAccessLayer;

namespace UserManagementApiReverb.BusinessLayer.FluentValidation;

public class RoleCreateRequestValidator :  AbstractValidator<RoleCreateRequest>
{
    public RoleCreateRequestValidator(AppDbContext db)
    {
        RuleFor(r => r.RoleName)
            .NotEmpty().WithMessage("Role name is required")
            .MaximumLength(50).WithMessage("Role name cannot exceed 50 characters")
            .MinimumLength(3).WithMessage("Role name cannot exceed 3 characters")
            .Must( name => !db.Roles.Any(r => r.RoleName == name))
            .WithMessage("Role name is invalid");
    }
}