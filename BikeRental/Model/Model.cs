using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.Model
{
    public enum Gender { Male, Female, Unknown };
    public enum BikeCategory { Standard_bike, Mountainbike, Trecking_bike, Racing_bike };

    public class Customer
    {
        public int CustomerId { get; set; }
        public Gender Gender { get; set; }
        [Required, MaxLength(50)]
        public string FirstName { get; set; }
        [Required, MaxLength(75)]
        public string LastName { get; set; }
        [Required]
        public DateTime Birthday { get; set; }
        [Required, MaxLength(75)]
        public string Street { get; set; }
        [MaxLength(10)]
        public int HouseNumber { get; set; }
        [Required, MaxLength(10)]
        public string ZipCode { get; set; }
        [Required, MaxLength(75)]
        public string Town { get; set; }
        public List<Rental> Rentals { get; set; }
    }

    public class Bike
    {
        public int BikeId { get; set; }
        [Required]
        [MaxLength(25)]
        public string Brand { get; set; }
        [Required]
        public DateTime PurchesDate { get; set; }
        [MaxLength(1000)]
        public string Notes { get; set; }
        public DateTime LastService { get; set; }
        [Required, RegularExpression("\\d*\\.\\d{1,2}$")]
        //Value >=0 Range???
        public decimal PriceFirstHour { get; set; }
        [Required, RegularExpression("\\d*\\.\\d{1,2}$")]
        public decimal PriceAdditionalHours { get; set; }
        [Required]
        public BikeCategory BikeCategory { get; set; }
    }

    public class Rental
    {
        public int RentalId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        [Required]
        public int BikeId { get; set; }
        public Bike Bike { get; set; }
        [Required]
        public DateTime RentalBegin { get; set; }
        public DateTime RentalEnd { get; set; }
        [Required, RegularExpression("\\d*\\.\\d{1,2}$"), Range(0, Double.PositiveInfinity)]
        public decimal TotalCost { get; set; }
        public bool Paid { get; set; }

    }
}
