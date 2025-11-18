using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.DTOs
{
    public class MerchantSummaryDto
    {
        public int MerchantId { get; set; }
        public string MerchantName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalHolders { get; set; }

        public decimal TotalBalance { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalPayments { get; set; }
        public int TotalRefunds { get; set; }
    }
}
