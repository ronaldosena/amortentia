using System.Security.Claims;
using System.Threading.Tasks;
using backend.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Helpers
{
  public class LogUserActivity : IAsyncActionFilter
  {
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      var resultContext = await next();
      var username = resultContext.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
      var repo = resultContext.HttpContext.RequestServices.GetService<IUsersRepository>();
      await repo.UpdateLastActive(username);
    }
  }
}