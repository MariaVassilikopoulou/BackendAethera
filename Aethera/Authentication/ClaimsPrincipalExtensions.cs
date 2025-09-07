using System.Security.Claims;

namespace Aethera.Authentication
{
    internal static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the User ID (NameIdentifier) from the ClaimsPrincipal.
        /// </summary>
        /// <param name="principal">The current ClaimsPrincipal (User).</param>
        /// <returns>User ID as string.</returns>
        /// <exception cref="ApplicationException">Thrown if user ID is not found.</exception>
        public static string GetUserId(this ClaimsPrincipal? principal)
        {
            return principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new ApplicationException("User ID is unavailable from claims.");
        }

        /// <summary>
        /// Gets the user's email if available.
        /// </summary>
        public static string? GetEmail(this ClaimsPrincipal? principal)
        {
            return principal?.FindFirstValue(ClaimTypes.Email);
        }

        /// <summary>
        /// Gets the user's display name if available.
        /// </summary>
        public static string? GetName(this ClaimsPrincipal? principal)
        {
            return principal?.FindFirstValue(ClaimTypes.Name);
        }
    }
}


//using System.Security.Claims;

//namespace Aethera.Authentication
//{
//    internal static class ClaimsPrincipalExtensions
//    {
//        /// <summary>
//        /// Gets the User ID (Object ID from Azure AD) from the ClaimsPrincipal.
//        /// This uses the "oid" claim to match what the frontend extracts from the token.
//        /// </summary>
//        /// <param name="principal">The current ClaimsPrincipal (User).</param>
//        /// <returns>User ID as string.</returns>
//        /// <exception cref="ApplicationException">Thrown if user ID is not found.</exception>
//        public static string GetUserId(this ClaimsPrincipal? principal)
//        {
//            // Try "oid" first (Azure AD Object ID), then fall back to standard claims
//            return principal?.FindFirstValue("oid")
//                ?? principal?.FindFirstValue(ClaimTypes.NameIdentifier)
//                ?? principal?.FindFirstValue("sub")
//                ?? throw new ApplicationException("User ID is unavailable from claims.");
//        }

//        /// <summary>
//        /// Gets the user's email if available.
//        /// </summary>
//        /// <param name="principal">The current ClaimsPrincipal (User).</param>
//        /// <returns>Email as string or null if not found.</returns>
//        public static string? GetEmail(this ClaimsPrincipal? principal)
//        {
//            return principal?.FindFirstValue(ClaimTypes.Email)
//                ?? principal?.FindFirstValue("email")
//                ?? principal?.FindFirstValue("preferred_username");
//        }

//        /// <summary>
//        /// Gets the user's display name if available.
//        /// </summary>
//        /// <param name="principal">The current ClaimsPrincipal (User).</param>
//        /// <returns>Name as string or null if not found.</returns>
//        public static string? GetName(this ClaimsPrincipal? principal)
//        {
//            return principal?.FindFirstValue(ClaimTypes.Name)
//                ?? principal?.FindFirstValue("name")
//                ?? principal?.FindFirstValue(ClaimTypes.GivenName);
//        }

//        /// <summary>
//        /// Gets all available claims for debugging purposes.
//        /// </summary>
//        /// <param name="principal">The current ClaimsPrincipal (User).</param>
//        /// <returns>Dictionary of claim types and values.</returns>
//        public static Dictionary<string, string> GetAllClaims(this ClaimsPrincipal? principal)
//        {
//            return principal?.Claims?.ToDictionary(c => c.Type, c => c.Value)
//                ?? new Dictionary<string, string>();
//        }
//    }
//}