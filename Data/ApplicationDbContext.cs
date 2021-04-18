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
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(c => c.Cards)
                .WithOne(e => e.CardCreator)
                .HasForeignKey(e => e.CardCreatorId);

            modelBuilder.Entity<Card>()
                .Property(c => c.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Card>().HasIndex(x => x.LangName);
            modelBuilder.Entity<Card>().HasIndex(x => x.Type);
            modelBuilder.Entity<Card>().HasIndex(x => x.NumberOfViewAnswers);
            modelBuilder.Entity<Card>().HasIndex(x => x.NumberOfViews);
            modelBuilder.Entity<Card>().HasIndex(x => x.DateCreated);
            modelBuilder.Entity<Card>().HasIndex(x => x.DateDeleted);
            modelBuilder.Entity<Card>().HasIndex(x => x.DateModified);

        }

    }

}
