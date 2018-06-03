using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Library.Infrastructure.Data
{
    public class LibraryContextSeed
    {
        public static async Task SeedAsync(LibraryContext libraryContext,
            ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            try
            {
                libraryContext.Database.EnsureCreated();

                if (!libraryContext.Users.Any())
                {
                    libraryContext.Users.Add(new PROTR.Core.Security.EF.AppUser
                    {
                        Name = "Demo",
                        Surname = "User",
                        Email = "demo@demo.com",
                        Password = "demo",
                    });

                    await libraryContext.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger<LibraryContextSeed>();
                    log.LogError(ex.Message);
                    await SeedAsync(libraryContext, loggerFactory, retryForAvailability);
                }
            }
        }
    }
}
