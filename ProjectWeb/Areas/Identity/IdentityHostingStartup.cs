using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectWeb.Areas.Identity.Data;
using ProjectWeb.Data;

[assembly: HostingStartup(typeof(ProjectWeb.Areas.Identity.IdentityHostingStartup))]
namespace ProjectWeb.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<ProjectWebContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("ProjectWebContext")));

                services.AddDefaultIdentity<ProjectWebUser>(options => options.SignIn.RequireConfirmedAccount = true)
                     .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ProjectWebContext>();
            });
        }
    }
}