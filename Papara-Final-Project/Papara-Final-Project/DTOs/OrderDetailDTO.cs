namespace Papara_Final_Project.DTOs
{
    public class OrderDetailDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderDetailExtendedDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice {  get; set; }
    }

    public class OrderWithDetailsDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string CouponCode { get; set; }
        public decimal CouponAmount { get; set; }
        public decimal PointsUsed { get; set; }
        public DateTime OrderDate { get; set; }
    }
    
}
