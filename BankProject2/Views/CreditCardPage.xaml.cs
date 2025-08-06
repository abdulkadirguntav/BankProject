using System.Windows.Controls;
using System.Windows;
using BankProject2.Data;
using BankProject2.Models;
using System.Linq;

namespace BankProject2.Views
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
                    float kullanilabilirLimit = creditCard.Limit - creditCard.CurrentDebt;
                    LimitText.Text = kullanilabilirLimit.ToString("N2") + " TL";
                    DebtText.Text = creditCard.CurrentDebt.ToString("N2") + " TL";

                    var transactions = context.transactions
                        .Where(t => t.CreditCardID == creditCard.CreditCardID)
                        .OrderByDescending(t => t.TransactionDate)
                        .ToList();
                    CreditCardDataGrid.ItemsSource = transactions;
                }
                else
                {
                    LimitText.Text = "-";
                    DebtText.Text = "-";
                    CreditCardDataGrid.ItemsSource = null;
                }
            }
        }

        private void PayDebt_Click(object sender, RoutedEventArgs e)
        {
            var payDebtControl = new PayDebtControl(customerId);
            ShowDialogUserControl(payDebtControl, "Borç Öde");
            LoadCreditCardData();
        }

        private void ChangeLimit_Click(object sender, RoutedEventArgs e)
        {
            var changeLimitControl = new ChangeLimitControl(customerId);
            ShowDialogUserControl(changeLimitControl, "Limit Arttýr/Azalt");
            LoadCreditCardData();
        }

        // UserControl'ü modal pencere olarak göstermek için yardýmcý fonksiyon
        private void ShowDialogUserControl(UserControl control, string title)
        {
            var window = new Window
            {
                Title = title,
                Content = control,
                Width = control.Width > 0 ? control.Width + 40 : 400,
                Height = control.Height > 0 ? control.Height + 40 : 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this),
                ResizeMode = ResizeMode.NoResize
            };
            window.ShowDialog();
        }
    }
}