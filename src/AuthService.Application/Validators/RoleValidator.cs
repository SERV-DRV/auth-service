using AuthService.Application.Validators;
using AuthService.Domain.Constants;

public class RoleValidator
{
    public bool IsValidRole(string role)
    {
        return RoleConstants.AllowedRoles.Contains(role);
    }
}