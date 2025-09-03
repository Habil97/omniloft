using System.ComponentModel.DataAnnotations;

namespace Verveo.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime PaidAt { get; set; }
    }
    public enum PaymentStatus
    {
        Basarili,
        Basarisiz,
        Beklemede
    }
}