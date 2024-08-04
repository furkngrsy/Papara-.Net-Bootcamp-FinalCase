namespace Papara_Final_Project.DTOs
{
    public class CouponDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
