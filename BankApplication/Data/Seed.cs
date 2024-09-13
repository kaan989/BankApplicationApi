using BankApplication.Data.Enum;
using BankApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BankApplication.Data
{
    public class Seed
    {
        private readonly ApplicationDbContext _context;
        public Seed(ApplicationDbContext context)
        {
            _context = context;
        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Rolleri oluştur
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                if (!await roleManager.RoleExistsAsync(UserRoles.Customer))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Customer));
                if (!await roleManager.RoleExistsAsync(UserRoles.Employee))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Customer));

                //// Admin kullanıcıyı oluştur ve rolünü ata
                //string adminUserEmail = "kaanpeh@gmail.com";
                //var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                //if (adminUser == null)
                //{
                //    var newAdminUser = new AppUser()
                //    {
                //        UserName = "Kaanpeh",
                //        Email = adminUserEmail,
                //        EmailConfirmed = true,
                //    };
                //    await userManager.CreateAsync(newAdminUser, "Kaan_2872");
                //    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);

                //    // Admin için bir hesap oluştur
                //    var adminAccount = new Account
                //    {
                //        AccountNumber = GenerateAccountNumber(),
                //        Balance = 1000, // Admin başlangıç bakiyesi
                //        Type = AccountType.vadesiz, // Vadesiz hesap
                //        CreatedAt = DateTime.Now,
                //        AppUserId = newAdminUser.Id
                //    };
                //    context.Accounts.Add(adminAccount);
                //}

                //// Normal kullanıcıyı oluştur ve rolünü ata
                //string appUserEmail = "jake@gmail.com";
                //var appUser = await userManager.FindByEmailAsync(appUserEmail);
                //if (appUser == null)
                //{
                //    var newAppUser = new AppUser()
                //    {
                //        UserName = "app-user",
                //        Email = appUserEmail,
                //        EmailConfirmed = true,
                //    };
                //    await userManager.CreateAsync(newAppUser, "Jake_2872");
                //    await userManager.AddToRoleAsync(newAppUser, UserRoles.User);

                //    // Kullanıcı için bir hesap oluştur
                //    var userAccount = new Account
                //    {
                //        AccountNumber = GenerateAccountNumber(),
                //        Balance = 500, // Kullanıcı başlangıç bakiyesi
                //        Type = AccountType.vadeli, // Vadeli hesap
                //        CreatedAt = DateTime.Now,
                //        LastInterestAppliedDate = DateTime.Now,
                //        AppUserId = newAppUser.Id
                //    };
                //    context.Accounts.Add(userAccount);
                //}

                // Veritabanı değişikliklerini kaydet
                await context.SaveChangesAsync();
            }
        }

        // Hesap numarası oluşturma fonksiyonu
        private static string GenerateAccountNumber()
        {
            return $"ACC-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }
}
