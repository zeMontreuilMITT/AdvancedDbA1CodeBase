namespace WebApplication2.Models
{
    public class Store
    {

        public Guid StoreId { get; set; }

        public string StreetNameAndNumber { get; set; }

        public string Province { get; set; }

        public HashSet <Laptop> Laptops { get; set; } = new HashSet<Laptop>();

        private void SetProvince(string province)
        {
        // set string array to restrict provinces
        // https://www.w3schools.com/cs/cs_arrays.php
            string[] canadaProvinces = 
                { 
                "Alberta",
                "British Columbia",
                "Manitoba",
                "New Brunswick",
                "Newfoundland and Labrador",
                "Nova Scotia",
                "Ontario",
                "Prince Edward Island",
                "Quebec",
                "Saskatchewan"
            };
            //https://stackoverflow.com/questions/72696/which-is-generally-best-to-use-stringcomparison-ordinalignorecase-or-stringcom

            if (!canadaProvinces.Contains(province, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid province. Please choose a valid Canadian province.", nameof(province));
            }

            Province = province;
        }

        public Store(Guid storeId, string streetNameAndNumber, string province)
        {
            StoreId = storeId;
            StreetNameAndNumber = streetNameAndNumber;
            SetProvince(province);
        }

    }
}
