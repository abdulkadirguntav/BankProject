using System;
using System.ComponentModel.DataAnnotations;

namespace BankProject2.Models
{
    public class CreditCard
    {
        [Key]
        public int CreditCardID { get; set; }
        public int CustomerID { get; set; }
        public string? CardNumber { get; set; }
        public DateTime? CardExpiry { get; set; }
        public string? CardCVV { get; set; }
        public float? Limit { get; set; }
        public float? CurrentDebt { get; set; }
        public int? RiskScore { get; set; }
        public int? LatePaymentCount { get; set; } // Gecikmiş ödeme sayısı
    }
} 