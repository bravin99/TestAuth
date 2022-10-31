using Microsoft.EntityFrameworkCore;
using TestAuth.Server.Models;

namespace TestAuth.Server.Context
{
    public class TestAuthContext : DbContext
    {
        public TestAuthContext(DbContextOptions options) : base(options)
        {}

        public DbSet<User>? Users { get; set; }
        public DbSet<JWT>? Tokens { get; set; }
    }
}