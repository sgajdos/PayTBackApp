using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Microsoft.EntityFrameworkCore;
using RESTful_1.Enumeration;

namespace RESTful_1.Models
{
    [Index(nameof(PaymentId))]
    public class ATransaction
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string CardholderNumber { get; set; }
        public string HolderName { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public int CVV { get; set; }
        [MaxLength(50)]
        public string OrderReference { get; set; }
        public StatusType Status { get; set; }
        [Column(TypeName = "DateTime")]
        public DateTime CreatedDateTime { get; set; }
    }
}
