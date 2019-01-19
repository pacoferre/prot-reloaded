using Demo.Library.Business.Security.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Library.Business.Data
{
    public class LibraryContextSeed
    {
        public static async Task SeedAsync(LibraryContext libraryContext,
            ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            try
            {
                libraryContext.Database.Migrate();

                if (!libraryContext.Users.Any())
                {
                    libraryContext.Users.Add(new LibraryAppUserModel
                    {
                        Name = "Demo",
                        Surname = "User",
                        Email = "demo@demo.com",
                        Password = "demo",
                    });

                    await libraryContext.SaveChangesAsync();
                }

                if (!libraryContext.AuthorNationalities.Any())
                {
                    libraryContext.AuthorNationalities.Add(new Business.Authors.EF.AuthorNationality()
                    {
                        Name = "Spain"
                    });
                    libraryContext.AuthorNationalities.Add(new Business.Authors.EF.AuthorNationality()
                    {
                        Name = "France"
                    });
                    libraryContext.AuthorNationalities.Add(new Business.Authors.EF.AuthorNationality()
                    {
                        Name = "United Kingdom"
                    });
                    libraryContext.AuthorNationalities.Add(new Business.Authors.EF.AuthorNationality()
                    {
                        Name = "Germany"
                    });
                    libraryContext.AuthorNationalities.Add(new Business.Authors.EF.AuthorNationality()
                    {
                        Name = "EE.UU."
                    });
                    libraryContext.AuthorNationalities.Add(new Business.Authors.EF.AuthorNationality()
                    {
                        Name = "Italy"
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
