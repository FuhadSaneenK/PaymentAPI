using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Tests.Mocks.Entity_Mock
{
    public static class TransactionMock
    {
        public static Transaction GetTransaction(
            int id,
            decimal amount,
            string type,
            int accountId,
            int? paymentMethodId = null,
            string reference = "REF123")
        {
            return new Transaction
            {
                Id = id,
                AccountId = accountId,
                Amount = amount,
                Type = type,
                Status = "Completed",
                ReferenceNumber = reference,
                PaymentMethodId = paymentMethodId ?? 1,
                Date = DateTime.UtcNow
            };
        }

        public static Transaction GetPayment(
            int id = 1,
            int accountId = 1,
            decimal amount = 500,
            string reference = "REF123")
        {
            return new Transaction
            {
                Id = id,
                AccountId = accountId,
                Amount = amount,
                Type = "Payment",
                Status = "Completed",
                ReferenceNumber = reference,
                Date = DateTime.UtcNow
            };
        }

        public static Transaction GetRefund(
            int id = 2,
            int accountId = 1,
            decimal amount = 200,
            string reference = "REF123-REF")
        {
            return new Transaction
            {
                Id = id,
                AccountId = accountId,
                Amount = amount,
                Type = "Refund",
                Status = "Completed",
                ReferenceNumber = reference,
                Date = DateTime.UtcNow
            };
        }
    }
}
