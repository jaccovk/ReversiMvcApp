using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ReversiMvcApp.Models;

namespace ReversiMvcApp.Data
{
    public class ReversiDbContext : IdentityDbContext
    {
        public ReversiDbContext(DbContextOptions<ReversiDbContext> options)
            : base(options)
        {
        }

        public DbSet<Speler> Spelers { get; set; }

        public DbSet<Spel> Spel { get; set; }
    }
}
