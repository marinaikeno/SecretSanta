using Microsoft.EntityFrameworkCore;

namespace SecretSanta.Models
{
    public class SSContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public SSContext(DbContextOptions<SSContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<WishItem> WishItems { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<SecretSantaModel> SecretSantaModels { get; set; }
    }
}