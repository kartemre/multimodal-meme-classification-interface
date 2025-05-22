using DAL.Context;
using Entities.Enums;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(MyContext context)
        {
            // Veritabanını oluştur
            await context.Database.MigrateAsync();

            // Admin kullanıcısı var mı kontrol et
            if (!await context.AppUsers.AnyAsync(u => u.Profile.Role == UserRoles.Admin))
            {
                // Admin kullanıcısı oluştur
                var adminUser = new AppUser
                {
                    UserName = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    CreatedTime = DateTime.UtcNow,
                    IsActive = true,
                    Profile = new AppUserProfile
                    {
                        FirstName = "Admin",
                        LastName = "Kart",
                        Mail = "kartemre3403@gmail.com",
                        PhoneNumber = "5066405389",
                        Role = UserRoles.Admin,
                        CreatedTime = DateTime.UtcNow
                    }
                };

                await context.AppUsers.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
} 