using Microsoft.EntityFrameworkCore;
using PaystackIntegrationPracticeApi.Models;

namespace PaystackIntegrationPracticeApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<PaystackIntegration> PaystackIntegration => Set<PaystackIntegration>();
    }
}
