namespace ReaB_Bakery.Models
{
    public class CustomOrder
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CakeFlavour { get; set; }
        public string? OtherFlavour { get; set; }
        public string CakeColour { get; set; }
        public string? CustomColour { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? DesignNotes { get; set; }
        public string? InspirationPhotoPath { get; set; }
        public DateTime OrderedAt { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
        public string? OrderReference { get; set; }
    }
}