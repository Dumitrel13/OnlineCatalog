using Microsoft.AspNetCore.Identity;
using OnlineCatalog.Models;

namespace OnlineCatalog.Middleware
{
    public class CheckTeacherAssignmentExpirationMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckTeacherAssignmentExpirationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                //var user = await userManager.GetUserAsync(context.User);
                
            }

            await _next(context);
        }
    }
}
