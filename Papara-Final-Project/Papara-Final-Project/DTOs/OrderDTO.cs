namespace Papara_Final_Project.DTOs
{
    public class OrderDTO
    {
        public string CouponCode { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}
