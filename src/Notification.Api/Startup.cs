 using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Notification.Application.Service;
using Notification.Application.Service.Interface;
using Notification.Infrastructure.Repository;
using Notification.Infrastructure.Repository.Interface;
using Notification.Infrastructure.Security;
using Swashbuckle.AspNetCore.Swagger;
using Notification.Api.ExceptionHandling;
using Notification.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Notification.Infrastructure.MQSettings;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Notification.Api
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        private readonly IHostingEnvironment _HostingEnvironment;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="enviroment"></param>
        public Startup(IConfiguration configuration, IHostingEnvironment enviroment)
        {
            Configuration = configuration;
            _HostingEnvironment = enviroment;
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddCors();
            AddJwtAuthorization(services);
            services.AddApiVersioning();

            UpdateDatabase();

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddEntityFrameworkInMemoryDatabase().AddDbContext<NotificationContext>((sp, options) =>
            {
                options.UseInMemoryDatabase().UseInternalServiceProvider(sp);
            });

            services.AddHostedService<ConsumeSendMessageMQService>();
            services.AddHostedService<ConsumeGetMessagesMQService>(); 

             var rebbitSettings = new RabbitMQSettings();

            new ConfigureFromConfigurationOptions<RabbitMQSettings>(
                Configuration.GetSection("RabbitMQSettings")
                ).Configure(rebbitSettings);

            services.AddSingleton(rebbitSettings);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "Notification API",
                        Version = "v1",
                        Description = "API responsible for generate notifications ",
                        Contact = new Contact
                        {
                            Name = "Thiago Cardoso de Oliveira",
                        }

                    });

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Entre com o token JWT",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", Enumerable.Empty<string> () }
                });

                string xmlFile = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddAuthentication();
        }

        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API NOTIFICATION");
                c.RoutePrefix = string.Empty;
            });

            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);

            app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));
            app.UseMvc();
        }

        /// <summary>
        /// AddJwtAuthorization
        /// </summary>
        /// <param name="services"></param>
        public void AddJwtAuthorization(IServiceCollection services)
        {
            var signinConfigurations = new SigninConfigurations(Configuration);
            services.AddSingleton(signinConfigurations);

            var tokenConfigurations = new TokenConfiguration();

            new ConfigureFromConfigurationOptions<TokenConfiguration>(
                Configuration.GetSection("TokenConfigurations")
                ).Configure(tokenConfigurations);

            services.AddSingleton(tokenConfigurations);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = tokenConfigurations.Audience,
                ValidIssuer = tokenConfigurations.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigurations.SecurityKey)),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                ValidateAudience = true,
                ValidateIssuer = true,
                ClockSkew = TimeSpan.Zero,
                AuthenticationType = "Bearer"
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(bearerOptions =>
            {
                bearerOptions.SaveToken = true;
                bearerOptions.TokenValidationParameters = tokenValidationParameters;
                bearerOptions.RequireHttpsMetadata = true;

                bearerOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });

            // Ativa o uso do token como forma de autorizar o acesso
            // a recursos deste projeto
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });
        }

        private void UpdateDatabase()
        {
            var options = new DbContextOptionsBuilder<NotificationContext>()
              .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
              .UseInMemoryDatabase(Guid.NewGuid().ToString())
              .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
              .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
              .Options;
    
            using (var context = new NotificationContext(options))
            {
                if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
