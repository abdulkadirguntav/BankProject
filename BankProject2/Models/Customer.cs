using BankProject2.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankProject2.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string CustomerPassword { get; set; } = string.Empty;
        public float MonthlyIncome { get; set; } // Risk skoru için aylık gelir
    }   
}