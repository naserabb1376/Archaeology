using Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class ArchaeologyDbContext : IdentityDbContext<User, UserRole, int>
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<ContextParagraph> ContextParagraphs { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Context> Contexts { get; set; }
        public DbSet<ContextImage> ContextImages { get; set; }
        public DbSet<ContextTable> ContextTables { get; set; }
        public DbSet<ContextTableColumn> ContextTableColumns { get; set; }
        public DbSet<ContextTableRow> ContextTableRows { get; set; }
        public DbSet<ContextTableCell> ContextTableCells { get; set; }
        public DbSet<ContextDisplayItem> ContextDisplayItems { get; set; }

        public ArchaeologyDbContext(DbContextOptions<ArchaeologyDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserLogin<int>>().HasKey(p => p.UserId);
            modelBuilder.Entity<IdentityUserRole<int>>().HasKey(p => new { p.UserId, p.RoleId });
            modelBuilder.Entity<IdentityUserToken<int>>().HasKey(p => new { p.UserId });
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ContextTableColumn>()
                .HasOne(c => c.ParentColumn)
                .WithMany(p => p.SubColumns)
                .HasForeignKey(c => c.ParentColumnId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ContextTableCell>()
                .HasOne(c => c.Column)
                .WithMany()
                .HasForeignKey(c => c.ColumnId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Ignore<BaseEntity>();
        }
    }
}