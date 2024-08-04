namespace Papara_Final_Project.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public decimal WalletBalance { get; set; }
        public int PointsBalance { get; set; }
        public string Token { get; set; }
    }
}
