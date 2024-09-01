using Microsoft.EntityFrameworkCore;

namespace App.Jensen.Models
{
    public class ContextUser : DbContext 
    {
        public ContextUser(DbContextOptions<ContextUser> options) : base(options)
            {          
        }
        public DbSet<User> Users { get; set; }
    }
}
