using CsvHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Opifex.Rsvp.Data
{
    public class SeedData
    {
        public static void Run(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                var config = host.Services.GetRequiredService<IConfiguration>();
                var userList = FetchData();

                Initialize(services, userList, context).Wait();

            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred adding users.");
            }
        }

        public static IEnumerable<SeedDataRow> FetchData()
        {
            var data = new List<SeedDataRow>();
            using (var reader = new StreamReader("wwwroot\\InitialData.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    data.Add(new SeedDataRow
                    {
                        UserName = csv.GetField<string>("EmailAddress"),
                        DisplayName = csv.GetField<string>("DisplayName"),
                        Role = csv.GetField<string>("Role"),
                        MaxGuests = csv.GetField<int>("MaxGuests"),
                        OptionCombinations = csv.GetField<RsvpOptions>("RsvpOption") | RsvpOptions.NotAttending,
                    });
                }
            }
            return data;
        }

        public static async Task Initialize(IServiceProvider serviceProvider,
            IEnumerable<SeedDataRow> rows, ApplicationDbContext context)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetService<RoleManager<ApplicationRole>>();

            await EnsureRole(roleManager, "Administrator");
            await EnsureRole(roleManager, "Manager");

            foreach (var row in rows)
            {
                var userId = await EnsureUser(userManager, row.UserName, row.DisplayName, row.Role);
                await EnsureRsvpConstraint(serviceProvider, userId, row.OptionCombinations, row.MaxGuests);
            }
        }

        private static async Task EnsureRsvpConstraint(IServiceProvider serviceProvider,
            Guid userId,
            RsvpOptions optionCombinations,
            int maxGuests)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>());
            RsvpConstraint constraint = await context.RsvpConstraints.FindAsync(userId);
            if (constraint == null)
            {
                context.Add(new RsvpConstraint
                {
                    Id = userId,
                    MaxGuests = maxGuests,
                    OptionCombinations = optionCombinations
                });
            }
            else
            {
                constraint.MaxGuests = maxGuests;
                constraint.OptionCombinations = optionCombinations;
            }
            await context.SaveChangesAsync();
        }

        private static async Task<IdentityResult> DeleteUserIfExists(UserManager<ApplicationUser> userManager,
            string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                return await userManager.DeleteAsync(user);
            }
            return await Task.FromResult(IdentityResult.Success);
        }

        private static async Task<Guid> EnsureUser(
            UserManager<ApplicationUser> userManager,
            string userName,
            string displayName,
            string role)
        {
            IdentityResult result = IdentityResult.Success;
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    DisplayName = displayName,
                    UserName = userName,
                    Email = userName,
                    EmailConfirmed = true,
                };
                result = await userManager.CreateAsync(user);
            }
            else
            {
                user.DisplayName = displayName;
                user.UserName = userName;
                user.Email = userName;
                user.EmailConfirmed = true;
                result = await userManager.UpdateAsync(user);
            }
            if (result != IdentityResult.Success)
            {
                throw new Exception(string.Join(',', result.Errors
                    .Select(x => x.Description)));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
            
            return await Task.FromResult(user.Id);
        }

        private static async Task EnsureRole(RoleManager<ApplicationRole> roleManager,
            string roleName)
        {
            IdentityResult result = IdentityResult.Success;
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                role = new ApplicationRole
                {
                    Name = roleName
                };
                result = await roleManager.CreateAsync(role);
            }
            if (result != IdentityResult.Success)
            {
                throw new Exception(string.Join(',', result.Errors
                    .Select(x => x.Description)));
            }
        }
    }
}