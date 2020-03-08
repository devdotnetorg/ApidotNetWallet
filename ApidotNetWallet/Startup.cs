using ApidotNetWallet.Models;
using ApidotNetWallet.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using static ApidotNetWallet.Helper.BaseHelper;
using System.Text;
using ApidotNetWallet.Services;

namespace ApidotNetWallet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //Change folder for appsettings.json
            var builder = new ConfigurationBuilder()
                .AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();
            //
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Add Controllers
                services.AddControllers()
                .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            
            //add support DataBase
            services.AddEntityFrameworkNpgsql().AddDbContext<WalletApiContext>(opt => {
                opt.UseLazyLoadingProxies()
                .UseNpgsql(Configuration.GetConnectionString("DBWebApiConection"));
                }, ServiceLifetime.Singleton);
            //Репозитарий
            services.AddSingleton<IUnitOfWork, UnitOfWork>();
            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            //Add Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // укзывает, будет ли валидироваться издатель при валидации токена
                            ValidateIssuer = true,
                            // строка, представляющая издателя
                            ValidIssuer = AuthenticationJWTService.ISSUER,
                            // будет ли валидироваться потребитель токена
                            ValidateAudience = true,
                            // установка потребителя токена
                            ValidAudience = AuthenticationJWTService.AUDIENCE,
                            // будет ли валидироваться время существования
                            ValidateLifetime = true,
                            // установка ключа безопасности
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                        // валидация ключа безопасности
                        ValidateIssuerSigningKey = true,
                        };
                    });
            // configure DI for application services
            services.AddScoped<IAuthenticateService, AuthenticationJWTService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
