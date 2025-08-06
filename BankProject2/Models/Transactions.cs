using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankProject2.Models;

namespace BankProject2.Models
{
    public class Transactions
    {
        [Key]
        public int TransactionID { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public int? FromAccountID { get; set; }
        public int? ToAccountID { get; set; }
        public int? CreditCardID { get; set; }
        public string Description { get; set; }
        public double Fee { get; set; }
        public float Amount { get; set; }
    }
}
