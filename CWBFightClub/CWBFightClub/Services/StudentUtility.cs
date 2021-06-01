using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CWBFightClub.Data;
using CWBFightClub.Models;
using CWBFightClub.Utilities;
using PhoneNumbers;

namespace CWBFightClub.Services
{
    /// <summary>
    /// The class used for business logic for the student object.
    /// </summary>
    public class StudentUtility : IStudentUtility
    {
        /// <summary>
        /// The phone utility.
        /// </summary>
        private readonly PhoneNumberUtil _phoneUtil;

        /// <summary>
        /// Initializes a new instance of the StudentUtility class.
        /// </summary>
        public StudentUtility()
        {
            this._phoneUtil = PhoneNumberUtil.GetInstance();
        }

        /// <summary>
        /// Verifies attendance records for a  list of students.
        /// Attendance is verifies based on app settings.
        /// </summary>
        /// <param name="students">List of students to verify.</param>
        /// <param name="db">The db context.</param>
        public void VerifyAttendanceRecord(IEnumerable<Student> students, CWBContext db)
        {
            AppSetting appSetting = db.AppSettings.Where(x => x.AppSettingID > 0).FirstOrDefault();
            foreach (Student s in students)
            {
                foreach (AttendanceRecord ar in s.AttendanceRecords.Where(x => !x.IsArchived && x.IsVerified == false))
                {
                    TimeSpan classDuration = ar.ScheduledClass.End - ar.ScheduledClass.Start;
                    TimeSpan timeInClass = ar.End - ar.Start;

                    if (timeInClass >= classDuration * (appSetting.PercentOfClassRequiredToVerify / 100))
                    {
                        ar.IsVerified = true;
                        db.AttendanceRecords.Update(ar);
                    }
                }
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Updates the student balance information based on due date, current date, and payment agreement information.
        /// </summary>
        /// <param name="student">The student to update.</param>
        /// <param name="db">The db context.</param>
        public void UpdateStudentBalance(Student student, CWBContext db)
        {
            if (student.PaymentAgreementAmount > 0 && 
                student.BalanceDueDate.HasValue && 
                student.BalanceDueDate.Value <= DateTime.Now)
            {
                int monthsPast;
                switch (student.PaymentAgreenmentPeriod)
                {
                    case PaymentPeriod.Monthly:
                        monthsPast = DateTime.Now.GetTotalMonthsFrom(student.BalanceDueDate.Value);
                        
                        for (int i = 0; i < monthsPast + 1; i++)
                        {
                            student.BalanceDue = student.BalanceDue.HasValue ? student.BalanceDue + student.PaymentAgreementAmount : student.PaymentAgreementAmount;
                        }

                        student.BalanceDueDate = student.BalanceDueDate.Value.AddMonths(monthsPast + 1);
                        student.BalanceModifiedBySystemDate = DateTime.Now;

                        break;
                    case PaymentPeriod.ThreeMonth:
                        monthsPast = DateTime.Now.GetTotalMonthsFrom(student.BalanceDueDate.Value) / 3;

                        for (int i = 0; i < monthsPast + 1; i++)
                        {
                            student.BalanceDue = student.BalanceDue.HasValue ? student.BalanceDue + student.PaymentAgreementAmount : student.PaymentAgreementAmount;
                        }

                        student.BalanceDueDate = student.BalanceDueDate.Value.AddMonths((monthsPast + 1) * 3);
                        student.BalanceModifiedBySystemDate = DateTime.Now;

                        break;
                    case PaymentPeriod.Yearly:
                        monthsPast = DateTime.Now.GetTotalMonthsFrom(student.BalanceDueDate.Value) / 12;

                        for (int i = 0; i < monthsPast + 1; i++)
                        {
                            student.BalanceDue = student.BalanceDue.HasValue ? student.BalanceDue + student.PaymentAgreementAmount : student.PaymentAgreementAmount;
                        }

                        student.BalanceDueDate = student.BalanceDueDate.Value.AddMonths((monthsPast + 1) * 12);
                        student.BalanceModifiedBySystemDate = DateTime.Now;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Unrecognized payment interval.");
                }

                db.Students.Update(student);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Update the balance due on a list of students.
        /// </summary>
        /// <param name="students">The list of students to update.</param>
        /// <param name="db">The db context.</param>
        public void UpdateStudentBalance(IEnumerable<Student> students, CWBContext db)
        {
            foreach(Student student in students)
            {
                if (student.PaymentAgreementAmount > 0 &&
                student.BalanceDueDate.HasValue &&
                student.BalanceDueDate.Value <= DateTime.Now)
                {
                    int monthsPast;
                    switch (student.PaymentAgreenmentPeriod)
                    {
                        case PaymentPeriod.Monthly:
                            monthsPast = DateTime.Now.GetTotalMonthsFrom(student.BalanceDueDate.Value);

                            for (int i = 0; i < monthsPast + 1; i++)
                            {
                                student.BalanceDue = student.BalanceDue.HasValue ? student.BalanceDue + student.PaymentAgreementAmount : student.PaymentAgreementAmount;
                            }

                            student.BalanceDueDate = student.BalanceDueDate.Value.AddMonths(monthsPast + 1);
                            student.BalanceModifiedBySystemDate = DateTime.Now;

                            break;
                        case PaymentPeriod.ThreeMonth:
                            monthsPast = DateTime.Now.GetTotalMonthsFrom(student.BalanceDueDate.Value) / 3;

                            for (int i = 0; i < monthsPast + 1; i++)
                            {
                                student.BalanceDue = student.BalanceDue.HasValue ? student.BalanceDue + student.PaymentAgreementAmount : student.PaymentAgreementAmount;
                            }

                            student.BalanceDueDate = student.BalanceDueDate.Value.AddMonths((monthsPast + 1) * 3);
                            student.BalanceModifiedBySystemDate = DateTime.Now;

                            break;
                        case PaymentPeriod.Yearly:
                            monthsPast = DateTime.Now.GetTotalMonthsFrom(student.BalanceDueDate.Value) / 12;

                            for (int i = 0; i < monthsPast + 1; i++)
                            {
                                student.BalanceDue = student.BalanceDue.HasValue ? student.BalanceDue + student.PaymentAgreementAmount : student.PaymentAgreementAmount;
                            }

                            student.BalanceDueDate = student.BalanceDueDate.Value.AddMonths((monthsPast + 1) * 12);
                            student.BalanceModifiedBySystemDate = DateTime.Now;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Unrecognized payment interval.");
                    }

                    db.Students.Update(student);
                    
                }
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Checks if a phone number string is a valid phone number.
        /// </summary>
        /// <param name="phoneNumberToCheck">The string to check.</param>
        /// <returns>True if valid.</returns>
        public bool PhoneNumberIsValid(string phoneNumberToCheck)
        {
            bool valid;
            
            try
            {
                PhoneNumber phoneNumber = this._phoneUtil.Parse(phoneNumberToCheck, SystemConstants.PhoneCountryCode);
                valid = this.PhoneNumberIsValid(phoneNumber);
            }
            catch (NumberParseException npex)
            {
                Debug.Write("Phone", $"{npex.ErrorType}: {npex.Message}");
                valid = false;
            }

            return valid;
        }

        /// <summary>
        /// Determines if a phone number is valid.
        /// </summary>
        /// <param name="phoneNumberToCheck">The phone number to check.</param>
        /// <returns>True if valid.</returns>
        public bool PhoneNumberIsValid(PhoneNumber phoneNumberToCheck)
        {
            return this._phoneUtil.IsValidNumberForRegion(phoneNumberToCheck, SystemConstants.PhoneCountryCode);
        }

        /// <summary>
        /// Converts a string to a format usable by a mobile device.
        /// </summary>
        /// <param name="unformattedNumber">The unformatted string to format.</param>
        /// <returns>The mobile format for the number is returned.</returns>
        public string ConvertNumberToMobileFormat(string unformattedNumber)
        {
            string numberInMobileFormat = string.Empty;
            try
            {
                PhoneNumber phoneNumber = this._phoneUtil.Parse(unformattedNumber, SystemConstants.PhoneCountryCode);
                numberInMobileFormat = this._phoneUtil.FormatInOriginalFormat(phoneNumber, SystemConstants.PhoneCountryCode);
            }
            catch (NumberParseException npex)
            {
                Debug.Write("Phone", $"{npex.ErrorType}: {npex.Message}");
            }

            return numberInMobileFormat;
        }
    }
}
