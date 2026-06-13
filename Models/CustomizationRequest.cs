using System.ComponentModel.DataAnnotations;

namespace ReaB_Bakery.Models
{
    public class CustomizationRequest
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime EventDate { get; set; }
        public string DesignNotes { get; set; }
        public string InspirationPhotoPath { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
    }
}