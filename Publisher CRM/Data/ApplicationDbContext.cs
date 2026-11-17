using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Publisher_CRM.Models;

namespace Publisher_CRM.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Account { get; set; }
        public DbSet<Postcode> Postcodes { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Salescall> Salescall { get; set; }
        public DbSet<OrderManagementViewModel> orderManagementViewModels { get; set; }
        public DbSet<LeadViewModel> LeadViewModels { get; set; }
        public DbSet<RetailerPointview> RetailerPointviews { get; set; }
        public DbSet<Complaints> Complaint { get; set; }
        public DbSet<EventMaster> EventMaster { get; set; }
        public DbSet<Files> File { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SalesCallEnquiryItem> SalesCallEnquiryItems { get; set; }

        // --- FIXED: New Beat Route & Plan Tables ---
        public DbSet<BeatRoute> BeatRoutes { get; set; }
        public DbSet<BeatRouteCustomer> BeatRouteCustomers { get; set; }
        public DbSet<BeatPlan> BeatPlans { get; set; }
        public DbSet<PlanMeeting> PlanMeetings { get; set; }

        public DbSet<DailyPlan> DailyPlans { get; set; }
        public DbSet<DailyPlanCustomer> DailyPlanCustomers { get; set; }
        public DbSet<DailyPlanContact> DailyPlanContacts { get; set; }

        public DbSet<ProjectTitle> ProjectTitles { get; set; }

        public object Order { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "CEO", ParentRoleId = null },
                new Role { Id = 2, Name = "Sales Manager", ParentRoleId = 1 },
                new Role { Id = 3, Name = "Sales Executive", ParentRoleId = 2 }
            );

            modelBuilder.Ignore<SelectListItem>();
            modelBuilder.Ignore<SelectListGroup>();

            modelBuilder.Entity<Contact>()
                .HasIndex(c => c.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL AND [Email] <> ''");


            // 1. Prevent cycle between User -> BeatPlan
            modelBuilder.Entity<BeatPlan>()
                .HasOne(b => b.SalesPerson)
                .WithMany()
                .HasForeignKey(b => b.SalesPersonId)
                .OnDelete(DeleteBehavior.NoAction);

            // 2. Prevent cycle between Account -> PlanMeeting
            modelBuilder.Entity<PlanMeeting>()
                .HasOne(p => p.Customer)
                .WithMany()
                .HasForeignKey(p => p.AccountId)
                .OnDelete(DeleteBehavior.NoAction);

            // 3. Prevent cycle between Account -> BeatRouteCustomer
            modelBuilder.Entity<BeatRouteCustomer>()
                .HasOne(b => b.Customer)
                .WithMany()
                .HasForeignKey(b => b.AccountId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DailyPlanCustomer>()
                .HasOne(d => d.Customer).WithMany()
                .HasForeignKey(d => d.AccountId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DailyPlanContact>()
                .HasOne(d => d.Contact).WithMany()
                .HasForeignKey(d => d.ContactId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DailyPlan>()
                .HasOne(d => d.CreatedBy).WithMany()
                .HasForeignKey(d => d.CreatedById).OnDelete(DeleteBehavior.NoAction);
        }
    }
}