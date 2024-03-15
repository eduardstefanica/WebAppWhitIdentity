using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp8WhitIdentity.Models;
using WebAppWithIdentity.Data;

namespace WebAppWithIdentity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Add CORS
            var ReactSpecificOrigins = "enablecorsfromreact";

            var builder = WebApplication.CreateBuilder(args);

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: ReactSpecificOrigins,
                                       builder =>
                                       {
                                           builder.WithOrigins("http://localhost:3000")
                                               .AllowAnyHeader()
                                               .AllowAnyMethod();
                                       });
            });

            var connectionString = builder.Configuration.GetConnectionString("WebAppDevConnection");
            /// Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            // builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddIdentity<AppUser, IdentityRole>(
                options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredLength = 8;
                })
                .AddDefaultUI()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // PATRIZIO : Add the following line to enable the use of the [Authorize] attribute
            builder.Services.AddAuthorization(options => {
                options.AddPolicy("readpolicy",
                    builder => builder.RequireRole("Admin", "Manager", "User"));
                options.AddPolicy("writepolicy",
                    builder => builder.RequireRole("Admin", "Manager"));
            });

            var app = builder.Build();



            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // InvalidOperationException: Endpoint WebApp8WithIdentity.Controllers.AccountController.Login(WebApp8WithIdentity)
            // contains CORS metadata, but a middleware was not found that supports CORS.
            // Configure your application startup by adding app.UseCors() in the application startup code.
            // If there are calls to app.UseRouting() and app.UseEndpoints(...), the call to app.UseCors() must go between them.

            app.UseRouting();

            // CORS
            app.UseCors(ReactSpecificOrigins);

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //    endpoints.MapControllers();
            //});

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
