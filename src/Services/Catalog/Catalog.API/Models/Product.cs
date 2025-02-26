namespace Catalog.API.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } =default!;
        public List<string> Category { get; set; } = new List<string>();
        public string Description { get; set; } = default!;
        public string ImageFile { get; set; } = default!;
        public decimal Price { get; set; }
        //public decimal PriceTotal { get; set;} = default!;
        //public string Currency { get; set; } = default!;    
        //public string CurrencyCode { get; set; } = default!;
        //public string CurrencySymbol { get; set; } = default!;
        //public string CurrencyName { get; set; } = default!;
            
    }
}
