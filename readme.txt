The CWB Fight Club app is a .NET MVC 5 app using Entity Framework and designed to be utilized with SQL Server Express.

The majority of the application follows the standard MVC pattern with minor deviations when needed. 
The app utilizes bootstrap 4, JavaScript, jquery, css and razor pages to fullfil the front end design. 
The css styling is contained in a single site.css file, with additional styling coming from bootstrap classes.
The calendar utilizes a toast ui calendar implementation, with minor customization.
The report graphs are generated using chart.js version 3.

Incorporated NuGet packages include the standard .NET, EF and SQL related packages. 
libphonenumber-csharp was included to enhance phone number related operations for students and guardians.

The folder structure is set up with with all css and javascript stored in respective folders in wwwroot.
wwwroot also holds our image library for help graphics and student attachments.

Other notable areas beyond the normal Model, View and Controller folders would include the following:
Data - Database related files. Context definitions and initializers.
Services - Helper files utilized in dependency injection, broken down by the area they help.
Utilities - Helper files, including non-dependency injected, such as extention methods and constants libraries

Code summaries and comments have been included throughout the code base to decrease the learning curve for new developers.

---Application Design---

Color Scheme and logos match theme presented by the CWB Website as well as decor of the gym itself.
Buttons, tables, and navigation links also follow this theme through common styling.
CSS and bootstrap classes are in use to allow rapid develop of new areas when necessary.

Creating new students follows a workflow that is standardized to the new student workflow when logged in or not.
Checking students in also follows a workflow that looks familiar when logged in or not.

The login form takes users to directly to student index as this is likely to be the first stop when logging in.
Pagination and search on tables is consistent throughout where applicable on tables.

Within the student profile, a sub nav menu was put into place to allow fast navigation through related student areas.
All button groups are styled and aligned in similar ways for consistent feel.
Forms are also layed out in a consistent manner.

Checking the Is Instructor field on a student profile will allow that student to then be added as an instructor, allowing
that person to create a login profile.

Removing guardians from a student's profile where there are no other students currently using that guardian will result
in the guardian record being archived to keep data from becoming bloated.

When archiving a student record, all associated discipline, belt, attendance, payment, guardian, and detail records will
cascade archive with it and a modified date will be placed on the student.

Arching Disciplines will cascade archive belt information and associated classes as well as any active enrollment and 
attendance records.

When adding Disciplines, Calendar Color field can be set which will then be used on all classes that are created with that
associted discipline. If the calendar color is outside of a safe range for black or white text, the text color will be 
flipped for accessibility.

Adding a discipline where one already exists will add on to the balance owed and maintain the same due date.
Adding a gym package will change the term of payment but again maintain the same existing due date when one already exists.

All enrollment records can be viewed from the Disciplines nav menu > Discipline > Enrollments tab.

Ending an enrollment record allows all historical information to be kept for students, but deleting it will archive all 
associated information.

Adding a new student when not logged in will redirect to the login page. Adding a new student when logged in will redirect to
the Payments section of their profile.

CRUD functions of the calendar are automatically saved.

Reports all have a max number of 7 or fewer positions on the chart but the text representation is unlimited.

Students may check in up to 30 minutes before a class begins, once that window has approached students enrolled in the next
most recent class will display in the Enrolled Students section for easier access.