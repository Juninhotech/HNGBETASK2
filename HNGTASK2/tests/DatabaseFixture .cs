using HNGTASK2.Data;
using HNGTASK2.Models;
using Microsoft.EntityFrameworkCore;

namespace HNGTASK2.tests
{
    public class DatabaseFixture : IDisposable
    {
        public ApplicationDbContext Context { get; private set; }

        public DatabaseFixture()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new ApplicationDbContext(options);
            SeedTestData().Wait();
        }

        private async Task SeedTestData()
        {
            // Clear existing data
            Context.Users.RemoveRange(Context.Users);
            Context.Organisations.RemoveRange(Context.Organisations);
            Context.UserOrganisations.RemoveRange(Context.UserOrganisations);

            // Save changes to ensure database is empty
            await Context.SaveChangesAsync();

            var user1 = new User { UserId = "user1", Email = "user1@example.com", FirstName = "John", LastName = "Doe", Password = "Password1" };
            var user2 = new User { UserId = "user2", Email = "user2@example.com", FirstName = "Jane", LastName = "Smith", Password = "Password2" };

            var org1 = new Organisation { OrgId = "org1", Name = "Organisation 1", Description = "Description 1" };
            var org2 = new Organisation { OrgId = "org2", Name = "Organisation 2", Description = "Description 2" };

            var userOrg1 = new UserOrganisation { UserId = "user1", OrgId = "org1" };
            var userOrg2 = new UserOrganisation { UserId = "user2", OrgId = "org2" };

            await Context.Users.AddRangeAsync(user1, user2);
            await Context.Organisations.AddRangeAsync(org1, org2);
            await Context.UserOrganisations.AddRangeAsync(userOrg1, userOrg2);

            await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            // Context.Dispose();
            // Dispose of context if it hasn't been disposed yet
            if (Context != null)
            {
                Context.Dispose();
                Context = null; // Set to null to prevent accidental reuse
            }
        }
    }


}
