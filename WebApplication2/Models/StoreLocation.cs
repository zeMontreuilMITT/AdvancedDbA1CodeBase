using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class StoreLocation
    {
        [Key]
        public Guid StoreNumber { get; set; }

        public string StreetNameAndNumber { get; set; }

        public Province Province { get; set; }

        public ICollection<LaptopStoreLocation> LaptopStoreLocations { get; set; }
    }
    public enum Province
    {
        AB, // Alberta
        BC, // British Columbia
        MB, // Manitoba
        NB, // New Brunswick
        NL, // Newfoundland and Labrador
        NS, // Nova Scotia
        ON, // Ontario
        PE, // Prince Edward Island
        QC, // Quebec
        SK, // Saskatchewan
        NT, // Northwest Territories
        NU, // Nunavut
        YT  // Yukon
    }


}
