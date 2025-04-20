using System.Collections.Generic;

namespace EcommerceApp.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Product> FeaturedProducts { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Promotion> Promotions { get; set; }
    }

    public class Category
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
    }

    public class Promotion
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
    }
}