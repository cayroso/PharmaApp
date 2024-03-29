
using App.Services;
using Cayent.Core.CQRS.Services;
using Cayent.Core.Hubs;
using Data.App.DbContext;
using Data.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Web.Middlewares;

namespace Web
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
            services.AddDbContext<AppDbContext>(opt => { });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<RouteOptions>(opt =>
            {
                opt.LowercaseUrls = true;
                opt.AppendTrailingSlash = true;
            });

            services.AddLogging();

            services.AddSignalR().AddNewtonsoftJsonProtocol(options =>
            {
                //options.PayloadSerializerSettings.ContractResolver = new CamelCaseContractResolver();
                options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //options.PayloadSerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mmZ";
                //options.PayloadSerializerSettings.Culture = cultureInfo;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(ApplicationRoles.SystemsRoleName, policy =>
                   policy.RequireAssertion(context =>
                       context.User.HasClaim(c =>
                       c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == ApplicationRoles.SystemsRoleName
                           )));

                options.AddPolicy(ApplicationRoles.AdministratorRoleName, policy =>
                   policy.RequireAssertion(context =>
                       context.User.HasClaim(c =>
                       c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == ApplicationRoles.AdministratorRoleName
                           )));

                options.AddPolicy(ApplicationRoles.StaffRoleName, policy =>
                   policy.RequireAssertion(context =>
                       context.User.HasClaim(c =>
                       c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == ApplicationRoles.StaffRoleName
                           )));

                options.AddPolicy(ApplicationRoles.CustomerRoleName, policy =>
                   policy.RequireAssertion(context =>
                       context.User.HasClaim(c =>
                       c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == ApplicationRoles.CustomerRoleName
                           )));


                //options.AddPolicy(ApplicationPermissions.ManageUserTasks, policy =>
                //   policy.RequireAssertion(context =>
                //       context.User.HasClaim(c =>
                //       c.Type == System.Security.Claims.ClaimTypes.Role &&
                //        (c.Value == ApplicationRoles.ManagerRoleName || c.Value == ApplicationRoles.AssistantRoleName)
                //           )));
            });


            var mvcBuilder = services.AddRazorPages(opt =>
            {

            }).AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            .AddRazorPagesOptions(opt =>
            {
                opt.Conventions.AuthorizeAreaFolder(ApplicationRoles.SystemsRoleName, "/", ApplicationRoles.SystemsRoleName);
                opt.Conventions.AuthorizeAreaFolder(ApplicationRoles.AdministratorRoleName, "/", ApplicationRoles.AdministratorRoleName);
                opt.Conventions.AuthorizeAreaFolder(ApplicationRoles.StaffRoleName, "/", ApplicationRoles.StaffRoleName);
                opt.Conventions.AuthorizeAreaFolder(ApplicationRoles.CustomerRoleName, "/", ApplicationRoles.CustomerRoleName);
            });



            //services.AddTransient<Hub<IUserTaskHubClient>, UserTaskHub>();
            //services.AddTransient<UserTaskHubClient>();

            //services.AddTransient<Hub<IOrderHubClient>, OrderHub>();
            //services.AddTransient<OrderHubClient>();

            services.AddMvc(opt =>
            {
                opt.EnableEndpointRouting = false;
            });

#if DEBUG
            mvcBuilder.AddRazorRuntimeCompilation();
#endif

#if !DEBUG
            services.AddProgressiveWebApp();
#endif
            services.AddScoped<ChatService>();
            services.AddScoped<NotificationService>();

            services.AddTransient<ChatHub>();
            services.AddTransient<OrderHub>();

            services.AddScoped<AppBaseDbContext, AppDbContext>();

            StartupExtension.RegisterCQRS(services);
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value;

                ApplicationRoles.Items.ForEach(r =>
                {
                    var find = $"/{r.Id}/";

                    if (path.StartsWith(find))
                    {
                        context.Request.Path = find;
                    }
                });

                //if (context.Request.Path.Value.StartsWith("/administrator/"))
                //{
                //    context.Request.Path = "/administrator/";
                //}
                //else if (context.Request.Path.Value.StartsWith("/staff/"))
                //{
                //    context.Request.Path = "/staff/";
                //}
                //else if (context.Request.Path.Value.StartsWith("/customer/"))
                //{
                //    context.Request.Path = "/customer/";
                //}


                //context.Response.ContentType = "text/html";
                //await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));

                await next();
            });

            app.ConfigureCustomExceptionMiddleware();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();

                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapHub<OrderHub>("/orderHub");
            });
        }
    }
}
