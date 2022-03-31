using System.Security.Claims;

namespace ReversiMvcApp.Repositories
{
    public static class SecurityCheck
    {
        public static bool LoginCheck(ClaimsPrincipal user)
        {
            if (user?.FindFirst(ClaimTypes.NameIdentifier)?.Value != null)
                return true;
            return false;
        }
    }
}
