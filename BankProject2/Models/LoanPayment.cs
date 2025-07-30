using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankProject2.Models
{
    public class LoanPayment
    {
        [Key]
        public int LoanPaymentID { get; set; }
        public float Amount { get; set; }
        public DateTime PaidDate { get; set; }
    }
}
