using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectWeb.Data;
using ProjectWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ProjectWeb.Areas.Identity.Data;
using Newtonsoft.Json;

namespace ProjectWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        
            services.AddRazorPages();
            services.AddHttpContextAccessor();
            services.AddServerSideBlazor();
            services.AddScoped<AppState>();
            services.AddScoped<ShoppingCartService>();
            services.AddDbContext<ProjectContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("ProjectWebContext")));
            services.Configure<Email>(Configuration.GetSection("Email"));
            services.AddSingleton<IMailer, MailHandler>();
            services.AddAuthorization(Options =>
            {
                Options.AddPolicy("Admin", policy =>  policy.RequireClaim("EmployeeNumber"));
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 7;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            CreateRoles(serviceProvider).GetAwaiter().GetResult();

            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapBlazorHub();
            });
        }
        // Method for creating an admin account and assign the admin role
        private  async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var Usermanager = serviceProvider.GetRequiredService<UserManager<ProjectWebUser>>();

            string Role = "Admin";
            IdentityResult RoleResult;

            var RoleExists = await RoleManager.RoleExistsAsync(Role);

            if (!RoleExists)
            {
                RoleResult = await RoleManager.CreateAsync(new IdentityRole(Role));
            }

            //Find admin account
            var User = await Usermanager.FindByEmailAsync("Admin@gmail.com");

            if(User == null)
            {
                var AdminAccount = new ProjectWebUser
                {
                    UserName = "Admin@gmail.com",
                    Email = "Admin@gmail.com",
                    EmailConfirmed = true
                    
                };
                string AdminPassword = "Admin56.1";
                var CreateAdmin = await Usermanager.CreateAsync(AdminAccount, AdminPassword);
                if (CreateAdmin.Succeeded)
                {
                    await Usermanager.AddToRoleAsync(AdminAccount, Role);
                }
            }
        }
        
    }
}
