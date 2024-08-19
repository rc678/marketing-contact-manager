using MarketingContactManager.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketingContactManager.Contexts
{
    public class ContactContext : DbContext
    {
        public ContactContext(DbContextOptions<ContactContext> options) : base(options)
        {

        }

        public DbSet<ContactModel> Contacts { get; set; } = null!;
    }
}
