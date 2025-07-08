using FluentValidation;
using UserManagementApiReverb.BusinessLayer.DTOs.UserRole;
using UserManagementApiReverb.DataAccessLayer;

namespace UserManagementApiReverb.BusinessLayer.FluentValidation;

public class UserRoleAssignValidator : AbstractValidator<UserRoleAssign>
{
    public UserRoleAssignValidator(AppDbContext db)
    {
        RuleFor(x => x.UserId)
            .Must(userId => db.Users.Any(user => user.UserId == userId))
            .WithMessage("User does not exist");
        
        RuleFor(x => x.RoleId)
            .Must(roleId => db.Roles.Any(role => role.RoleId == roleId))
            .WithMessage("User does not exist");
    }
}