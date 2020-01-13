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
            
            modelBuilder.Entity<ApplicationUserCard>()
                .HasKey(t => new { t.ApplicationUserId, t.CardId });

            modelBuilder.Entity<ApplicationUserCard>()
                .HasOne(pt => pt.ApplicationUser)
                .WithMany(p => p.ApplicationUserCards)
                .HasForeignKey(pt => pt.ApplicationUserId);

            modelBuilder.Entity<ApplicationUserCard>()
                .HasOne(pt => pt.Card)
                .WithMany(t => t.ApplicationUserCards)
                .HasForeignKey(pt => pt.CardId);

            modelBuilder.Entity<Card>()
                .Property(c => c.Type)
                .HasConversion<string>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<ApplicationUserCard> ApplicationUserCards { get; set; }
    }

}
