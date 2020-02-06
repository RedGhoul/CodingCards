using System;
using System.Collections.Generic;
using System.Text;
using CodingCards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodingCards.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(c => c.Cards)
                .WithOne(e => e.CardCreator);

            modelBuilder.Entity<Card>()
                .Property(c => c.Type)
                .HasConversion<string>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Card> Cards { get; set; }
    }

}
