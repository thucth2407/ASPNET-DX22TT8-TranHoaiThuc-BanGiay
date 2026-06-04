namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models
{
    public class CartModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? Size { get; set; }   // Thêm size
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalOrder { get; set; }
        public decimal Total => Quantity * Price;
    }
}