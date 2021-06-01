using System;
using CWBFightClub.Models;
using Microsoft.EntityFrameworkCore;

namespace CWBFightClub.Data
{
    /// <summary>
    /// The context class for the CWBFightClub database.
    /// </summary>
    public class CWBContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the CWBContext class.
        /// </summary>
        /// <param name="options">Options take from the base DbContext class.</param>
        public CWBContext(DbContextOptions<CWBContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the Student table.
        /// </summary>
        public DbSet<Student> Students { get; set; }

        /// <summary>
        /// Gets or sets the StudentGuardian table.
        /// </summary>
        public DbSet<StudentGuardian> StudentGuardians { get; set; }

        /// <summary>
        /// Gets or sets the Guardian table.
        /// </summary>
        public DbSet<Guardian> Guardians { get; set; }

        /// <summary>
        /// Gets or sets the Account table.
        /// </summary>
        public DbSet<Account> Accounts { get; set; }

        /// <summary>
        /// Creates the Discipline table.
        /// </summary>
        public DbSet<Discipline> Disciplines { get; set; }

        /// <summary>
        /// Gets or sets the ScheduledClass table.
        /// </summary>
        public DbSet<ScheduledClass> ScheduledClasses { get; set; }

        /// <summary>
        /// Gets or sets the AttendanceRecord table.
        /// </summary>
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }

        /// <summary>
        /// Creates the Belt table.
        /// </summary>
        public DbSet<Belt> Belts { get; set; }

        /// <summary>
        /// Gets or sets the Enrollment table.
        /// </summary>
        public DbSet<Enrollment> Enrollments { get; set; }

        /// <summary>
        /// Gets or sets the AchievedBelt table.
        /// </summary>
        public DbSet<AchievedBelt> AchievedBelts { get; set; }

        /// <summary>
        /// Gets or sets the Payment table.
        /// </summary>
        public DbSet<Payment> Payments { get; set; }

        /// <summary>
        /// Gets or sets the file path for documents.
        /// </summary>
        public DbSet<FilePath> FilePaths { get; set; }

        /// <summary>
        /// Gets or sets the global app settings.
        /// </summary>
        public DbSet<AppSetting> AppSettings { get; set; }

        /// <summary>
        /// Overrides the naming conventions of the table to keep them from being pluralized.
        /// </summary>
        /// <param name="modelBuilder">Model builder to use.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<StudentGuardian>().ToTable("StudentGuardian");
            modelBuilder.Entity<Guardian>().ToTable("Guardian");
            modelBuilder.Entity<Account>().ToTable("Account");
            modelBuilder.Entity<Discipline>().ToTable("Discipline");
            modelBuilder.Entity<ScheduledClass>().ToTable("ScheduledClass");
            modelBuilder.Entity<Belt>().ToTable("Belt");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<AchievedBelt>().ToTable("AchievedBelt");
            modelBuilder.Entity<Payment>().ToTable("Payment");
            modelBuilder.Entity<AttendanceRecord>().ToTable("AttendanceRecord");
            modelBuilder.Entity<FilePath>().ToTable("FilePath");
            modelBuilder.Entity<AppSetting>().ToTable("AppSetting");
        }

        public void ExecuteDatabaseBackupUSP()
        {
            string sql = "EXEC dbo.usp_DatabaseBackup";

            try
            {
                this.Database.ExecuteSqlRaw(sql);
            }
            catch(DbUpdateConcurrencyException ex)
            {
                throw new DbUpdateConcurrencyException("There was a problem reaching the database. Message: ", ex);
            }
            catch(InvalidOperationException ex)
            {
                throw new InvalidOperationException("There was a problem with the operation. Message: ", ex);
            }
            catch(Exception ex)
            {
                throw new Exception("There was a problem when using the database. Error: ", ex);
            }
        }
    }
}
