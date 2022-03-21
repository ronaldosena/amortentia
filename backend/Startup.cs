using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using backend.Data;
using backend.Helpers;
using backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace backend
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
      services.AddCors(options =>
      {
        options.AddPolicy("CorsPolicy",
                  builder => builder.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials());
      });

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      services.Configure<Settings>(options =>
      {
        options.ConnectionString
                  = Configuration.GetSection("MongoConnection:ConnectionString").Value;
        options.Database
                  = Configuration.GetSection("MongoConnection:Database").Value;
      });

      services.AddAutoMapper();
      services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

      /* Adding Repository pattern to application */
      // Not persistent
      // services.AddTransient<IValueRepository, ValueRepository>();
      // This is persistent!!!
      services.AddScoped<IAuthRepository, AuthRepository>();
      services.AddScoped<IUsersRepository, UsersRepository>();
      services.AddScoped<LogUserActivity>();

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
      {
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
          ValidateIssuer = false,
          ValidateAudience = false
        };
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler(builder =>
        {
          builder.Run(async context =>
          {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var error = context.Features.Get<IExceptionHandlerFeature>();
            if (error != null)
            {
              context.Response.AddApplicationError(error.Error.Message);
              await context.Response.WriteAsync(error.Error.Message);
            }
          });
        });
        // app.UseHsts();
      }

      app.UseCors("CorsPolicy");
      app.UseForwardedHeaders(new ForwardedHeadersOptions
      {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
      });
      app.UseAuthentication();
      app.UseDefaultFiles();
      app.UseStaticFiles();
      app.UseMvc(routes =>
      {
        routes.MapSpaFallbackRoute(
            name: "spa-fallback",
            defaults: new { controller = "Fallback", action = "Index" }
        );
      });
    }
  }
}
