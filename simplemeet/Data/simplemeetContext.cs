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

        public DbSet<simplemeet.Models.Topic> Topic { get; set; } = default!;

        public DbSet<simplemeet.Models.Comment> Comment { get; set; }

        public DbSet<simplemeet.Models.User> User { get; set; }
    }
}
