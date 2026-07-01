# BankITDeskHelp

A web-based IT Help Desk Complaint Management System built for banking sector operations. The system enables employees to submit IT-related complaints, allows IT Admins to triage and assign tickets to Managers, and lets Managers resolve issues with a complete audit trail at every stage.

## Overview

BankITDeskHelp digitizes the internal IT support workflow of a bank, replacing manual complaint handling with a structured, role-based ticketing system. It is built on ASP.NET Core MVC (.NET 8) with Entity Framework Core and SQL Server, following a clean three-role architecture: **Employee**, **Manager**, and **Admin**.

## Features

- Public complaint submission with auto-generated ticket numbers (e.g. `IT-2026-00001`)
- Role-based access control using ASP.NET Core Identity (Admin, Manager, Employee)
- Admin dashboard with live ticket statistics and a complaint queue
- Ticket assignment from Admin to Manager
- Manager workspace to start work, add comments, and resolve tickets
- Admin verification step to close or reassign resolved tickets
- Full complaint history and audit timeline per ticket
- File attachment support on complaint submission
- Departments, Branches, and Categories as configurable master data

## Complaint Lifecycle

```
New → Assigned → In Progress → Resolved → Closed
                                   ↑           |
                                   └── Reassigned (if rejected by Admin)
```

| Status      | Triggered By | Description                                  |
|-------------|--------------|-----------------------------------------------|
| New         | Employee     | Complaint submitted via public form           |
| Assigned    | Admin        | Ticket assigned to a Manager                  |
| In Progress | Manager      | Manager has started working on the ticket     |
| Resolved    | Manager      | Manager has resolved the issue with remarks   |
| Closed      | Admin        | Admin verifies and closes the ticket          |

## Tech Stack

| Layer          | Technology                                  |
|----------------|----------------------------------------------|
| Backend        | ASP.NET Core MVC (.NET 8)                   |
| ORM            | Entity Framework Core                        |
| Database       | SQL Server (SQLEXPRESS / LocalDB)            |
| Authentication | ASP.NET Core Identity                        |
| Frontend       | Bootstrap 5                                  |
| Architecture   | MVC (Model-View-Controller)                  |

## Project Structure

```
BankITDeskHelp/
├── Controllers/
│   ├── AccountController.cs       # Login / Logout
│   ├── AdminController.cs         # Admin dashboard, assignment, verification
│   ├── ComplaintController.cs     # Public complaint submission
│   └── ManagerController.cs       # Manager dashboard and resolution flow
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext
│   └── SeedData.cs                # Roles, master data, and default user seeding
├── Models/
│   ├── ApplicationUser.cs         # Extended Identity user
│   ├── Department.cs
│   ├── Branch.cs
│   ├── Category.cs
│   ├── Complaint.cs
│   ├── ComplaintHistory.cs
│   ├── Comment.cs
│   ├── Attachment.cs
│   └── Enums.cs                   # PriorityLevel, ComplaintStatus
├── ViewModels/
│   ├── ComplaintCreateViewModel.cs
│   ├── LoginViewModel.cs
│   ├── AssignComplaintViewModel.cs
│   ├── ManagerTicketDetailsViewModel.cs
│   └── AdminVerifyViewModel.cs
├── Views/
│   ├── Account/
│   ├── Admin/
│   ├── Complaint/
│   └── Manager/
└── Migrations/
```

## Database Schema

Key entities and relationships:

- **Complaint** — central entity holding employee details, category, priority, status, and assigned manager
- **ComplaintHistory** — append-only audit log of every state change on a complaint
- **Comment** — manager/admin remarks attached to a complaint
- **Attachment** — uploaded files linked to a complaint
- **Department / Branch / Category** — master data used in complaint classification
- **ApplicationUser** — extends `IdentityUser`, used by Admin and Manager accounts

## Getting Started

### Prerequisites

- Visual Studio 2022
- .NET 8 SDK
- SQL Server (Express or LocalDB)

### Setup

1. Clone the repository
   ```
   git clone https://github.com/abdullahkhan-cs/BankITDeskHelp.git
   ```

2. Update the connection string in `appsettings.json`
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=BankITDeskHelpDB2;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```

3. Apply migrations
   ```
   Update-Database
   ```

4. Run the application (F5 in Visual Studio)

On first run, the application automatically seeds:
- Roles: `Admin`, `Manager`, `Employee`
- Master data: Departments, Branches, Categories
- Default Admin and Manager accounts

### Default Credentials

| Role    | Email                     | Password     |
|---------|---------------------------|--------------|
| Admin   | admin@bankitdesk.com      | Admin@123    |
| Manager | manager@bankitdesk.com    | Manager@123  |

> Change these credentials before deploying to any non-local environment.

## Usage

1. **Employee** submits a complaint via the public form — no login required.
2. **Admin** logs in, reviews new complaints, and assigns them to a Manager.
3. **Manager** logs in, starts work on assigned tickets, adds comments, and resolves them.
4. **Admin** reviews resolved tickets and either closes them or reassigns them back to the Manager.

## Roadmap

- Public complaint tracking (ticket lookup without login)
- Email and SMS notifications
- SLA tracking and auto-escalation
- Reports export (PDF / Excel)
- Manager performance dashboard
- Mobile-responsive UI refresh

## Author

**Abdullah Khan**
Final-year BS Computer Science student, QUEST, Nawabshah
[GitHub](https://github.com/abdullahkhan-cs) · [Portfolio](https://abdullahkhan-cs.github.io)

## License

This project is intended for educational and portfolio purposes.
