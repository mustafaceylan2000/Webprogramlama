using KuaforWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
//using Microsoft.AspNetCore.E;
using KuaforWeb.Authorization;
using Microsoft.EntityFrameworkCore;
using KuaforWeb.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    // Global authorization policy - requires authenticated user for all endpoints
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); //require login after 60 minutes
    });
builder.Services.AddSingleton<IAuthorizationHandler, AdminOrRedirectHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOrRedirect", policy =>
        policy.Requirements.Add(new AdminOrRedirectRequirement()));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

app.Use(async (context, next) =>
{
    var endpoint = context.GetEndpoint();
    if (endpoint?.Metadata?.GetMetadata<IAuthorizeData>() != null)
    {
        var authService = context.RequestServices.GetRequiredService<IAuthorizationService>();
        var result = await authService.AuthorizeAsync(context.User, "AdminOrRedirect");
        if (!result.Succeeded) //authorising the service: if user admin result is successfull, or not.
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                await context.ChallengeAsync(); //return back to login screen
                return;
            }
            else
            {
                context.Response.Redirect("/Randevus/Index"); //if not admin, return back to randevu page
                return;
            }
        }
    }

    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();















app.Run();
