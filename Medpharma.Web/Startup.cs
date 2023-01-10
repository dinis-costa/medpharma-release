using Medpharma.Web.Data;
using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories;
using Medpharma.Web.Data.Repositories.Users;
using Medpharma.Web.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Vereyon.Web;

namespace Medpharma.Web
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
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(this.Configuration["Syncfusion:Key"]);
            services.AddRazorPages();

            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                cfg.SignIn.RequireConfirmedEmail = true;
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = true;
                cfg.Password.RequiredUniqueChars = 1;
                cfg.Password.RequireUppercase = true;
                cfg.Password.RequireLowercase = true;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 8;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<DataContext>();

            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = this.Configuration["Tokens:Issuer"],
                        ValidAudience = this.Configuration["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(this.Configuration["Tokens:Key"]))
                    };
                });

            services.AddDbContext<DataContext>(cfg => cfg.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<SeedDb>();

            //Entities
            services.AddScoped<IPriorityRepository, PriorityRepository>();
            services.AddScoped<ISpecialityRepository, SpecialityRepository>();
            services.AddScoped<IMedicalScreeningRepository, MedicalScreeningRepository>();
            services.AddScoped<IMedicineRepository, MedicineRepository>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IExamRepository, ExamRepository>();

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IAdmissionRepository, AdmissionRepository>();
            services.AddScoped<ITimeslotRepository, TimeslotRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            services.AddScoped<ICustomerFileRepository, CustomerFileRepository>();
            services.AddScoped<IClerkRepository, ClerkRepository>();

            //Helpers
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<IImageHelper, ImageHelper>();
            services.AddScoped<IConverterHelper, ConverterHelper>();
            services.AddScoped<IMailHelper, MailHelper>();
            services.AddScoped<ICheckoutHelper, CheckoutHelper>();
            services.AddScoped<IStripeHelper, StripeHelper>();

            services.AddScoped<IUserClaimsPrincipalFactory<User>, ApplicationUserClaimsPrincipalFactory>();


            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/NotAuthorized";
                options.AccessDeniedPath = "/Account/NotAuthorized";
            });

            services.AddFlashMessage();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
                context.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Error/{0}");

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

                endpoints.MapRazorPages();
            });
        }
    }
}
