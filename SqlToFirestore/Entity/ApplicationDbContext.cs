using SqlToFirestore.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlToFirestore.Entity
{
   public class ApplicationDbContext : DbContext
    {
        public DbSet<Companies> Companies { get; set; }
        public DbSet<BusinessOpportunities> BusinessOpportunities { get; set; }
        public DbSet<ContactPersons> ContactPersons { get; set; }
        public DbSet<Activities> Activities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder
            {
                DataSource = ".",
                InitialCatalog = "Geshdo",
                IntegratedSecurity = true
            };
            optionsBuilder.UseSqlServer(connectionString.ToString());
        }
    }
}
