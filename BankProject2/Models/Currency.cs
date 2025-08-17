using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankProject2.Models
{
    public class Currency
    {
        [Key]
        public int CurrencyID { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime CurrencyDate { get; set; }
        public double? RateToTRY { get; set; }
    }
}
    