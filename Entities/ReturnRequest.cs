using System;

namespace Verveo.Entities
{
    public class ReturnRequest
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; } // "İade" veya "Değişim"
        public string Reason { get; set; }
        public ReturnStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public enum ReturnStatus
    {
        Beklemede,
        Onaylandi,
        Reddedildi
    }
}