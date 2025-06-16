using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using DDDSample1.Infrastructure;

using DDDSample1.Infrastructure.Shared;
using DDDSample1.Domain.Shared;

using DDDSample1.Domain.Users;
using Newtonsoft.Json.Serialization;
using DDDSample1.Infrastructure.Users;
using DDDSample1.Infrastructure.HospitalStaff;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Specializations;
using DDDSample1.Infrastructure.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.AppServices;
using DDDSample1.Infrastructure.Tokens;
using DDDSample1.Domain.Doctors;
using DDDSample1.Infrastructure.Doctors;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Infrastructure.ContactInformations;
using Microsoft.AspNetCore.Authentication.Cookies;
using DDDSample1.Domain.LoginAttemptTrackers;
using DDDSample1.Infrastructure.LoginAttemptTrackers;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Infrastructure.HospitalPatient;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Infrastructure.HospitalAppointment;
using DDDSample1.Domain.OperationTypes;
using DDDSample1.Infrastructure.OperationTypes;
using Microsoft.Extensions.DependencyInjection.Extensions;
using DDDSample1.Domain.RequiredSpecialists;
using DDDSample1.Infrastructure.RequiredSpecialists;
using DDDSample1.Domain.OperationPhases;
using DDDSample1.Infrastructure.OperationPhases;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.OperationRequests;
using DDDSample1.Infrastructure.OperationRequests;
using DDDSample1.Domain.AvailabilitySlots;
using DDDSample1.Infrastructure.AvailabilitySlots;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Infrastructure.OperationRooms;
using DDDSample1.Domain.OperationRoomTypes;
using DDDSample1.Infrastructure.OperationRoomTypes;
using DDDSample1.AppServices.OperationRoomTypes;
using System;
using Microsoft.Extensions.Logging;
using DDDSample1.Infrastructure.AssignedStaffs;
using DDDSample1.Domain.AssignedStaffs;

namespace DDDSample1
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
			// To use local sqlite db uncomment this code
			services.AddDbContext<HospitalDbContext>(opt =>
				opt.UseSqlite("Data Source=hospitalDb.db")
				.ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>()
			);

			// and comment this one
			/* services.AddDbContext<HospitalDbContext>(opt =>
				// opt.UseMySql("ConnectionStrings:HospitalDb",
				opt.UseMySql(Configuration.GetConnectionString("HospitalDb"),
				new MySqlServerVersion(new Version(8, 0, 40)))
				// these 3 lines can be removed, used only for debug purposes
				.LogTo(Console.WriteLine, LogLevel.Information)
				.EnableSensitiveDataLogging()
				.EnableDetailedErrors()
				.ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>()
			); */

			ConfigureMyServices(services);

			services.AddControllers().AddNewtonsoftJson(options =>
		{
			options.SerializerSettings.ContractResolver = new DefaultContractResolver();
		});

			// Add Swagger services
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Hospital Management API", Version = "v1" });
			});

			// Google Authentication services
			services.AddAuthentication(options =>
			{
				options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
			})
			.AddCookie()
			.AddGoogle(options =>
			{
				//Information in appsettings.json. If theres a problem with the commits, update the information to be here directly.
				options.ClientId = Configuration["GoogleKeys:ClientId"];
				options.ClientSecret = Configuration["GoogleKeys:ClientSecret"];
			});
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
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseRouting();

			// Enable middleware to serve generated Swagger as a JSON endpoint
			app.UseSwagger();

			app.UseCors("AllowSpecificOrigin");

			// Enable middleware to serve Swagger-UI (HTML, JS, CSS, etc.),
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "DDDSample1 API V1");
				c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root (http://localhost:<port>/)
			});

			// Add authentication and authorization middleware
			app.UseAuthentication();  // Line to ensure the authentication middleware is in the pipeline
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		public void ConfigureMyServices(IServiceCollection services)
		{
			services.AddCors(options =>
			{
				options.AddPolicy("AllowSpecificOrigin", builder =>
				{
					builder.AllowAnyOrigin()
						.AllowAnyHeader()
						.AllowAnyMethod(); // Permite métodos GET, POST, PUT, DELETE, etc.
				});
			});
			services.AddTransient<IUnitOfWork, UnitOfWork>();


			services.AddTransient<IUserRepository, UserRepository>();
			services.AddTransient<UserService>();

			services.AddTransient<IStaffRepository, StaffRepository>();
			services.AddTransient<StaffService>();

			services.AddTransient<IPatientRepository, PatientRepository>();
			services.AddTransient<PatientService>();

			services.AddTransient<IAppointmentRepository, AppointmentRepository>();
			services.AddTransient<AppointmentService>();

			services.AddTransient<ISpecializationRepository, SpecializationRepository>();
			services.AddTransient<SpecializationService>();

			services.AddTransient<ITokenRepository, TokenRepository>();
			services.AddTransient<TokenService>();

			services.AddTransient<PasswordActivationService>();

			services.AddTransient<IDoctorRepository, DoctorRepository>();
			services.AddTransient<DoctorService>();


			services.AddTransient<IAvailabilitySlotsRepository, AvailabilitySlotsRepository>();

			services.AddTransient<IContactInformationRepository, ContactInformationRepository>();

			services.AddTransient<ILoginAttemptTrackerRepository, LoginAttemptTrackerRepository>();
			services.AddTransient<LoginService>();

			services.AddTransient<ILogRepository, LogRepository>();

			services.AddTransient<IRequiredSpecialistRepository, RequiredSpecialistRepository>();

			services.AddTransient<IAssignedStaffRepository, AssignedStaffRepository>();
			
			services.AddTransient<IOperationPhaseRepository, OperationPhaseRepository>();

			services.AddTransient<IOperationTypeRepository, OperationTypeRepository>();
			services.AddTransient<OperationTypeService>();

			services.AddTransient<IOperationRequestRepository, OperationRequestRepository>();
			services.AddTransient<OperationRequestService>();

			services.AddTransient<IOperationRoomRepository, OperationRoomRepository>();

			services.AddTransient<IOperationRoomTypeRepository, OperationRoomTypeRepository>();
			services.AddTransient<OperationRoomTypeService>();

			services.AddTransient<LogService>();
			services.AddHostedService<LegalDeletionCheckService>();
			services.AddScoped<UpdateInformationService>();
		}
	}
}
