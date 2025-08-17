using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BankProject2.Models
{
    internal class Accounts
    {
        [Key]
        public int AccountID { get; set; }
        public string AccountType { get; set; }
        public float? Balance { get; set; }
        public string IBAN { get; set; } // Hesap IBAN'ı
        public int? CurrencyID { get; set; } // Döviz hesabı için
        public DateTime? StartDate { get; set; } // Vadeli hesap için başlangıç tarihi
        public DateTime? MaturityDate { get; set; } // Vadeli hesap için vade bitişi
        public float? InterestRate { get; set; } // Vadeli hesap için faiz oranı
        public bool? IsBroken { get; set; }      // Vadeli hesap bozuldu mu
        public float? PrincipalAmount { get; set; } // Vadeli hesap ana para
        public float? AccruedInterest { get; set; } // Vadeli hesap biriken faiz
        public int CustomerID { get; set; } // Hesabın sahibi müşteri
    }
}
