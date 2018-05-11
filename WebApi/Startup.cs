﻿using System;
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
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
