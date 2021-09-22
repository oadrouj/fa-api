using Abp.Authorization;
using Facturi.Authorization.Roles;
using Facturi.Authorization.Users;

namespace Facturi.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
