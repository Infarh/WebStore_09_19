using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebStore.Clients.Employees;
using WebStore.Clients.Orders;
using WebStore.Clients.Products;
using WebStore.Clients.Users;
using WebStore.Clients.Values;
using WebStore.DAL.Context;
using WebStore.Data;
using WebStore.Domain.Entities;
using WebStore.Infrastructure.Implementations;
using WebStore.Infrastructure.Interfaces;
using WebStore.Interfaces.Api;
using WebStore.Models;
using WebStore.Services.Data;

namespace WebStore
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration Configuration) => this.Configuration = Configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<WebStoreContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConection")));

            services.AddTransient<IValuesService, ValuesClient>();

            //services.AddTransient<WebStoreContextInitializer>();

            services.AddSingleton<IEmployeesData, EmployeesClient>();
            //services.AddSingleton<IEmployeesData, InMemoryEmployeesData>();
            //services.AddSingleton<IProductData, InMemoryProductData>();
            services.AddScoped<IProductData, ProductsClient>();
            //services.AddScoped<IProductData, SqlProductData>();
            services.AddScoped<ICartService, CookieCartService>();
            services.AddScoped<IOrderService, OrdersClient>();
            //services.AddScoped<IOrderService, SqlOrdersService>();

            services.AddIdentity<User, IdentityRole>(options =>
                {
                    // конфигурация cookies возможна здесь
                })
                //.AddEntityFrameworkStores<WebStoreContext>()
                .AddDefaultTokenProviders();

            #region Custom identity implementation

            //IUserStore<User>,
            //IUserRoleStore<User>,
            //IUserPasswordStore<User>,
            //IUserEmailStore<User>,
            //IUserPhoneNumberStore<User>,
            //IUserClaimStore<User>,
            //IUserTwoFactorStore<User>,
            //IUserLoginStore<User>,
            //IUserLockoutStore<User>

            services.AddTransient<IUserStore<User>, UsersClient>();
            services.AddTransient<IUserRoleStore<User>, UsersClient>();
            services.AddTransient<IUserPasswordStore<User>, UsersClient>();
            services.AddTransient<IUserEmailStore<User>, UsersClient>();
            services.AddTransient<IUserPhoneNumberStore<User>, UsersClient>();
            services.AddTransient<IUserClaimStore<User>, UsersClient>();
            services.AddTransient<IUserTwoFactorStore<User>, UsersClient>();
            services.AddTransient<IUserLoginStore<User>, UsersClient>();
            services.AddTransient<IUserLockoutStore<User>, UsersClient>();

            services.AddTransient<IRoleStore<IdentityRole>, RolesClient>();

            #endregion

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

            services.ConfigureApplicationCookie(cfg =>
            {
                cfg.Cookie.HttpOnly = true;
                cfg.Cookie.Expiration = TimeSpan.FromDays(150);
                cfg.Cookie.MaxAge = TimeSpan.FromDays(150);

                cfg.LoginPath = "/Account/Login";
                cfg.LogoutPath = "/Account/Logout";
                cfg.AccessDeniedPath = "/Account/AccessDenied";

                cfg.SlidingExpiration = true;
            });

            services.AddMvc(opt =>
            {
                //opt.Filters.Add<ActionFilter>();
                //opt.Conventions.Add(new TestConvention());
            });

            services.AddAutoMapper(opt =>
            {
                opt.CreateMap<Employee, Employee>();
            });

            //AutoMapper.Mapper.Initialize(opt =>
            //{
            //    opt.CreateMap<Employee, Employee>();
            //});
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env/*, WebStoreContextInitializer db*/)
        {
            //db.InitializeAsync().Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }

            app.UseStaticFiles();
            app.UseDefaultFiles();

            //app.UseWelcomePage("/Welcome");

            app.UseAuthentication();

            //app.UseMvcWithDefaultRoute(); // "default" : "{controller=Home}/{action=Index}/{id?}"
            app.UseMvc(route =>
            {
                route.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                route.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"/*,
                    defaults: new
                    {
                        controller = "Home", 
                        action = "Index",
                        id = (int?)null
                    }*/);
            });
        }
    }
}
