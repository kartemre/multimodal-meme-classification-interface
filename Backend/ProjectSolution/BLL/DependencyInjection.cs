using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.Services;
using DAL.Context;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBLLServices(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IUserService, UserService>();
            //yeni servisler eklenecek
            

            services.AddScoped<IUserRepository, UserRepository>();
            //yeni repositoryler eklenecek

            // DbContext (Sadece BLL DAL üzerinden çözecek)
            services.AddDbContext<MyContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
    }
}
