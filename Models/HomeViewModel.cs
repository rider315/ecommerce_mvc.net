using System.Collections.Generic;

namespace EcommerceApp.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Product> FeaturedProducts { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }

    public class Category
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
    }
}