using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            
            // Gets user id from security context in HttpContext
            var userId = int.Parse(resultContext.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier).Value);

            // Gets repository instance from IoC container
            var repo = resultContext.HttpContext.RequestServices
                .GetService<IDatingRepository>();

            // ARCH: Repository Interface Access -> Returns ORM model, but var hides the typing
            var user = await repo.GetUser(userId);
            user.LastActive = DateTime.Now;

            // ARCH: Repository Interface Access -> Save data to DB
            await repo.SaveAll();
        }
    }
}