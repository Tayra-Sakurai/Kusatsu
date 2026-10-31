// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using Maiko.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maiko.Contexts
{
    public partial class MaikoContext : DbContext
    {
        public DbSet<LargeCategory> LargeCategories { get; set; }
        public DbSet<MediumCategory> MediumCategories { get; set; }
        public DbSet<SmallCategory> SmallCategories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Method> Methods { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LargeCategory>()
                .HasBaseType<Category>();
            modelBuilder.Entity<MediumCategory>()
                .HasBaseType<Category>();
            modelBuilder.Entity<SmallCategory>()
                .HasBaseType<Category>();
            modelBuilder.Entity<Item>()
                .Property(i => i.DateTime)
                .HasDefaultValueSql("datetime('now', 'localtime')");
            modelBuilder.Entity<Method>();
            modelBuilder.Entity<Account>();
            modelBuilder.Entity<Tag>();
        }

        public MaikoContext(DbContextOptions<MaikoContext> dbContextOptions)
            : base(dbContextOptions)
        { }
    }
}
