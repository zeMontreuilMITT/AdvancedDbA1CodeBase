
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System;

namespace WebApplication2.Data
{
    public class SlnDbContext : DbContext
    {
        
        public SlnDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Brand> Brand { get; set; } = null!;

        public DbSet<Laptop> Laptop { get; set; } = null!;

    }
}