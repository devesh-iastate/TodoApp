namespace TodoApp.Middleware;

// Middleware/FirebaseAuthMiddleware.cs
using FirebaseAdmin.Auth;
using System.Security.Claims;

public class FirebaseAuthMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader["Bearer ".Length..].Trim();

            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid),
                };

                if (decodedToken.Claims.TryGetValue("email", out var email))
                {
                    claims.Add(new Claim(ClaimTypes.Email, email.ToString()));
                }

                var identity = new ClaimsIdentity(claims, "Firebase");
                context.User = new ClaimsPrincipal(identity);
            }
            catch
            {
                // ignored
            }
        }

        await next(context);
    }
}
