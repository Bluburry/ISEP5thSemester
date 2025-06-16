using Microsoft.EntityFrameworkCore;

using DDDSample1.Domain.Users;
using DDDSample1.Infrastructure.Users;
using DDDSample1.Infrastructure.HospitalStaff;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.Specializations;
using DDDSample1.Infrastructure.Specializations;
using DDDSample1.Domain.Tokens;
using DDDSample1.Infrastructure.Tokens;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.Domain.Doctors;
using DDDSample1.Infrastructure.Doctors;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Infrastructure.ContactInformations;
using DDDSample1.Domain.LoginAttemptTrackers;
using DDDSample1.Infrastructure.LoginAttemptTrackers;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalAppointment;
using DDDSample1.Domain.OperationTypes;
using DDDSample1.Domain.OperationPhases;
using DDDSample1.Domain.RequiredSpecialists;
using DDDSample1.Infrastructure.OperationPhases;
using DDDSample1.Infrastructure.RequiredSpecialists;
using DDDSample1.Domain.Logs;
using DDDSample1.Domain.OperationRequests;
using DDDSample1.Infrastructure.AvailabilitySlots;
using DDDSample1.Domain.AvailabilitySlots;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Domain.OperationRoomTypes;
using DDDSample1.Domain.AssignedStaffs;
using DDDSample1.Infrastructure.AssignedStaffs;

namespace DDDSample1.Infrastructure
{
	public class HospitalDbContext : DbContext
	{
		public DbSet<Log> ShadowTable { get; set; }
		public DbSet<OperationRoom> operationRooms { get; set; }
		public DbSet<OperationRoomType> operationRoomTypes { get; set; }
		public DbSet<AvailabilitySlot> availabilitySlots { get; set; }
		public DbSet<LoginAttemptTracker> loginAttemps { get; set; }
		public DbSet<ContactInformation> ContactInformations { get; set; }
		public DbSet<Doctor> Doctors { get; set; }
		public DbSet<Token> Tokens { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Specialization> Specializations { get; set; }
		public DbSet<Staff> Staff { get; set; }
		public DbSet<Patient> Patients { get; set; }
		public DbSet<Appointment> Appointments { get; set; }
		public DbSet<OperationType> OperationTypes { get; set; }
		public DbSet<OperationPhase> OperationPhases { get; set; }
		public DbSet<RequiredSpecialist> RequiredSpecialists { get; set; }
		public DbSet<AssignedStaff> AssignedStaff { get; set; }

		public DbSet<OperationRequest> OperationRequests { get; set; }

		public HospitalDbContext(DbContextOptions options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new StaffEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new PatientEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new AppointmentEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new SpecializationEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new TokenEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new DoctorEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new ContactInformationEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new LoginAttemptTrackerEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new LogEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new OperationTypeEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new OperationPhaseEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new RequiredSpecialistEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new AssignedStaffEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new OperationRequestEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new AvailabilitySlotsEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new OperationRoomEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new OperationRoomTypeEntityTypeConfiguration());
			//modelBuilder.ApplyConfiguration(new FullNameEntityTypeConfiguration());

			// Changes Tokens Id from varchar(250) to varchar(550)
			// necessary change to ensure it doesn't result in an error on the db server
			modelBuilder.Entity<Token>()
				.Property(t => t.Id)
				.HasConversion(
					id => id.AsString(),
					str => new TokenId(str))
				.HasMaxLength(550);

			// Change SpecializationDescription form varchar to text
			// so as not to limit description size
			/* modelBuilder.Entity<Specialization>()
				.Property(p => p.SpecializationDescription)
				.HasColumnType("TEXT"); */

			/* modelBuilder.Entity<ContactInformation>()
				.Property(ci => ci.Id)
				.HasColumnType("VARCHAR(550)")
				.IsRequired(); */
		}
	}
}