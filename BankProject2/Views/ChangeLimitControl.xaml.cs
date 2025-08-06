using System.Windows.Controls;
using System.Windows;
using BankProject2.Data;
using BankProject2.Models;
using System.Linq;

namespace BankProject2.Views
{
    public partial class ChangeLimitControl : UserControl
    {
        private int customerId;
        public ChangeLimitControl(int customerId)
        {
            InitializeComponent();
            this.customerId = customerId;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (!float.TryParse(AmountTextBox.Text, out float amount) || amount == 0)
            {
                MessageBox.Show("Geçerli bir miktar giriniz.");
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

                // RiskScore'u CreditCard tablosundan al
                if (card.RiskScore >= 50)
                {
                    MessageBox.Show("Risk skorunuz 50 ve üzeri olduğu için limit değişikliği yapılamaz.");
                    return;
                }

                float newLimit = card.Limit + amount;
                if (newLimit < 0)
                {
                    MessageBox.Show("Limit sıfırın altına inemez.");
                    return;
                }

                card.Limit = newLimit;
                context.SaveChanges();
                MessageBox.Show("Limit başarıyla güncellendi.");
            }
        }
    }
}