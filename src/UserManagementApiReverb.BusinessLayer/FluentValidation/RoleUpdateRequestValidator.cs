using FluentValidation;
using UserManagementApiReverb.BusinessLayer.DTOs.Role;

namespace UserManagementApiReverb.BusinessLayer.FluentValidation;

public class RoleUpdateRequestValidator :  AbstractValidator<RoleUpdateRequest>
{
    public RoleUpdateRequestValidator()
    {
        RuleFor(r => r.RoleName)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters")
            .MinimumLength(3).WithMessage("Name must not exceed 3 characters");
    }
}