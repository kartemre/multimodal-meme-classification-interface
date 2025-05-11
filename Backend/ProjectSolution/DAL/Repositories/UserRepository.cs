using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Interfaces;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MyContext _context;

        public UserRepository(MyContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AppUser user)
        {
            await _context.AppUsers.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<AppUser> GetByUsernameAsync(string username) // Arayüzdeki metotla uyumlu
        {
            return await _context.AppUsers
                .Include(u => u.Profile) // Profile ile ilişki varsa dahil ediliyor
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<AppUser> GetByIdAsync(int id)
        {
            return await _context.AppUsers
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsAsync(string username)
        {
            return await _context.AppUsers.AnyAsync(u => u.UserName == username);
        }

        public async Task UpdateAsync(AppUser user)
        {
            _context.AppUsers.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
