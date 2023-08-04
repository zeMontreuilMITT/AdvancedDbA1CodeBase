using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebApplication2.Models
{
    public class StoreLocation
    {
        public Guid StoreNumber { get; set; }

        private string _streetNameAndNumber;

        public string StreetNameAndNumber
        {
            get => _streetNameAndNumber;
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length < 3)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), 
                        "Street name must be at least three characters in " +
                        "length.");
                }

                else
                {
                    _streetNameAndNumber = value;
                }
            }
        }

        public Province Province { get; set; }

        public HashSet<Laptop_Store> LaptopStores { get; set; } = new HashSet<Laptop_Store>();
    }

    public enum Province
    {
        Alberta,
        BritishColumbia,
        Manitoba,
        NewBrunswick,
        NewfoundlandAndLabrador,
        NorthwestTerritories,
        NovaScotia,
        Nunavut,
        Ontario,
        PrinceEdwardIsland,
        Quebec,
        Saskatchewan,
        Yukon
    }
}
