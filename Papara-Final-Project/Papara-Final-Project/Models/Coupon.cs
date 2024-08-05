namespace Papara_Final_Project.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool IsUsed { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
