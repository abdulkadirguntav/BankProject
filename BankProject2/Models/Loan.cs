using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankProject2.Models
{
    public class Loan
    {
        [Key]
        public int LoanID { get; set; }
        public float InterestRate { get; set; }
        public float Principal { get; set; }
        public int TermMonths { get; set; }
        public string LoanStatus { get; set; }
    }
}
