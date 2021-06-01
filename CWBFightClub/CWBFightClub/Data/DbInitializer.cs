using CWBFightClub.Models;
using CWBFightClub.Services;
using CWBFightClub.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CWBFightClub.Data
{
    /// <summary>
    /// Gets the database ready through entity framework core.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Initializes the database. Creates it if it does not already exist and then seeds the database with some test data.
        /// </summary>
        /// <param name="context">DbContext parameters from the CWBContext class.</param>
        public static void Initialize(CWBContext context)
        {
            // Run migrations. This will create the database if it is not already and run migrations in order.
            context.Database.Migrate();

            // Look for existing students. Stop if this data already exists.
            if (context.Students.Any())
            {
                return;
            }

            addStudents(context);
            addAccounts(context);
            addGuardians(context);
            addStudentGuardians(context);
            addDisciplines(context);
            addBelts(context);
            addEnrollments(context);
            addScheduledClasses(context);
            addAppSettings(context);
            addAchievedBelts(context);
            addAttendanceRecords(context);
            //addPayments() //TODO maybe?
        }

        private static void addAttendanceRecords(CWBContext db)
        {
            List<AttendanceRecord> attendanceRecords = new List<AttendanceRecord>();
            IEnumerable<Student> students = db.Students;
            IEnumerable<ScheduledClass> scheduledClasses = db.ScheduledClasses;
            int monKarate = scheduledClasses.Where(x => x.Name == "Mon Karate").Select(x => x.ScheduledClassID).FirstOrDefault();
            int thuKarate = scheduledClasses.Where(x => x.Name == "Thu Karate").Select(x => x.ScheduledClassID).FirstOrDefault();
            int satKarate = scheduledClasses.Where(x => x.Name == "Sat Karate").Select(x => x.ScheduledClassID).FirstOrDefault();

            int studentID = students.Where(x => x.FirstName == "Trent" && x.LastName == "Hohenstein").Select(x => x.StudentID).FirstOrDefault();

            for (int i = 0; i < 6; i++)
            {
                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = monKarate,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-05 16:30:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-05 17:30:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = thuKarate,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-08 16:30:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-08 17:30:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = satKarate,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-10 08:30:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-10 09:30:00.0000000").AddDays(i * 7)
                });
            }

            studentID = students.Where(x => x.FirstName == "Nate" && x.LastName == "Hohenstein").Select(x => x.StudentID).FirstOrDefault();
            for (int i = 0; i < 5; i++)
            {
                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = monKarate,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-05 16:30:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-05 17:30:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = thuKarate,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-08 16:30:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-08 17:30:00.0000000").AddDays(i * 7)
                });

                if (i < 4)
                {
                    attendanceRecords.Add(new AttendanceRecord
                    {
                        ScheduledClassID = satKarate,
                        StudentID = studentID,
                        IsVerified = true,
                        Start = DateTime.Parse("2021-04-10 08:30:00.0000000").AddDays(i * 7),
                        End = DateTime.Parse("2021-04-10 09:30:00.0000000").AddDays(i * 7)
                    });
                }                
            }

            studentID = students.Where(x => x.FirstName == "Joe" && x.LastName == "Jirschele").Select(x => x.StudentID).FirstOrDefault();
            for (int i = 0; i < 5; i++)
            {
                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = monKarate,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-05 16:30:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-05 17:30:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = satKarate,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-10 08:30:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-10 09:30:00.0000000").AddDays(i * 7)
                });
            }

            studentID = students.Where(x => x.FirstName == "Brittany" && x.LastName == "Frank").Select(x => x.StudentID).FirstOrDefault();
            for (int i = 0; i < 6; i++)
            {
                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = thuKarate,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-08 16:30:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-08 17:30:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = satKarate,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-10 08:30:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-10 09:30:00.0000000").AddDays(i * 7)
                });
            }

            studentID = students.Where(x => x.FirstName == "Joel" && x.LastName == "Campbell").Select(x => x.StudentID).FirstOrDefault();
            for (int i = 0; i < 6; i++)
            {
                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = monKarate,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-05 16:30:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-05 17:30:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = thuKarate,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-08 16:30:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-08 17:30:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = satKarate,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-10 08:30:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-10 09:30:00.0000000").AddDays(i * 7)
                });
            }

            studentID = students.Where(x => x.FirstName == "Parker" && x.LastName == "Hohenstein").Select(x => x.StudentID).FirstOrDefault();
            for (int i = 0; i < 6; i++)
            {
                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = satKarate,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-10 08:30:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-10 09:30:00.0000000").AddDays(i * 7)
                });
            }

            studentID = students.Where(x => x.FirstName == "Billy" && x.LastName == "Martinez").Select(x => x.StudentID).FirstOrDefault();
            for (int i = 0; i < 5; i++)
            {
                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = monKarate,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-12 16:30:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-12 17:30:00.0000000").AddDays(i * 7)
                });

                if (i % 2 == 0)
                {
                    attendanceRecords.Add(new AttendanceRecord
                    {
                        ScheduledClassID = thuKarate,
                        StudentID = studentID,
                        IsVerified = true,
                        Start = DateTime.Parse("2021-04-15 16:30:00.0000000").AddDays(i * 7),
                        End = DateTime.Parse("2021-04-15 17:30:00.0000000").AddDays(i * 7)
                    });
                }
            }

            studentID = students.Where(x => x.FirstName == "John" && x.LastName == "Blake").Select(x => x.StudentID).FirstOrDefault();
            attendanceRecords.Add(new AttendanceRecord
            {
                ScheduledClassID = monKarate,
                StudentID = studentID,
                IsVerified = true,
                Start = DateTime.Parse("2021-05-10 16:30:00.0000000"),
                End = DateTime.Parse("2021-05-10 17:30:00.0000000")
            });

            attendanceRecords.Add(new AttendanceRecord
            {
                ScheduledClassID = thuKarate,
                StudentID = studentID,
                IsVerified = true,
                Start = DateTime.Parse("2021-05-06 16:30:00.0000000"),
                End = DateTime.Parse("2021-05-06 17:30:00.0000000")
            });

            attendanceRecords.Add(new AttendanceRecord
            {
                ScheduledClassID = satKarate,
                StudentID = studentID,
                IsVerified = true,
                Start = DateTime.Parse("2021-05-08 08:30:00.0000000"),
                End = DateTime.Parse("2021-05-08 09:30:00.0000000")
            });

            studentID = students.Where(x => x.FirstName == "April" && x.LastName == "Gesserti").Select(x => x.StudentID).FirstOrDefault();
            attendanceRecords.Add(new AttendanceRecord
            {
                ScheduledClassID = monKarate,
                StudentID = studentID,
                IsVerified = true,
                Start = DateTime.Parse("2021-05-10 16:30:00.0000000"),
                End = DateTime.Parse("2021-05-10 17:30:00.0000000")
            });

            attendanceRecords.Add(new AttendanceRecord
            {
                ScheduledClassID = thuKarate,
                StudentID = studentID,
                IsVerified = true,
                Start = DateTime.Parse("2021-05-06 16:30:00.0000000"),
                End = DateTime.Parse("2021-05-06 17:30:00.0000000")
            });

            attendanceRecords.Add(new AttendanceRecord
            {
                ScheduledClassID = satKarate,
                StudentID = studentID,
                IsVerified = true,
                Start = DateTime.Parse("2021-05-08 08:30:00.0000000"),
                End = DateTime.Parse("2021-05-08 09:30:00.0000000")
            });

            studentID = students.Where(x => x.FirstName == "Phil" && x.LastName == "Idaho").Select(x => x.StudentID).FirstOrDefault();
            attendanceRecords.Add(new AttendanceRecord
            {
                ScheduledClassID = monKarate,
                StudentID = studentID,
                IsVerified = true,
                Start = DateTime.Parse("2021-05-10 16:30:00.0000000"),
                End = DateTime.Parse("2021-05-10 17:30:00.0000000")
            });

            attendanceRecords.Add(new AttendanceRecord
            {
                ScheduledClassID = thuKarate,
                StudentID = studentID,
                IsVerified = true,
                Start = DateTime.Parse("2021-05-06 16:30:00.0000000"),
                End = DateTime.Parse("2021-05-06 17:30:00.0000000")
            });

            studentID = students.Where(x => x.FirstName == "Jack" && x.LastName == "Skellington").Select(x => x.StudentID).FirstOrDefault();
            attendanceRecords.Add(new AttendanceRecord
            {
                ScheduledClassID = satKarate,
                StudentID = studentID,
                IsVerified = true,
                Start = DateTime.Parse("2021-05-08 08:30:00.0000000"),
                End = DateTime.Parse("2021-05-08 09:30:00.0000000")
            });

            int monMuay = scheduledClasses.Where(x => x.Name == "Mon Muay Thai").Select(x => x.ScheduledClassID).FirstOrDefault();
            int wedMuay = scheduledClasses.Where(x => x.Name == "Wed Muay Thai").Select(x => x.ScheduledClassID).FirstOrDefault();
            int friMuay = scheduledClasses.Where(x => x.Name == "Fri Muay Thai").Select(x => x.ScheduledClassID).FirstOrDefault();

            studentID = students.Where(x => x.FirstName == "Joel" && x.LastName == "Campbell").Select(x => x.StudentID).FirstOrDefault();
            for (int i = 0; i < 6; i++)
            {
                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = monMuay,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-05 19:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-05 20:00:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = wedMuay,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-07 19:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-07 20:00:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = friMuay,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-09 19:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-09 20:00:00.0000000").AddDays(i * 7)
                });
            }

            int monJiu = scheduledClasses.Where(x => x.Name == "Mon Jiu-Jitsu").Select(x => x.ScheduledClassID).FirstOrDefault();
            int tueJiu = scheduledClasses.Where(x => x.Name == "Tue Jiu-Jitsu").Select(x => x.ScheduledClassID).FirstOrDefault();
            int wedJiu = scheduledClasses.Where(x => x.Name == "Wed Jiu-Jitsu").Select(x => x.ScheduledClassID).FirstOrDefault();
            int thuJiu = scheduledClasses.Where(x => x.Name == "Thu Jiu-Jitsu").Select(x => x.ScheduledClassID).FirstOrDefault();
            int friJiu = scheduledClasses.Where(x => x.Name == "Fri Jiu-Jitsu").Select(x => x.ScheduledClassID).FirstOrDefault();

            for (int i = 0; i < 5; i++)
            {
                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = monJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-05 18:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-05 19:00:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = tueJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-06 19:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-06 20:00:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = wedJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-07 18:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-07 19:00:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = thuJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-08 19:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-08 20:00:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = friJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-09 18:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-09 19:00:00.0000000").AddDays(i * 7)
                });
            }

            studentID = students.Where(x => x.FirstName == "Melissa" && x.LastName == "Marly").Select(x => x.StudentID).FirstOrDefault();
            for (int i = 0; i < 4; i++)
            {
                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = monJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-05 18:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-05 19:00:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = tueJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-06 19:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-06 20:00:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = wedJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-07 18:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-07 19:00:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = thuJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-08 19:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-08 20:00:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = friJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-09 18:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-09 19:00:00.0000000").AddDays(i * 7)
                });
            }

            studentID = students.Where(x => x.FirstName == "Paul" && x.LastName == "Marly").Select(x => x.StudentID).FirstOrDefault();
            for (int i = 0; i < 4; i++)
            {
                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = monJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-05 18:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-05 19:00:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = tueJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-06 19:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-06 20:00:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = wedJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-07 18:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-07 19:00:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = thuJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-08 19:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-08 20:00:00.0000000").AddDays(i * 7)
                });

                attendanceRecords.Add(new AttendanceRecord
                {
                    ScheduledClassID = friJiu,
                    StudentID = studentID,
                    IsVerified = true,
                    Start = DateTime.Parse("2021-04-09 18:00:00.0000000").AddDays(i * 7),
                    End = DateTime.Parse("2021-04-09 19:00:00.0000000").AddDays(i * 7)
                });
            }

            db.AttendanceRecords.AddRange(attendanceRecords);
            db.SaveChanges();
        }

        private static void addAchievedBelts(CWBContext db)
        {
            IEnumerable<Student> students = db.Students;
            IEnumerable<Enrollment> enrollments = db.Enrollments;

            List<AchievedBelt> achievedBelts = new List<AchievedBelt>();

            // Belts for Karate
            int discID = db.Disciplines.Where(x => x.Name == "Budokai Karate").Select(x => x.DisciplineID).FirstOrDefault();
            Belt belt1 = db.Belts.Where(x => x.DisciplineID == discID && x.Rank == 1).FirstOrDefault();
            Belt belt2 = db.Belts.Where(x => x.DisciplineID == discID && x.Rank == 2).FirstOrDefault();
            Belt belt3 = db.Belts.Where(x => x.DisciplineID == discID && x.Rank == 3).FirstOrDefault();
            
            int studentID = students.Where(x => x.FirstName == "Trent" && x.LastName == "Hohenstein").Select(x => x.StudentID).FirstOrDefault();
            int enrollmentID = enrollments.Where(x => x.StudentID == studentID && x.DisciplineID == discID).Select(x => x.EnrollmentID).FirstOrDefault();
            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt1.Rank,
                DateAchieved = new DateTime(2020, 7, 5),
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 7, 5),
                Description = belt1.BeltDescription,
                Name = belt1.Name
            });

            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt2.Rank,
                DateAchieved = new DateTime(2021, 2, 22),
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 2, 22),
                Description = belt2.BeltDescription,
                Name = belt2.Name
            });

            studentID = students.Where(x => x.FirstName == "Nate" && x.LastName == "Hohenstein").Select(x => x.StudentID).FirstOrDefault();
            enrollmentID = enrollments.Where(x => x.StudentID == studentID && x.DisciplineID == discID).Select(x => x.EnrollmentID).FirstOrDefault();
            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt1.Rank,
                DateAchieved = new DateTime(2020, 1, 1),
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 1, 1),
                Description = belt1.BeltDescription,
                Name = belt1.Name
            });

            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt2.Rank,
                DateAchieved = new DateTime(2020, 2, 22),
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 2, 22),
                Description = belt2.BeltDescription,
                Name = belt2.Name
            });

            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt3.Rank,
                DateAchieved = new DateTime(2021, 2, 22),
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 2, 22),
                Description = belt3.BeltDescription,
                Name = belt3.Name
            });

            studentID = students.Where(x => x.FirstName == "Joe" && x.LastName == "Jirschele").Select(x => x.StudentID).FirstOrDefault();
            enrollmentID = enrollments.Where(x => x.StudentID == studentID && x.DisciplineID == discID).Select(x => x.EnrollmentID).FirstOrDefault();
            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt1.Rank,
                DateAchieved = new DateTime(2020, 1, 1),
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 1, 1),
                Description = belt1.BeltDescription,
                Name = belt1.Name
            });

            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt2.Rank,
                DateAchieved = new DateTime(2020, 2, 22),
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 2, 22),
                Description = belt2.BeltDescription,
                Name = belt2.Name
            });

            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt3.Rank,
                DateAchieved = new DateTime(2021, 2, 22),
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 2, 22),
                Description = belt3.BeltDescription,
                Name = belt3.Name
            });

            studentID = students.Where(x => x.FirstName == "Brittany" && x.LastName == "Frank").Select(x => x.StudentID).FirstOrDefault();
            enrollmentID = enrollments.Where(x => x.StudentID == studentID && x.DisciplineID == discID).Select(x => x.EnrollmentID).FirstOrDefault();
            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt1.Rank,
                DateAchieved = new DateTime(2020, 2, 5),
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 2, 5),
                Description = belt1.BeltDescription,
                Name = belt1.Name
            });

            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt2.Rank,
                DateAchieved = new DateTime(2020, 2, 22),
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 2, 22),
                Description = belt2.BeltDescription,
                Name = belt2.Name
            });

            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt3.Rank,
                DateAchieved = new DateTime(2021, 4, 22),
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 22),
                Description = belt3.BeltDescription,
                Name = belt3.Name
            });

            studentID = students.Where(x => x.FirstName == "Joel" && x.LastName == "Campbell").Select(x => x.StudentID).FirstOrDefault();
            enrollmentID = enrollments.Where(x => x.StudentID == studentID && x.DisciplineID == discID).Select(x => x.EnrollmentID).FirstOrDefault();
            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt1.Rank,
                DateAchieved = new DateTime(2020, 2, 6),
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 2, 6),
                Description = belt1.BeltDescription,
                Name = belt1.Name
            });

            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt2.Rank,
                DateAchieved = new DateTime(2020, 2, 22),
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 2, 22),
                Description = belt2.BeltDescription,
                Name = belt2.Name
            });

            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt3.Rank,
                DateAchieved = new DateTime(2021, 2, 22),
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 2, 22),
                Description = belt3.BeltDescription,
                Name = belt3.Name
            });

            studentID = students.Where(x => x.FirstName == "Parker" && x.LastName == "Hohenstein").Select(x => x.StudentID).FirstOrDefault();
            enrollmentID = enrollments.Where(x => x.StudentID == studentID && x.DisciplineID == discID).Select(x => x.EnrollmentID).FirstOrDefault();
            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt1.Rank,
                DateAchieved = new DateTime(2021, 2, 6),
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 2, 6),
                Description = belt1.BeltDescription,
                Name = belt1.Name
            });

            studentID = students.Where(x => x.FirstName == "Billy" && x.LastName == "Martinez").Select(x => x.StudentID).FirstOrDefault();
            enrollmentID = enrollments.Where(x => x.StudentID == studentID && x.DisciplineID == discID).Select(x => x.EnrollmentID).FirstOrDefault();
            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt1.Rank,
                DateAchieved = new DateTime(2021, 5, 6),
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 5, 6),
                Description = belt1.BeltDescription,
                Name = belt1.Name
            });

            studentID = students.Where(x => x.FirstName == "John" && x.LastName == "Blake").Select(x => x.StudentID).FirstOrDefault();
            enrollmentID = enrollments.Where(x => x.StudentID == studentID && x.DisciplineID == discID).Select(x => x.EnrollmentID).FirstOrDefault();
            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt1.Rank,
                DateAchieved = new DateTime(2021, 5, 10),
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 5, 10),
                Description = belt1.BeltDescription,
                Name = belt1.Name
            });

            studentID = students.Where(x => x.FirstName == "Jack" && x.LastName == "Skellington").Select(x => x.StudentID).FirstOrDefault();
            enrollmentID = enrollments.Where(x => x.StudentID == studentID && x.DisciplineID == discID).Select(x => x.EnrollmentID).FirstOrDefault();
            achievedBelts.Add(new AchievedBelt
            {
                EnrollmentID = enrollmentID,
                Rank = belt1.Rank,
                DateAchieved = new DateTime(2021, 5, 6),
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 5, 6),
                Description = belt1.BeltDescription,
                Name = belt1.Name
            });

            db.AchievedBelts.AddRange(achievedBelts);
            db.SaveChanges();
        }

        private static void addAppSettings(CWBContext db)
        {
            db.AppSettings.Add(new AppSetting
            {
                BundleCostPerMonth = 120m,
                BundleCostPerThreeMonths = 360,
                BundleCostPerYear = 1440,
                PercentOfClassRequiredToVerify = 80
            });

            db.SaveChanges();
        }

        private static void addScheduledClasses(CWBContext db)
        {
            List<ScheduledClass> scheduledClasses = new List<ScheduledClass>();
            int discID = db.Disciplines.Where(x => x.Name == "Budokai Karate").Select(x => x.DisciplineID).FirstOrDefault();
            scheduledClasses.Add(new ScheduledClass { DisciplineID = discID, Name = "Mon Karate", Start = DateTime.Parse("2021-04-05 16:30:00.0000000"), End = DateTime.Parse("2021-04-05 17:30:00.0000000"), CreatedDate = DateTime.Now, CreatedBy = 1, HasRecurrence = true, RecurrenceFrequency = "weekly", RecurrenceTime = 104 });
            scheduledClasses.Add(new ScheduledClass { DisciplineID = discID, Name = "Thu Karate", Start = DateTime.Parse("2021-04-08 16:30:00.0000000"), End = DateTime.Parse("2021-04-08 17:30:00.0000000"), CreatedDate = DateTime.Now, CreatedBy = 1, HasRecurrence = true, RecurrenceFrequency = "weekly", RecurrenceTime = 104 });
            scheduledClasses.Add(new ScheduledClass { DisciplineID = discID, Name = "Sat Karate", Start = DateTime.Parse("2021-04-10 08:30:00.0000000"), End = DateTime.Parse("2021-04-10 09:30:00.0000000"), CreatedDate = DateTime.Now, CreatedBy = 1, HasRecurrence = true, RecurrenceFrequency = "weekly", RecurrenceTime = 104 });

            discID = db.Disciplines.Where(x => x.Name == "Muay Thai").Select(x => x.DisciplineID).FirstOrDefault();
            scheduledClasses.Add(new ScheduledClass { DisciplineID = discID, Name = "Mon Muay Thai", Start = DateTime.Parse("2021-04-05 19:00:00.0000000"), End = DateTime.Parse("2021-04-05 20:00:00.0000000"), CreatedDate = DateTime.Now, CreatedBy = 1, HasRecurrence = true, RecurrenceFrequency = "weekly", RecurrenceTime = 104 });
            scheduledClasses.Add(new ScheduledClass { DisciplineID = discID, Name = "Wed Muay Thai", Start = DateTime.Parse("2021-04-07 19:00:00.0000000"), End = DateTime.Parse("2021-04-07 20:00:00.0000000"), CreatedDate = DateTime.Now, CreatedBy = 1, HasRecurrence = true, RecurrenceFrequency = "weekly", RecurrenceTime = 104 });
            scheduledClasses.Add(new ScheduledClass { DisciplineID = discID, Name = "Fri Muay Thai", Start = DateTime.Parse("2021-04-09 19:00:00.0000000"), End = DateTime.Parse("2021-04-09 20:00:00.0000000"), CreatedDate = DateTime.Now, CreatedBy = 1, HasRecurrence = true, RecurrenceFrequency = "weekly", RecurrenceTime = 104 });

            discID = db.Disciplines.Where(x => x.Name == "Brazilian Jiu-Jitsu").Select(x => x.DisciplineID).FirstOrDefault();
            scheduledClasses.Add(new ScheduledClass { DisciplineID = discID, Name = "Mon Jiu-Jitsu", Start = DateTime.Parse("2021-04-05 18:00:00.0000000"), End = DateTime.Parse("2021-04-05 19:00:00.0000000"), CreatedDate = DateTime.Now, CreatedBy = 1, HasRecurrence = true, RecurrenceFrequency = "weekly", RecurrenceTime = 104 });
            scheduledClasses.Add(new ScheduledClass { DisciplineID = discID, Name = "Wed Jiu-Jitsu", Start = DateTime.Parse("2021-04-07 18:00:00.0000000"), End = DateTime.Parse("2021-04-07 19:00:00.0000000"), CreatedDate = DateTime.Now, CreatedBy = 1, HasRecurrence = true, RecurrenceFrequency = "weekly", RecurrenceTime = 104 });
            scheduledClasses.Add(new ScheduledClass { DisciplineID = discID, Name = "Fri Jiu-Jitsu", Start = DateTime.Parse("2021-04-09 18:00:00.0000000"), End = DateTime.Parse("2021-04-09 19:00:00.0000000"), CreatedDate = DateTime.Now, CreatedBy = 1, HasRecurrence = true, RecurrenceFrequency = "weekly", RecurrenceTime = 104 });
            scheduledClasses.Add(new ScheduledClass { DisciplineID = discID, Name = "Tue Jiu-Jitsu", Start = DateTime.Parse("2021-04-06 19:00:00.0000000"), End = DateTime.Parse("2021-04-06 20:00:00.0000000"), CreatedDate = DateTime.Now, CreatedBy = 1, HasRecurrence = true, RecurrenceFrequency = "weekly", RecurrenceTime = 104 });
            scheduledClasses.Add(new ScheduledClass { DisciplineID = discID, Name = "Thu Jiu-Jitsu", Start = DateTime.Parse("2021-04-08 19:00:00.0000000"), End = DateTime.Parse("2021-04-08 20:00:00.0000000"), CreatedDate = DateTime.Now, CreatedBy = 1, HasRecurrence = true, RecurrenceFrequency = "weekly", RecurrenceTime = 104 });

            discID = db.Disciplines.Where(x => x.Name == SystemConstants.Walkin).Select(x => x.DisciplineID).FirstOrDefault();
            scheduledClasses.Add(new ScheduledClass { DisciplineID = discID, Name = SystemConstants.Walkin, Start = DateTime.Parse("2021-03-25 00:00:00.0000000"), End = DateTime.Parse("2021-03-25 23:59:59.9999999"), CreatedDate = DateTime.Now, CreatedBy = 1, HasRecurrence = true, RecurrenceFrequency = "daily", RecurrenceTime = 104 });

            db.ScheduledClasses.AddRange(scheduledClasses);
            db.SaveChanges();
        }

        private static void addEnrollments(CWBContext db)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            IEnumerable<Student> students = db.Students;

            // Budokai Karate Enrollments
            int discID = db.Disciplines.Where(x => x.Name == "Budokai Karate").Select(x => x.DisciplineID).FirstOrDefault();
            int studentID = students.Where(x => x.FirstName == "Trent" && x.LastName == "Hohenstein").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment 
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 5, 1),
                StartDate = new DateTime(2020, 5, 1)
            });
            
            studentID = students.Where(x => x.FirstName == "Parker" && x.LastName == "Hohenstein").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 5, 1),
                StartDate = new DateTime(2020, 5, 1)
            });

            studentID = students.Where(x => x.FirstName == "Nate" && x.LastName == "Hohenstein").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 1, 1),
                StartDate = new DateTime(2020, 1, 1)
            });

            studentID = students.Where(x => x.FirstName == "Joe" && x.LastName == "Jirschele").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 1, 1),
                StartDate = new DateTime(2020, 1, 1)
            });

            studentID = students.Where(x => x.FirstName == "Brittany" && x.LastName == "Frank").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 1, 1),
                StartDate = new DateTime(2020, 1, 1)
            });

            studentID = students.Where(x => x.FirstName == "Joel" && x.LastName == "Campbell").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 1, 1),
                StartDate = new DateTime(2020, 1, 1)
            });

            studentID = students.Where(x => x.FirstName == "Billy" && x.LastName == "Martinez").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 11),
                StartDate = new DateTime(2021, 4, 11)
            });

            studentID = students.Where(x => x.FirstName == "John" && x.LastName == "Blake").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 29),
                StartDate = new DateTime(2021, 4, 29)
            });

            studentID = students.Where(x => x.FirstName == "April" && x.LastName == "Gesserti").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 29),
                StartDate = new DateTime(2021, 4, 29)
            });

            studentID = students.Where(x => x.FirstName == "Phil" && x.LastName == "Idaho").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 29),
                StartDate = new DateTime(2021, 4, 29)
            });

            studentID = students.Where(x => x.FirstName == "Jack" && x.LastName == "Skellington").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 29),
                StartDate = new DateTime(2021, 4, 29)
            });

            // Brazilian Jiu-Jitsu Enrollments
            discID = db.Disciplines.Where(x => x.Name == "Brazilian Jiu-Jitsu").Select(x => x.DisciplineID).FirstOrDefault();
            studentID = students.Where(x => x.FirstName == "Brittany" && x.LastName == "Frank").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 1, 1),
                StartDate = new DateTime(2020, 1, 1)
            });

            studentID = students.Where(x => x.FirstName == "Joel" && x.LastName == "Campbell").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 1, 1),
                StartDate = new DateTime(2020, 1, 1)
            });

            studentID = students.Where(x => x.FirstName == "Melissa" && x.LastName == "Marly").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 5, 1),
                StartDate = new DateTime(2020, 5, 1)
            });

            studentID = students.Where(x => x.FirstName == "Paul" && x.LastName == "Marly").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 5, 1),
                StartDate = new DateTime(2020, 5, 1)
            });

            studentID = students.Where(x => x.FirstName == "Billy" && x.LastName == "Martinez").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 11),
                StartDate = new DateTime(2021, 4, 11)
            });

            studentID = students.Where(x => x.FirstName == "John" && x.LastName == "Blake").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 29),
                StartDate = new DateTime(2021, 4, 29)
            });

            studentID = students.Where(x => x.FirstName == "April" && x.LastName == "Gesserti").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 29),
                StartDate = new DateTime(2021, 4, 29)
            });

            studentID = students.Where(x => x.FirstName == "Phil" && x.LastName == "Idaho").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 29),
                StartDate = new DateTime(2021, 4, 29)
            });

            studentID = students.Where(x => x.FirstName == "Jack" && x.LastName == "Skellington").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 29),
                StartDate = new DateTime(2021, 4, 29)
            });

            // Muay Thai Enrollments
            discID = db.Disciplines.Where(x => x.Name == "Muay Thai").Select(x => x.DisciplineID).FirstOrDefault();
            studentID = students.Where(x => x.FirstName == "Brittany" && x.LastName == "Frank").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 1, 1),
                StartDate = new DateTime(2020, 1, 1)
            });

            studentID = students.Where(x => x.FirstName == "Joel" && x.LastName == "Campbell").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 1, 1),
                StartDate = new DateTime(2020, 1, 1)
            });

            studentID = students.Where(x => x.FirstName == "Billy" && x.LastName == "Martinez").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 11),
                StartDate = new DateTime(2021, 4, 11)
            });

            studentID = students.Where(x => x.FirstName == "John" && x.LastName == "Blake").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 29),
                StartDate = new DateTime(2021, 4, 29)
            });

            studentID = students.Where(x => x.FirstName == "April" && x.LastName == "Gesserti").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 29),
                StartDate = new DateTime(2021, 4, 29)
            });

            studentID = students.Where(x => x.FirstName == "Phil" && x.LastName == "Idaho").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 29),
                StartDate = new DateTime(2021, 4, 29)
            });

            studentID = students.Where(x => x.FirstName == "Jack" && x.LastName == "Skellington").Select(x => x.StudentID).FirstOrDefault();
            enrollments.Add(new Enrollment
            {
                StudentID = studentID,
                DisciplineID = discID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 29),
                StartDate = new DateTime(2021, 4, 29)
            });

            db.Enrollments.AddRange(enrollments);
            db.SaveChanges();
        }

        private static void addBelts(CWBContext db)
        {
            List<Belt> belts = new List<Belt>();

            int discID = db.Disciplines.Where(x => x.Name == "Muay Thai").Select(x => x.DisciplineID).FirstOrDefault();
            belts.Add(new Belt { Name = "White", DisciplineID = discID, BeltDescription = "This is the White arm band in the Muay Thai Discipline.", Rank = 1, RankDescription = "This is the 1st rank in the Muay Thai Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Yellow", DisciplineID = discID, BeltDescription = "This is the Yellow arm band in the Muay Thai Discipline.", Rank = 2, RankDescription = "This is the 2nd rank in the Muay Thai Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Orange", DisciplineID = discID, BeltDescription = "This is the Orange arm band in the Muay Thai Discipline.", Rank = 3, RankDescription = "This is the 3rd rank in the Muay Thai Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Green", DisciplineID = discID, BeltDescription = "This is the Green arm band in the Muay Thai Discipline.", Rank = 4, RankDescription = "This is the 4th rank in the Muay Thai Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Blue", DisciplineID = discID, BeltDescription = "This is the Blue arm band in the Muay Thai Discipline.", Rank = 5, RankDescription = "This is the 5th rank in the Muay Thai Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Purple", DisciplineID = discID, BeltDescription = "This is the Purple arm band in the Muay Thai Discipline.", Rank = 6, RankDescription = "This is the 6th rank in the Muay Thai Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Red", DisciplineID = discID, BeltDescription = "This is the Red arm band in the Muay Thai Discipline.", Rank = 7, RankDescription = "This is the 7th rank in the Muay Thai Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Brown", DisciplineID = discID, BeltDescription = "This is the Brown arm band in the Muay Thai Discipline.", Rank = 8, RankDescription = "This is the 8th rank in the Muay Thai Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Black", DisciplineID = discID, BeltDescription = "This is the Black arm band in the Muay Thai Discipline.", Rank = 9, RankDescription = "This is the 9th rank in the Muay Thai Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Black and White", DisciplineID = discID, BeltDescription = "This is the Black and White arm band in the Muay Thai Discipline.", Rank = 10, RankDescription = "This is the 10th rank in the Muay Thai Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Black and Red", DisciplineID = discID, BeltDescription = "This is the Black and Red arm band in the Muay Thai Discipline.", Rank = 11, RankDescription = "This is the 11th rank in the Muay Thai Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Black and Silver", DisciplineID = discID, BeltDescription = "This is the Black and Silver arm band in the Muay Thai Discipline.", Rank = 12, RankDescription = "This is the 12th rank in the Muay Thai Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Black and Gold", DisciplineID = discID, BeltDescription = "This is the Black and Gold arm band in the Muay Thai Discipline.", Rank = 13, RankDescription = "This is the 13th rank in the Muay Thai Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            discID = db.Disciplines.Where(x => x.Name == "Brazilian Jiu-Jitsu").Select(x => x.DisciplineID).FirstOrDefault();
            belts.Add(new Belt { Name = "White", DisciplineID = discID, BeltDescription = "This is the White belt in the Brazilian Jiu-Jitsu Discipline.", Rank = 1, RankDescription = "This is the 1st rank in the Brazilian Jiu-Jitsu Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Blue", DisciplineID = discID, BeltDescription = "This is the Blue belt in the Brazilian Jiu-Jitsu Discipline.", Rank = 2, RankDescription = "This is the 2nd rank in the Brazilian Jiu-Jitsu Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Purple", DisciplineID = discID, BeltDescription = "This is the Purple belt in the Brazilian Jiu-Jitsu Discipline.", Rank = 3, RankDescription = "This is the 3rd rank in the Brazilian Jiu-Jitsu Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Brown", DisciplineID = discID, BeltDescription = "This is the Brown belt in the Brazilian Jiu-Jitsu Discipline.", Rank = 4, RankDescription = "This is the 4th rank in the Brazilian Jiu-Jitsu Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Black", DisciplineID = discID, BeltDescription = "This is the Black belt in the Brazilian Jiu-Jitsu Discipline.", Rank = 5, RankDescription = "This is the 5th rank in the Brazilian Jiu-Jitsu Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Red and Black", DisciplineID = discID, BeltDescription = "This is the Red and Black belt in the Brazilian Jiu-Jitsu Discipline.", Rank = 6, RankDescription = "This is the 6th rank in the Brazilian Jiu-Jitsu Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Red and White", DisciplineID = discID, BeltDescription = "This is the Red and White belt in the Brazilian Jiu-Jitsu Discipline.", Rank = 7, RankDescription = "This is the 7th rank in the Brazilian Jiu-Jitsu Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Red", DisciplineID = discID, BeltDescription = "This is the Red belt in the Brazilian Jiu-Jitsu Discipline.", Rank = 8, RankDescription = "This is the 8th rank in the Brazilian Jiu-Jitsu Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            discID = db.Disciplines.Where(x => x.Name == "Budokai Karate").Select(x => x.DisciplineID).FirstOrDefault();
            belts.Add(new Belt { Name = "White", DisciplineID = discID, BeltDescription = "This is the White belt in the Budokai Karate Discipline.", Rank = 1, RankDescription = "This is the 1st rank in the Budokai Karate Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Yellow", DisciplineID = discID, BeltDescription = "This is the Yellow belt in the Budokai Karate Discipline.", Rank = 2, RankDescription = "This is the 2nd rank in the Budokai Karate Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Orange", DisciplineID = discID, BeltDescription = "This is the Orange belt in the Budokai Karate Discipline.", Rank = 3, RankDescription = "This is the 3rd rank in the Budokai Karate Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Green", DisciplineID = discID, BeltDescription = "This is the Green belt in the Budokai Karate Discipline.", Rank = 4, RankDescription = "This is the 4th rank in the Budokai Karate Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Blue", DisciplineID = discID, BeltDescription = "This is the Blue belt in the Budokai Karate Discipline.", Rank = 5, RankDescription = "This is the 5th rank in the Budokai Karate Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });         
            belts.Add(new Belt { Name = "Brown", DisciplineID = discID, BeltDescription = "This is the Brown belt in the Budokai Karate Discipline.", Rank = 6, RankDescription = "This is the 6th rank in the Budokai Karate Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });
            belts.Add(new Belt { Name = "Black", DisciplineID = discID, BeltDescription = "This is the Black belt in the Budokai Karate Discipline.", Rank = 7, RankDescription = "This is the 7th rank in the Budokai Karate Discipline.", CreatedBy = 1, CreatedDate = new DateTime(2020, 1, 1), IsArchived = false });

            db.Belts.AddRange(belts);
            db.SaveChanges();
        }

        private static void addDisciplines(CWBContext db)
        {
            var disciplines = new Discipline[]
            {
                new Discipline{Name="Muay Thai", Description="At CWB Fight Club, we teach Brazilian Muay Thai. This style of Muay Thai is different than traditional Muay Thai in that the techniques make use of western boxing style and varied stances, encouraging more movement, and making it better adapted for mixed martial arts (MMA). During our kickboxing classes, we focus upon striking techniques, fitness, and conditioning. If you're looking for a great way to get in shape fast while learning valuable self-defense skills, Muay Thai should be at the top of your list!", DefaultCostPerMonth=60.00m, CreatedBy=1, CreatedDate=new DateTime(2020, 1, 1), IsArchived=false, CalendarColor="#22519c"},
                new Discipline{Name="Brazilian Jiu-Jitsu", Description="At CWB Fight Club, we teach Brazilian Jui-Jitsu. You may not know this, but most physical altercations end up on the ground. As a result it's important to be able to defend yourself should this happen to you. The Brazilian Jiu-Jitsu classes at CWB Fight Club focus on your ability to use technique and leverage to apply joint locks, chokes, and maintain dominant body position with an opponent on the ground regardless of their size or strength. Because of this, Brazilian Jiu-Jitsu is widely considered the most effective self-defense art in the world.", DefaultCostPerMonth=90.00m, CreatedBy=1, CreatedDate=new DateTime(2020, 1, 1), IsArchived=false, CalendarColor="#338216"},
                new Discipline{Name="Budokai Karate", Description="At CWB Fight Club, we teach Traditional Japanese Karate. Karate is about more than just punching and kicking. Our professional instructors focus upon fostering an environment where you will be able to develop your mind, body, and spirit in an uplifting, empowering, team environment.", DefaultCostPerMonth=50.00m, CreatedBy=1, CreatedDate=new DateTime(2020, 1, 1), IsArchived=false, CalendarColor="#821616"},
                new Discipline{Name=SystemConstants.Walkin, Description="Walkins not assigned to another Discipline.", DefaultCostPerMonth=0.00m, CreatedBy=1, CreatedDate=new DateTime(2020, 1, 1), IsArchived=false, CalendarColor="#000000"}
            };

            db.Disciplines.AddRange(disciplines);
            db.SaveChanges();
        }

        private static void addStudentGuardians(CWBContext db)
        {
            var studentGuardians = new List<StudentGuardian>();

            int studentID = db.Students.Where(x => x.FirstName == "Trent" && x.LastName == "Hohenstein").Select(x => x.StudentID).FirstOrDefault();
            int guardianID = db.Guardians.Where(x => x.FirstName == "Becky" && x.LastName == "Hohenstein").Select(x => x.GuardianID).FirstOrDefault();
            studentGuardians.Add(new StudentGuardian
            {
                StudentID = studentID,
                GuardianID = guardianID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 5, 1),
                IsArchived = false
            });

            studentID = db.Students.Where(x => x.FirstName == "Parker" && x.LastName == "Hohenstein").Select(x => x.StudentID).FirstOrDefault();
            studentGuardians.Add(new StudentGuardian
            {
                StudentID = studentID,
                GuardianID = guardianID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 5, 1),
                IsArchived = false
            });

            studentID = db.Students.Where(x => x.FirstName == "Melissa" && x.LastName == "Marly").Select(x => x.StudentID).FirstOrDefault();
            guardianID = db.Guardians.Where(x => x.FirstName == "Bob" && x.LastName == "Marly").Select(x => x.GuardianID).FirstOrDefault();
            studentGuardians.Add(new StudentGuardian
            {
                StudentID = studentID,
                GuardianID = guardianID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 5, 1),
                IsArchived = false
            });

            studentID = db.Students.Where(x => x.FirstName == "Paul" && x.LastName == "Marly").Select(x => x.StudentID).FirstOrDefault();
            studentGuardians.Add(new StudentGuardian
            {
                StudentID = studentID,
                GuardianID = guardianID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2020, 5, 1),
                IsArchived = false
            });

            studentID = db.Students.Where(x => x.FirstName == "Billy" && x.LastName == "Martinez").Select(x => x.StudentID).FirstOrDefault();
            guardianID = db.Guardians.Where(x => x.FirstName == "Joyce" && x.LastName == "Martinez").Select(x => x.GuardianID).FirstOrDefault();
            studentGuardians.Add(new StudentGuardian
            {
                StudentID = studentID,
                GuardianID = guardianID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 11),
                IsArchived = false
            });

            studentID = db.Students.Where(x => x.FirstName == "John" && x.LastName == "Blake").Select(x => x.StudentID).FirstOrDefault();
            guardianID = db.Guardians.Where(x => x.FirstName == "Doris" && x.LastName == "Ashok").Select(x => x.GuardianID).FirstOrDefault();
            studentGuardians.Add(new StudentGuardian
            {
                StudentID = studentID,
                GuardianID = guardianID,
                CreatedBy = 1,
                CreatedDate = new DateTime(2021, 4, 29),
                IsArchived = false
            });

            db.StudentGuardians.AddRange(studentGuardians);
            db.SaveChanges();
        }

        private static void addGuardians(CWBContext db)
        {
            var guardians = new Guardian[]
            {
                new Guardian
                {
                    FirstName = "Doris", LastName = "Ashok",
                    StreetAddress = "555 Wood Duck Way", City = "Stevens Point", State = "WI", Phone = "(716) 555-1235", ZIP = "54455",
                    Email = "tomwaits_for_no_one@email.com", CreatedBy = 1, CreatedDate = new DateTime(2020, 5, 10), IsArchived = false
                },
                new Guardian
                {
                    FirstName = "Joyce", LastName = "Martinez",
                    StreetAddress = "8675309 Wisconsin St.", City = "Plover", State = "WI", Phone = "(716) 555-1236", ZIP = "54455",
                    Email = "popeyes_neighbor@email.com", CreatedBy = 3, CreatedDate = new DateTime(2021, 4, 11), IsArchived = false
                },
                new Guardian
                {
                    FirstName = "Bob", LastName = "Marly",
                    StreetAddress = "4545 Cty. Hwy. C", City = "Stevens Point", State = "WI", Phone = "(716) 555-1237", ZIP = "54455",
                    Email = "here_for_the_gear@email.com", CreatedBy = 3,  CreatedDate = new DateTime(2020, 5, 12), IsArchived = false
                },
                new Guardian
                {
                    FirstName = "Becky", LastName = "Hohenstein",
                    StreetAddress = "4545 Cty. Hwy. C", City = "Marshfield", State = "WI", Phone = "(716) 555-1238", ZIP = "54455",
                    Email = "bhohen@email.com", CreatedBy = 3,  CreatedDate = new DateTime(2020, 5, 13), IsArchived = false
                }
            };

            db.Guardians.AddRange(guardians);
            db.SaveChanges();
        }

        private static void addAccounts(CWBContext db)
        {
            List<Account> accounts = new List<Account>();
            int studentID = db.Students.Where(x => x.FirstName == "Admin" && x.LastName == "System").Select(x => x.StudentID).FirstOrDefault();
            accounts.Add(new Account
            {
                StudentID = studentID,
                Username = "admin",
                Password = "1000.QH8KOSoOPiFGcqpv+cfYrg==.UKRl3p6A/iWI7N9Z3ugMKBxSnF7BtVPbOI+4KGFeDVc=",
                IsArchived = false,
                CreatedDate = new DateTime(2020, 1, 1),
                CreatedBy = 1
            });

            studentID = db.Students.Where(x => x.FirstName == "Nate" && x.LastName == "Hohenstein").Select(x => x.StudentID).FirstOrDefault();
            accounts.Add(new Account
            {
                StudentID = studentID,
                Username = "nateh",
                Password = "1000.3+8M3UE6g+lX4lccfwvvPA==.QEtLeh+qMDpxAijvwvFvFs8JXnmfMDy7V59e+5QLFbI=",
                IsArchived = false,
                CreatedDate = new DateTime(2020, 1, 1),
                CreatedBy = 1
            });

            studentID = db.Students.Where(x => x.FirstName == "Joe" && x.LastName == "Jirschele").Select(x => x.StudentID).FirstOrDefault();
            accounts.Add(new Account
            {
                StudentID = studentID,
                Username = "josephj",
                Password = "1000.gAFo3hVf8JBZCThERVHS3g==.HvvWQPQ8KgehjZ/Avg9E/tDWvuh1xlAK1FfEpulvAnY=",
                IsArchived = false,
                CreatedDate = new DateTime(2020, 1, 1),
                CreatedBy = 1
            });

            db.Accounts.AddRange(accounts);
            db.SaveChanges();
        }

        private static void addStudents(CWBContext db)
        {
            var students = new Student[]
            {
                new Student
                {
                    FirstName = "Admin", LastName = "System", DOB = new DateTime(1980, 2, 20), CreatedBy = 1,
                    CreatedDate = new DateTime(2020, 1, 1), IsArchived = false, IsInstructor = true
                },
                new Student
                {
                    FirstName = "Nate", LastName = "Hohenstein", DOB = new DateTime(1990, 3, 10), CreatedBy = 1,
                    CreatedDate = new DateTime(2020, 1, 1), IsArchived = false, IsInstructor = true, BalanceDue = 0, BalanceDueDate = DateTime.Parse("2021-06-05 16:30:00.0000000"),
                    PaymentAgreenmentPeriod = 0, PaymentAgreementAmount = 35, Phone = "(715) 555-1234"
                },
                new Student
                {
                    FirstName = "Joe", LastName = "Jirschele", DOB = new DateTime(1947, 12, 12), CreatedBy = 1,
                    CreatedDate = new DateTime(2020, 1, 1), IsArchived = false, IsInstructor = true, Phone = "(715) 555-1235"
                },
                new Student
                {
                    FirstName = "Brittany", LastName = "Frank", DOB = new DateTime(1983, 6, 25), CreatedBy = 1,
                    CreatedDate = new DateTime(2020, 2, 5), IsArchived = false, IsInstructor = true, Phone = "(715) 555-1236"
                },
                new Student
                {
                    FirstName = "Joel", LastName = "Campbell", DOB = new DateTime(1981, 6, 25), CreatedBy = 1,
                    CreatedDate = new DateTime(2020, 2, 5), IsArchived = false, IsInstructor = true, Phone = "(715) 555-1237"
                },
                new Student
                {
                    FirstName = "Trent", LastName = "Hohenstein", DOB = new DateTime(1999, 11, 30), CreatedBy = 1,
                    CreatedDate = new DateTime(2020, 5, 1), IsArchived = false, IsInstructor = false, BalanceDue = 0, 
                    BalanceDueDate = DateTime.Parse("2021-06-05 16:30:00.0000000"),
                    PaymentAgreenmentPeriod = 0, PaymentAgreementAmount = 50, Phone = "(715) 555-1239"
                },
                new Student
                {
                    FirstName = "Parker", LastName = "Hohenstein", DOB = new DateTime(2001, 4, 4), CreatedBy = 1,
                    CreatedDate = new DateTime(2020, 5, 1), IsArchived = false, IsInstructor = false, BalanceDue = 0, BalanceDueDate = new DateTime(2021, 5, 20),
                    PaymentAgreenmentPeriod = 0, PaymentAgreementAmount = 40, Phone = "(715) 555-1255"
                },
                new Student
                {
                    FirstName = "Melissa", LastName = "Marly", DOB = new DateTime(2004, 4, 4), CreatedBy = 1,
                    CreatedDate = new DateTime(2020, 5, 1), IsArchived = false, IsInstructor = false, BalanceDue = 0, BalanceDueDate = new DateTime(2021, 5, 21),
                    PaymentAgreenmentPeriod = 0, PaymentAgreementAmount = 20, Phone = "(715) 555-1244"
                },
                new Student
                {
                    FirstName = "Paul", LastName = "Marly", DOB = new DateTime(2005, 2, 11), CreatedBy = 1,
                    CreatedDate = new DateTime(2020, 5, 1), IsArchived = false, IsInstructor = false, BalanceDue = 0, BalanceDueDate = new DateTime(2021, 5, 21),
                    PaymentAgreenmentPeriod = 0, PaymentAgreementAmount = 20, Phone = "(715) 555-1733"
                },
                new Student
                {
                    FirstName = "Billy", LastName = "Martinez", DOB = new DateTime(2003, 7, 1), CreatedBy = 1,
                    CreatedDate = new DateTime(2021, 4, 11), IsArchived = false, IsInstructor = false, BalanceDue = 0, BalanceDueDate = new DateTime(2021, 5, 14),
                    PaymentAgreenmentPeriod = 0, PaymentAgreementAmount = 120, Phone = "(715) 555-1233"
                },
                new Student
                {
                    FirstName = "John", LastName = "Blake", DOB = new DateTime(2003, 6, 22), CreatedBy = 1,
                    CreatedDate = new DateTime(2021, 4, 29), IsArchived = false, IsInstructor = false, BalanceDue = 0, BalanceDueDate = new DateTime(2021, 5, 29),
                    PaymentAgreenmentPeriod = 0, PaymentAgreementAmount = 120, Phone = "(715) 555-1233"
                },
                new Student
                {
                    FirstName = "April", LastName = "Gesserti", DOB = new DateTime(1977, 6, 12), CreatedBy = 1,
                    CreatedDate = new DateTime(2021, 4, 29), IsArchived = false, IsInstructor = false, BalanceDue = 0, BalanceDueDate = new DateTime(2021, 5, 29),
                    PaymentAgreenmentPeriod = 0, PaymentAgreementAmount = 120, Phone = "(715) 555-1233"
                },
                new Student
                {
                    FirstName = "Phil", LastName = "Idaho", DOB = new DateTime(1984, 8, 22), CreatedBy = 1,
                    CreatedDate = new DateTime(2021, 4, 29), IsArchived = false, IsInstructor = false, BalanceDue = 0, BalanceDueDate = new DateTime(2021, 5, 29),
                    PaymentAgreenmentPeriod = 0, PaymentAgreementAmount = 120, Phone = "(715) 555-1233"
                },
                new Student
                {
                    FirstName = "Jack", LastName = "Skellington", DOB = new DateTime(1988, 10, 31), CreatedBy = 1,
                    CreatedDate = new DateTime(2021, 4, 29), IsArchived = false, IsInstructor = false, BalanceDue = 0, BalanceDueDate = new DateTime(2021, 5, 29),
                    PaymentAgreenmentPeriod = PaymentPeriod.ThreeMonth, PaymentAgreementAmount = 360, Phone = "(715) 555-1233"
                },
            };

            db.Students.AddRange(students);
            db.SaveChanges();
        }

        /// <summary>
        /// DEPRECATED METHOD
        /// Creates the stored procedure to create a database backup.
        /// </summary>
        /// <param name="context">The database context to use.</param>
        private async static void CreateBackupStoredProcedure(CWBContext context)
        {
            string database = SystemConstants.DatabaseName;
            string DatetimeNow = "(SELECT FORMAT(GETDATE(), 'yyyyMMddHHmmss'))";

            // Directory is created before this hard-coded value is reached.
            string DirectoryAndName = $"'{SystemConstants.SubdirectoryPath}\\{database}_' + @DatetimeNow + '.bak'";

            string sql =
                $"CREATE PROCEDURE usp_DatabaseBackup\n" +
                $"AS\n" +
                $"DECLARE @DatetimeNow nvarchar(14)\n" +
                $"DECLARE @DirectoryAndName nvarchar(70)\n" +
                $"SET @DatetimeNow = {DatetimeNow}\n" +
                $"SET @DirectoryAndName = {DirectoryAndName} \n" +
                $"BACKUP DATABASE [{database}]\n" +
                $"TO DISK = @DirectoryAndName\n";

            await context.Database.ExecuteSqlRawAsync(sql);
        }       
    }
}
