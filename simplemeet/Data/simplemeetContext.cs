using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using simplemeet.Models;

namespace simplemeet.Data
{
    public class simplemeetContext : DbContext
    {
        public simplemeetContext (DbContextOptions<simplemeetContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>()
                .HasOne<User>(s => s.User)
                .WithMany(g => g.Comments)
                .HasForeignKey(s => s.UserId);
        }
        public DbSet<simplemeet.Models.Topic> Topic { get; set; } = default!;

        public DbSet<simplemeet.Models.Comment> Comment { get; set; }

        public DbSet<simplemeet.Models.User> User { get; set; }
    }
}
