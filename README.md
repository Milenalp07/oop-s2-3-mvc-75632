# VgcCollege Web Application

## Overview

VgcCollege is an ASP.NET Core MVC web application designed to manage a college environment, including students, faculty, courses, enrolments, assignments, exams, and attendance.

The system uses:

* ASP.NET Core MVC (.NET 8)
* Entity Framework Core (SQLite)
* ASP.NET Identity (Authentication & Roles)
* Serilog (Logging)
* xUnit (Testing)

---

## Features

* Role-based dashboards (Admin, Faculty, Student)
* Course management
* Student enrolments
* Assignment & exam results
* Attendance tracking
* Logging with Serilog
* Basic unit tests

---

## Default Logins

### Admin

* Email: [admin@vgc.ie](mailto:admin@vgc.ie)
* Password: Admin123!

### Faculty

* Email: [faculty1@vgc.ie](mailto:faculty1@vgc.ie)
* Password: Password123!

### Student

* Email: [student1@vgc.ie](mailto:student1@vgc.ie)
* Password: Password123!

---

## Running the Application

1. Restore packages:

```
dotnet restore
```

2. Apply database:

```
dotnet ef database update
```

3. Run:

```
dotnet run
```

4. Open in browser:

```
https://localhost:xxxx
```

---

## Tests

Run tests using:

```
dotnet test
```

Includes:

* Model validation tests
* Controller tests
* In-memory database testing

---

## Notes

* Database: SQLite (`vgccollege.db`)
* Logs are stored in `/Logs`
* Seed data runs automatically on startup

---

## Author

VgcCollege Project – OOP / ASP.NET MVC Assignment
