using System.ComponentModel.DataAnnotations;

namespace RESTful_1.Dto
{
    public class ATransactionResponseDto
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string CardholderNumber { get; set; }
        public string HolderName { get; set; }
        public string OrderReference { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
