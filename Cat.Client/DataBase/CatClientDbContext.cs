using Cat.Client.Care;
using Microsoft.EntityFrameworkCore;

namespace Cat.Client.DataBase
{
    public class CatClientDbContext : DbContext
    {
        public DbSet<ChatServer> ChatServers { get; set; } 

        public DbSet<DefaultServer> DefaultServers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionDb = $"Filename={PathDB.GetPath("cat_client.db")}";
            optionsBuilder.UseSqlite(connectionDb);
        }
    }
}
