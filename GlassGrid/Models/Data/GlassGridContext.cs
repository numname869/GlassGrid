using Microsoft.EntityFrameworkCore;
using GlassGrid.Models;

namespace GlassGrid.Models.Data 
{
    public class GlassGridContext : DbContext
    {
        public GlassGridContext(DbContextOptions<GlassGridContext> options)
            : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ScoreModel> Scores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasMany(u => u.Scores)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<ScoreModel>()
                .Property(s => s.Difficulty)
                .HasConversion<string>();
        }

   

    }
}
