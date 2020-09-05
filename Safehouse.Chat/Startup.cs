using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Safehouse.Repository;
using Safehouse.Repository.Interfaces;
using Safehouse.Repository.MySql;
using Safehouse.Service;

namespace Safehouse.Chat
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();

            var chatMySqlRepo = this.Configuration.GetConnectionString("GrizzlyMySql");

            services.AddScoped<IUserRepository, UserMySqlRepository>(x =>
                new UserMySqlRepository(chatMySqlRepo)
            );

            services.AddScoped<IChatGroupRepository, ChatGroupMySqlRepository>(x =>
                new ChatGroupMySqlRepository(chatMySqlRepo)
            );

            services.AddScoped<IMessageRepository, MessageMySqlRepository>(x =>
                new MessageMySqlRepository(chatMySqlRepo)
            );

            services.AddScoped<IChatGroupMembershipRepository, ChatGroupMembershipMySqlRepository>(x =>
                new ChatGroupMembershipMySqlRepository(chatMySqlRepo)
            );

            services.AddScoped<IChatGroupChannelRepository, ChatGroupChannelMySqlRepository>(x =>
               new ChatGroupChannelMySqlRepository(chatMySqlRepo)
           ); 


            services.AddScoped<IChatGroupService, ChatGroupService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IChatGroupService, ChatGroupService>();



            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/Users/LogIn";
                options.LogoutPath = "/User/LogOut";
            });

            
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
 
            app.UseHttpsRedirection();
            app.UseStaticFiles();
             
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
 
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chathub");

                endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "group/{id}/{channelId?}",
                  new { Controller = "Group", Action = "Index" });
                 
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "users/{id}",
                  new { Controller = "Users", Action = "Index" });
            });
        }
    }
}
