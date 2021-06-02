using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtTest.Models;
using Isopoh.Cryptography.Argon2;

namespace JwtTest.EF
{
    public class JwtContext:DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Gift> Gift { get; set; }
        public DbSet<Give_gift> Give_gift { get; set; }

        public JwtContext(DbContextOptions<JwtContext> options)
            : base(options)
        {
            Database.EnsureCreated(); 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Person>().HasData(new Person() { Id = 1, Login = "admin", PasswordHash = Argon2.Hash("admin"), Role = UserRole.Admin });
        }
    }
}
