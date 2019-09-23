using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebStore.DAL.Context;
using WebStore.Domain.Entities;
using WebStore.Infrastructure.Implementations;
using WebStore.Infrastructure.Interfaces;
using WebStore.Services.Data;

namespace WebStore.ServiceHosting
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<WebStoreContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConection")));

            services.AddTransient<WebStoreContextInitializer>();

            services.AddIdentity<User, IdentityRole>()
               .AddEntityFrameworkStores<WebStoreContext>()
               .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(cfg =>
            {
                cfg.Password.RequiredLength = 3;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredUniqueChars = 3;

                cfg.Lockout.MaxFailedAccessAttempts = 10;
                cfg.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                cfg.Lockout.AllowedForNewUsers = true;

                cfg.User.RequireUniqueEmail = false; // грабли!
            });

            services.AddSingleton<IEmployeesData, InMemoryEmployeesData>();
            //services.AddSingleton<IProductData, InMemoryProductData>();
            services.AddScoped<IProductData, SqlProductData>();
            services.AddScoped<ICartService, CookieCartService>();
            services.AddScoped<IOrderService, SqlOrdersService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, WebStoreContextInitializer db)
        {
            db.InitializeAsync().Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
