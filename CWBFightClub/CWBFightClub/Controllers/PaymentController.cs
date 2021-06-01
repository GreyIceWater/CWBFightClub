using CWBFightClub.Data;
using CWBFightClub.Models;
using CWBFightClub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWBFightClub.Controllers
{
    public class PaymentController : BaseController
    {
        private IStudentUtility _studentUtility;
        public PaymentController(CWBContext db, IAccessChecker ac, IStudentUtility studentUtility)
            : base(ac, db)
        {
            _studentUtility = studentUtility;
        }

        // List of payments and balance related information for a student. id = student ID.
        public async Task<IActionResult> Student(int id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Student student = await _db.Students.FindAsync(id);

            if (student == null || student.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Student).Name, message = "Read failed." });
            }

            _studentUtility.UpdateStudentBalance(student, _db);

            student = await _db.Students
                                 .Where(x => x.IsArchived == false && x.StudentID == id)
                                 .Include(x => x.Payments.Where(y => y.IsArchived == false).OrderByDescending(x => x.ReceivedDate))
                                 .AsNoTracking().FirstOrDefaultAsync();



            return View(student);
        }

        // Add a payment and adjust the balance due by that amount. Student information contained in payment's studentID property.
        [HttpPost]
        public async Task<IActionResult> Add(Payment payment)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_Add");
            }

            AssignCreator(payment);

            await _db.Payments.AddAsync(payment);

            Student student = await _db.Students.FindAsync(payment.StudentID);
            
            if (student.BalanceDue.HasValue)
            {
                student.BalanceDue -= payment.Amount;
            }
            else
            {
                student.BalanceDue = payment.Amount * -1;
            }

            _db.Students.Update(student);
            _db.SaveChanges();

            return PartialView("_AddSuccess");
        }

        // Get the partial view for editting a payment.
        public async Task<IActionResult> Edit(int? id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Payment payment = await _db.FindAsync<Payment>(id);

            if (payment == null || payment.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Payment).Name, message = "Read failed." });
            }

            return PartialView("_Edit", payment);
        }

        // Process the payment changes and save them to the db.
        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost(Payment payment)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Payment paymentFound = await _db.Payments.FindAsync(payment.PaymentID);

            if (paymentFound is null || paymentFound.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Payment).Name, message = "Edit failed." });
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_Edit",paymentFound);
            }

            if (!await TryUpdateModelAsync(
                paymentFound,
                "",
                x => x.ReceivedDate,
                x => x.Amount,
                x => x.Note))
            {

                return PartialView(paymentFound);
            }

            AssignModifier(paymentFound);

            _db.Payments.Update(paymentFound);
            _db.SaveChanges();

            return RedirectToAction("Student", new { id = paymentFound.StudentID });
        }

        // Process balance due, payment agreenment amount, due date, payment period and general account notes edits.
        [HttpPost]
        public async Task<IActionResult> EditBalance(Student student)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Student studentFound = await _db.Students.FindAsync(student.StudentID);

            if (studentFound is null || studentFound.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Student).Name, message = "Edit failed." });
            }

            studentFound.BalanceDue = student.BalanceDue;
            studentFound.PaymentAgreementAmount = student.PaymentAgreementAmount;
            studentFound.BalanceDueDate = student.BalanceDueDate;
            studentFound.PaymentAgreenmentPeriod = student.PaymentAgreenmentPeriod;
            studentFound.PaymentAgreementNote = student.PaymentAgreementNote;

            AssignModifier(studentFound);

            _db.Students.Update(studentFound);
            _db.SaveChanges();

            return RedirectToAction("Student", new { id = studentFound.StudentID });
        }

        // Archive a payment and adjust the balance due by the same amount.
        public async Task<IActionResult> Delete(int id)
        {
            IActionResult checkResult = this.AccessChecker.CheckForAccess();
            if (checkResult != null)
            {
                return checkResult;
            }

            Payment payment = await _db.Payments.FindAsync(id);
            if (payment is null || payment.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Payment).Name, message = "Delete failed." });
            }

            Student student = await _db.Students.FindAsync(payment.StudentID);
            if (student is null || student.IsArchived == true)
            {
                return RedirectToAction("ObjectNotFound", new { type = typeof(Student).Name, message = "Read failed." });
            }

            AssignModifier(payment);
            payment.IsArchived = true;

            // Deleting a payment currently increases the student balance due by that same amount.
            student.BalanceDue += payment.Amount;

            _db.Payments.Update(payment);
            _db.Students.Update(student);
            _db.SaveChanges();
            return RedirectToAction("Student", "Payment", new { id = payment.StudentID });
        }
    }
}
