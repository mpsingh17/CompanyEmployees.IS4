using CompanyEmployees.IDP.Entities.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.IDP.Entities
{
    public class UserContext : IdentityDbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}
