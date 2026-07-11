using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using ToDoList.Entities;

namespace ToDoList.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        public DbSet<TaskItem> Tasks { get; set; }

        public DbSet<Status> Statuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Status>().HasData(
                new Status { StatusId = 1, StatusName = "Pending" },
                new Status { StatusId = 2, StatusName = "In Progress" },
                new Status { StatusId = 3, StatusName = "Completed" },
                new Status { StatusId = 4, StatusName = "Cancelled" }
            );
        }
    }
}
