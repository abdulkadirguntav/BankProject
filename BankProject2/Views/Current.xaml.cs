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
            LoadRates();
        }

        private async void LoadRates()
        {
            try
            {
                // Önce güncel verileri çek
                await App.FetchAndSaveExchangeRatesAsync();

                // Sonra veritabanından oku
                using (var context = new BankDbContext())
                {
                    var rates = context.currency
                        .GroupBy(c => c.CurrencyCode)
                        .Select(g => g.OrderByDescending(x => x.CurrencyDate).FirstOrDefault())
                        .ToList();

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
