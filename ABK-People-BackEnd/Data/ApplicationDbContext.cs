using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ABK_People_BackEnd.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ABK_People_BackEnd.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<VacationRequest> VacationRequests { get; set; }
        public DbSet<ComplaintRequest> ComplaintRequests { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageFile> MessageFiles { get; set; }
        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure inheritance
            builder.Entity<User>()
                .HasDiscriminator<string>("Role")
                .HasValue<Admin>("Admin")
                .HasValue<Employee>("Employee");

            builder.Entity<Request>()
                .HasDiscriminator<string>("RequestType")
                .HasValue<VacationRequest>("Vacation")
                .HasValue<ComplaintRequest>("Complaint");

            // Configure relationships to avoid cascade cycles
            builder.Entity<Message>()
                .HasOne(m => m.Request)
                .WithMany(r => r.Messages)
                .HasForeignKey(m => m.RequestId)
                .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade to Restrict

            builder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Request>()
                .HasOne(r => r.Employee)
                .WithMany(e => e.Requests)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MessageFile>()
                .HasOne(mf => mf.Message)
                .WithMany(m => m.Files)
                .HasForeignKey(mf => mf.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data for Admins and Employees
            var hasher = new PasswordHasher<User>();

            // Seed data
            builder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = "admin1",
                    UserName = "admin@company.com",
                    Email = "admin@company.com",
                    FirstName = "Abdulaziz",
                    LastName = "Marafi",
                    Position = User.EmployeePosition.ChiefOfficer,
                    NormalizedEmail = "ADMIN@COMPANY.COM",
                    NormalizedUserName = "ADMIN@COMPANY.COM",
                    PasswordHash = hasher.HashPassword(null, "Employee@123")
                },
                new Admin
                {
                    Id = "admin2",
                    UserName = "admin2@company.com",
                    Email = "admin2@company.com",
                    FirstName = "Ahmad",
                    LastName = "Damra",
                    Position = User.EmployeePosition.GeneralManager,
                    NormalizedEmail = "ADMIN2@COMPANY.COM",
                    NormalizedUserName = "ADMIN2@COMPANY.COM",
                    PasswordHash = hasher.HashPassword(null, "Admin@123")
                }
            );

            builder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = "emp1",
                    UserName = "employee@company.com",
                    Email = "employee@company.com",
                    FirstName = "John",
                    LastName = "Doe",
                    Position = User.EmployeePosition.Manager,
                    Department = Employee.EmployeeDepartment.IT,
                    VacationDays = 30,
                    SickDays = 15,
                    NormalizedEmail = "EMPLOYEE@COMPANY.COM",
                    NormalizedUserName = "EMPLOYEE@COMPANY.COM",
                    PasswordHash = hasher.HashPassword(null, "Employee@123")
                },
                new Employee
                {
                    Id = "emp2",
                    UserName = "employee2@company.com",
                    Email = "employee2@company.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    Position = User.EmployeePosition.SeniorOfficer,
                    Department = Employee.EmployeeDepartment.HR,
                    VacationDays = 25,
                    SickDays = 10,
                    NormalizedEmail = "EMPLOYEE2@COMPANY.COM",
                    NormalizedUserName = "EMPLOYEE2@COMPANY.COM",
                    PasswordHash = hasher.HashPassword(null, "Employee@123")
                }
            );
        }
    }
}

