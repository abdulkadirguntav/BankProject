using BankProject2.Data;
using BankProject2.Models;
using System.Windows;
using System.Windows.Controls;

namespace BankProject2.Views
{
    public partial class PayDebtControl : UserControl
    {
        private int customerId;
        public PayDebtControl(int customerId)
        {
            InitializeComponent();
            this.customerId = customerId;
            LoadDebt();
        }

        private void LoadDebt()
        {
            using (var context = new BankDbContext())
            {
                var card = context.creditCard.FirstOrDefault(c => c.CustomerID == customerId);
                if (card != null)
                    DebtText.Text = $"{(card.CurrentDebt ?? 0f):N2} TL";
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (!float.TryParse(AmountTextBox.Text, out float payAmount) || payAmount <= 0)
            {
                MessageBox.Show("Geçerli bir ödeme miktarı giriniz.");
                return;
            }

            using (var context = new BankDbContext())
            {
                var card = context.creditCard.FirstOrDefault(c => c.CustomerID == customerId);
                if (card == null)
                {
                    MessageBox.Show("Kredi kartı bulunamadı.");
                    return;
                }
                if (payAmount > card.CurrentDebt)
                {
                    MessageBox.Show("Ödeme miktarı borçtan fazla olamaz.");
                    return;
                }

                var vadesiz = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadesiz");
                var vadeli = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadeli");

                float vadesizBakiye = vadesiz?.Balance ?? 0;
                float vadeliBakiye = vadeli?.Balance ?? 0;

                if (vadesizBakiye >= payAmount)
                {
                    vadesiz.Balance -= payAmount;
                }
                else if (vadeliBakiye >= payAmount)
                {
                    var result = MessageBox.Show("Vadesiz hesabınızda yeterli bakiye yok. Vadeli hesabınızdan ödeme yapmak ister misiniz? Faiziniz yanar.", "Vadeli Hesap Uyarı", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result != MessageBoxResult.Yes)
                        return;
                    vadeli.Balance -= payAmount;
                    vadeli.IsBroken = true;
                }
                else
                {
                    MessageBox.Show("Yeterli bakiyeniz yok.");
                    return;
                }

                card.CurrentDebt -= payAmount;
                context.transactions.Add(new Transactions
                {
                    TransactionType = "Kredi Kartı Borç Ödeme",
                    TransactionDate = System.DateTime.Now,
                    Amount = payAmount,
                    CreditCardID = card.CreditCardID,
                    Description = "Kredi kartı borç ödemesi"
                });
                context.SaveChanges();
                MessageBox.Show("Borç ödeme işlemi başarılı.");
            }
        }
    }
}
