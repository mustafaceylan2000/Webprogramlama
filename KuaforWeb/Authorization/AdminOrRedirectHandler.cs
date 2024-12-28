using KuaforWeb.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace KuaforWeb.Authorization;

public class AdminOrRedirectHandler : AuthorizationHandler<AdminOrRedirectRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrRedirectRequirement requirement)
    {
        if (!context.User.Identity.IsAuthenticated)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}