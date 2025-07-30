using System.Windows.Controls;
using BankProject2.Data;
using System.Linq;
using BankProject2.Models;
using System.Windows;

namespace BankProject2
{
    public partial class CurrentPage : UserControl
    {
        public CurrentPage()
        {
            InitializeComponent();

            try
            {
                using (var context = new BankDbContext())
                {
                    var rates = context.currency
                        .OrderByDescending(c => c.CurrencyDate)
                        .GroupBy(c => c.CurrencyCode)
                        .Select(g => g.First())
                        .ToList();

                    //MessageBox.Show($"Kayıt sayısı: {rates.Count}");

                    dataGridRates.ItemsSource = rates;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}
