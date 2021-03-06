﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Database.Entities;

namespace WebApi.Database
{
    public class DBEntities : IdentityDbContext<User>
    {

        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoicePayment> InvoicePayment { get; set; }
        public virtual DbSet<Email> Emails { get; set; }

        public DBEntities(DbContextOptions<DBEntities> options) : base(options)
        {

        }
        /*public DBEntities() : this(new DbContextOptions<DBEntities>())
        {

        }*/

        public DBEntities() : this(GetOptions())
        {
        }

        private static DbContextOptions<DBEntities> GetOptions()
        {
            return (MySqlDbContextOptionsExtensions.UseMySql(new DbContextOptionsBuilder<DBEntities>(), Startup.ConnectionString).Options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseMySql(Startup.ConnectionString);
        }

    }
}
