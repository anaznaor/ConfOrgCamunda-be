using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using System.Net;
using Microsoft.Extensions.Logging;
using WebAPI.Interface;
using WebAPI.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebAPI.Services;
using AutoMapper;
using WebAPI.Controllers;
using WebAPI.Util.TaskHandlers;
using Camunda.Worker;
using Camunda.Worker.Client;

namespace WebAPI
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

            services.AddHttpClient();
            //Logging for application
            services.AddLogging();
            services.AddOptions();
            // Enable usage of HttpContext in services
            services.AddHttpContextAccessor();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            var connectionString = Configuration.GetConnectionString("ConfOrg");
            services.AddDbContext<DbContext, SqlServerApplicationDbContext>(opt =>
                    opt.UseSqlServer(connectionString)
                       .EnableSensitiveDataLogging()
                       .EnableDetailedErrors()
            );


            //Unit of work setup
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            services.AddScoped(typeof(IConferenceService), typeof(ConferenceService));
            services.AddScoped(typeof(IPaperService), typeof(PaperService));
            services.AddScoped(typeof(IRegistrationService), typeof(RegistrationService));
            services.AddScoped(typeof(IUserConfService), typeof(UserConfService));


            //services.AddControllers()
            //    .AddJsonOptions(o =>
            //        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
            //    )
            //    .AddJsonOptions(o =>
            //        o.JsonSerializerOptions.MaxDepth = 128
            //    )
            //;

            //services.AddAutoMapper(typeof(AutoMapper));
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperConf());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ConfOrg", Version = "v1" });
            });

            services.AddCamundaWorker("ConfOrgWorker")
                .AddHandler<SendReminderTaskHandler>()
                .AddHandler<CalcAvgGradeTaskHandler>()
                .AddHandler<EndOfConferenceTaskHandler>()
                .AddHandler<PaperSubmissionDeadlineTaskHandler>()
                .AddHandler<RegistrationDeadlineTaskHandler>()
                .AddHandler<ReviewNotifyTaskHandler>()
                .AddHandler<SendReviewReminderTaskHandler>();

            services.AddExternalTaskClient(client =>
            {
                client.BaseAddress = new Uri("http://localhost:8080/engine-rest");
                client.Timeout = TimeSpan.FromMinutes(1);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper, DbContext dataContext)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            dataContext.Database.Migrate();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConfOrg v1"));
            mapper.ConfigurationProvider.CompileMappings();
            try
            {
                mapper.ConfigurationProvider.AssertConfigurationIsValid();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseAuthorization();
            // Enable CORS
            app.UseCors("AllowAllOrigins");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().AllowAnonymous();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{user}/{controller=Camunda}/{action}/{id?}");
            });
        }
    }
}