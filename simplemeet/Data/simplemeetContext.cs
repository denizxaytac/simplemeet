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
            // one comment have one user, one user can have multiple comments
            modelBuilder.Entity<Comment>()
                .HasOne<User>(s => s.User)
                .WithMany(g => g.Comments)
                .HasForeignKey(s => s.UserId);
            modelBuilder.Entity<Topic>()
               .HasOne<User>(s => s.Creator)
               .WithMany(g => g.CreatedTopics)
               .HasForeignKey(s => s.CreatorId)
               .OnDelete(DeleteBehavior.ClientNoAction);
        }
        public DbSet<simplemeet.Models.Topic> Topic { get; set; } = default!;

        public DbSet<simplemeet.Models.Comment> Comment { get; set; }

        public DbSet<simplemeet.Models.User> User { get; set; }

        public DbSet<simplemeet.Models.Vote> Vote { get; set; }
    }
}
