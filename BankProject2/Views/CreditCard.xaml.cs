using System.Windows.Controls;
using System.Windows;
using BankProject2.Data;
using BankProject2.Models;
using System.Linq;

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
                    var limitText = this.FindName("LimitText") as TextBlock;
                    var debtText = this.FindName("DebtText") as TextBlock;
                    float kullanilabilirLimit = (float)(creditCard.Limit - creditCard.CurrentDebt);
                    if (limitText != null) limitText.Text = kullanilabilirLimit.ToString("N2") + " TL";
                    if (debtText != null) debtText.Text = $"{(creditCard.CurrentDebt ?? 0f):N2} TL";
                }
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

        private void PayDebt_Click(object sender, RoutedEventArgs e)
        {
            var payDebtControl = new BankProject2.Views.PayDebtControl(customerId);
            ShowDialogUserControl(payDebtControl, "Pay Debt");
            LoadCreditCardData();
        }

        private void ChangeLimit_Click(object sender, RoutedEventArgs e)
        {
            var changeLimitControl = new BankProject2.Views.ChangeLimitControl(customerId);
            ShowDialogUserControl(changeLimitControl, "Change Limit");
            LoadCreditCardData();
        }

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
