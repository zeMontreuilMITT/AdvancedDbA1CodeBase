using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace WebApplication2.Models
{
    public class Store
    {
        [Key]
        public Guid StoreNumber { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public CanadianProvince Province { get; set; }
        public ICollection<Laptop> LaptopsInStock { get; set; } = new List<Laptop>();
    }

    public enum CanadianProvince
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
