using HNGTASK2.Models;
using Microsoft.EntityFrameworkCore;

namespace HNGTASK2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { } // Add this parameterless constructor

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<UserOrganisation> UserOrganisations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasColumnName("userid");
                entity.Property(e => e.FirstName).HasColumnName("firstname");
                entity.Property(e => e.LastName).HasColumnName("lastname");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Password).HasColumnName("password");
                entity.Property(e => e.Phone).HasColumnName("phone");

                // Ensure the combination of UserId and Email is unique
                entity.HasIndex(e => new { e.UserId, e.Email }).IsUnique();
            });

            modelBuilder.Entity<Organisation>(entity =>
            {
                entity.ToTable("organisations");
                entity.HasKey(e => e.OrgId);
                entity.Property(e => e.OrgId).HasColumnName("orgid");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Description).HasColumnName("description");

                // Ensure OrgId is unique
                entity.HasIndex(e => e.OrgId).IsUnique();
            });

            modelBuilder.Entity<UserOrganisation>(entity =>
            {
                entity.ToTable("userorganisations");
                entity.HasKey(e => new { e.UserId, e.OrgId });
                entity.Property(e => e.UserId).HasColumnName("userid");
                entity.Property(e => e.OrgId).HasColumnName("orgid");

                entity.HasOne(uo => uo.User)
                    .WithMany(u => u.UserOrganisations)
                    .HasForeignKey(uo => uo.UserId);

                entity.HasOne(uo => uo.Organisation)
                    .WithMany(o => o.UserOrganisations)
                    .HasForeignKey(uo => uo.OrgId);
            });
        }
    }
}
