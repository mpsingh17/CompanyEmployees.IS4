using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using CompanyEmployees.Client.Handlers;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace CompanyEmployees.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<BearerTokenHandler>();

            services.AddHttpClient("APIClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            })
            .AddHttpMessageHandler<BearerTokenHandler>();

            services.AddHttpClient("IDPClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5005/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
            {
                opt.AccessDeniedPath = "/Auth/AccessDenied";
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opt =>
            {
                opt.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.Authority = "https://localhost:5005";
                opt.ClientId = "companyemployeeclient";
                opt.ClientSecret = "CompanyEmployeeClientSecret";
                opt.ResponseType = OpenIdConnectResponseType.Code;
                opt.SaveTokens = true;
                opt.GetClaimsFromUserInfoEndpoint = true;
                opt.Scope.Add("address");
                opt.Scope.Add("roles");
                opt.Scope.Add("country");
                opt.Scope.Add("companyemployeeapi");
                opt.Scope.Add("offline_access");

                opt.ClaimActions.MapUniqueJsonKey("role", "role");
                opt.ClaimActions.MapUniqueJsonKey("country", "country");

                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    RoleClaimType = JwtClaimTypes.Role
                };

                //opt.ClaimActions.MapUniqueJsonKey("nickname", "nickname");
                //opt.ClaimActions.MapUniqueJsonKey("address", "address");
                //opt.SignedOutCallbackPath = "/signout-callback-oidc";
            });

            services.AddAuthorization(authOpt =>
            {
                authOpt.AddPolicy("CanCreateAndModifyData", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireRole("Admin");
                    policyBuilder.RequireClaim("country", "USA");
                });
            });

            services.AddControllersWithViews();
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
