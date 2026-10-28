// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using Maiko.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maiko.Fakes
{
    public class FakeMaikoContextBuilder : IDesignTimeDbContextFactory<MaikoContext>
    {
        public MaikoContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<MaikoContext> builder = new();
            builder.UseSqlite("Data Source=Maiko.db");

            return new(builder.Options);
        }
    }
}
