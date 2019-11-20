using BikeRental.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental
{
    public class BikeRentalContext : DbContext
    {
        public BikeRentalContext(DbContextOptions<BikeRentalContext> options) : base(options)
        { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Bike> Bikes { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public void ConfigureServices(IServiceCollection services)
        {
      //      services.AddDbContext<BikeRentalContext>(options => options.UseSqlServer(
      //          Configuration["ConnectionStrings:DefaultConnection"]));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            modelBuilder.Entity<Bike>()
                .Property(b => b.PriceFirstHour)
                .HasColumnType("decimal");

            modelBuilder.Entity<Bike>()
                .Property(b => b.PriceAdditionalHours)
                .HasColumnType("decimal");

            modelBuilder.Entity<Rental>()
                .Property(r => r.TotalCost)
                .HasColumnType("decimal");

        }
    }
}
