using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string HolderName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public int MerchantId { get; set; }
    }
}
