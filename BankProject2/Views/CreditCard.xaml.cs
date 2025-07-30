using System.Windows.Controls;
using BankProject2.Data;
using BankProject2.Models;
using System.Linq;
using System.Windows;

namespace BankProject2
{
    public partial class CreditCardPage : UserControl
    {
        private int customerId;
        public CreditCardPage(int customerId)
        {
            InitializeComponent();
            this.customerId = customerId;
            LoadCreditCardData();
        }

        private void LoadCreditCardData()
        {
            using (var context = new BankDbContext())
            {
                var creditCard = context.creditCard.FirstOrDefault(c => c.CustomerID == customerId);
                if (creditCard != null)
                {
                    // Kullanılabilir limit ve borç hesapla
                    var limitText = this.FindName("LimitText") as TextBlock;
                    var debtText = this.FindName("DebtText") as TextBlock;
                    float kullanilabilirLimit = creditCard.Limit - creditCard.CurrentDebt;
                    if (limitText != null) limitText.Text = kullanilabilirLimit.ToString("N2") + " TL";
                    if (debtText != null) debtText.Text = creditCard.CurrentDebt.ToString("N2") + " TL";
                }
                // İşlem geçmişi: Sadece bu kredi kartına ait işlemler
                if (creditCard != null)
                {
                    var transactions = context.transactions
                        .Where(t => t.CreditCardID == creditCard.CreditCardID)
                        .OrderByDescending(t => t.TransactionDate)
                        .ToList();
                    var dataGrid = this.FindName("CreditCardDataGrid") as DataGrid;
                    if (dataGrid != null) dataGrid.ItemsSource = transactions;
                }
            }
        }
    }
}
