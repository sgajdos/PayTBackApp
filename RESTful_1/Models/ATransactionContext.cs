using Microsoft.EntityFrameworkCore;
using RESTful_1.Enumeration;

namespace RESTful_1.Models
{
    public class ATransactionContext:DbContext
    {
        public ATransactionContext(DbContextOptions<ATransactionContext> options) : base() { }

        public virtual DbSet<ATransaction> ATransactionSet { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=K56CB\\SQLEXPRESS;Database=ATransactionContext;TrustServerCertificate=True;Trusted_Connection=True;MultipleActiveResultSets=true",
                    builder => builder.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ATransaction>()
                .Property(e => e.Status)
                .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
