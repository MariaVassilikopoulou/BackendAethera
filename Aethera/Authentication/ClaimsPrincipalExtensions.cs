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
