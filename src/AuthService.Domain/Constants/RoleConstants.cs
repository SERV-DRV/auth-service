namespace AuthService.Domain.Constants
{
    public class Constants
    {
        public const string ADMIN_ROLE = "ADMIN_ROLE";
        public const string USER_ROLE = "USER_ROLE";

        // This array can be used to validate roles when creating or updating users
        public static readonly string[] AllowedRoles = { ADMIN_ROLE, USER_ROLE };
    }
}
