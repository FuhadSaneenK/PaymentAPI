using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ReferenceNo { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public int? PaymentMethodId { get; set; }
        public DateTime Date { get; set; }
    }
}
