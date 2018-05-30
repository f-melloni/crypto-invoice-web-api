using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebApi.Database.Entities;
using WebApi.Services;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Diagnostics;
using WebApi.Services.Interfaces;
using WebApi.Models;
using WebApi.Adapters;
using WebApi.Adapters.Interfaces;

namespace WebApi
{
    public class Startup
    {
        private static IConfiguration Configuration { get; set; }
        private IHostingEnvironment CurrentEnv { get; set; }
        public static string ConnectionString { get; set; }
        
        public Startup(IConfiguration configuration,IHostingEnvironment env)
        {
            Configuration = configuration;
            
            CurrentEnv = env;
            ConnectionString = CurrentEnv.IsDevelopment()
               ? Configuration.GetConnectionString("DefaultConnection")
               : Configuration.GetConnectionString("ReleaseConnection");
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DBEntities>(options => options.UseMySql(ConnectionString, b => b.MigrationsAssembly("WebApi")));
            services.AddIdentity<User, IdentityRole>(config =>
            {
            }).AddEntityFrameworkStores<DBEntities>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            CurrencyConfiguration currencyConfiguration = Configuration.GetSection("CurrencyConfiguration").Get<CurrencyConfiguration>();
            services.AddSingleton(currencyConfiguration);
            
            foreach(CurrencyConfigurationItem item in currencyConfiguration.Supported) {
                string CC = item.CurrencyCode.ToUpper();
                Type T = Type.GetType($"WebApi.Adapters.{CC}Adapter");
                currencyConfiguration.Adapters.Add(CC, (ICurrencyAdapter)Activator.CreateInstance(T));
            }
            
            ////add fb +ggl oath

            ////FB
            //services.AddAuthentication(options=> {
            //    options.DefaultChallengeScheme = FacebookDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            //}).AddFacebook(options =>
            //{
            //    options.AppId = "";
            //    options.AppSecret = "";
            //}).AddCookie();

            ////GGL
            //services.AddAuthentication(options => {
            //    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            //}).AddGoogle(options =>
            //{
            //    options.ClientId = "";
            //    options.ClientSecret = "";
            //}).AddCookie();

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            }));


            services.AddTransient<IEmailSender, EmailSender>();
            services.AddMvc();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseExceptionHandler("/api/Error/HandlingException");

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            });

            // Check if Starup is invoked by entityFramework and if so we can't continue because of infinite loop in Observer
            StackTrace stackTrace = new StackTrace();
            List<string> efMethods = new List<string>() { "RemoveMigration", "AddMigration", "UpdateDatabase" };
            if (stackTrace.GetFrames().Any(f => efMethods.Contains(f.GetMethod().Name))) {
                return;
            }

            RabbitMessenger.Setup(Configuration);
        }
    }
}
