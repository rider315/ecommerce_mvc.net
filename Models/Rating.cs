namespace EcommerceApp.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Score { get; set; } // 1 to 5
        public Product Product { get; set; }
    }
}