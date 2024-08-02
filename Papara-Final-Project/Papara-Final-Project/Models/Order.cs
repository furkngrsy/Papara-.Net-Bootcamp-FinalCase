﻿namespace Papara_Final_Project.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CouponAmount { get; set; }
        public string CouponCode { get; set; }
        public decimal PointsUsed { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
