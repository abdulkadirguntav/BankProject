using System.Windows.Controls;
using System;
using System.Linq;
using System.Collections.Generic;
using BankProject2.Data;
using System.Windows;

namespace BankProject2
{
    public partial class CurrencyPage : UserControl
    {
        private int customerId;
        public CurrencyPage(int customerId)
        {
            InitializeComponent();
            this.customerId = customerId;
            UpdateCurrencyRates();
            CurrencyComboBox.SelectionChanged += AssetOrCurrencyChanged;
        }

        private Dictionary<string, double> currencyRates = new Dictionary<string, double>();
        private double totalAssetTL = 0;

        private async void UpdateCurrencyRates()
        {
            try
            {
                await App.FetchAndSaveExchangeRatesAsync();
                using (var context = new BankDbContext())
                {
                    var rates = context.currency
                        .OrderByDescending(c => c.CurrencyDate)
                        .GroupBy(c => c.CurrencyCode)
                        .Select(g => g.First())
                        .ToList();
                    currencyRates = rates.ToDictionary(c => c.CurrencyCode, c => c.RateToTRY);
                    CurrencyComboBox.ItemsSource = currencyRates.Keys;
                    if (currencyRates.Keys.Any())
                        CurrencyComboBox.SelectedIndex = 0;

                    // Kullanıcının tüm hesaplarını çek
                    var accounts = context.accounts.Where(a => a.CustomerID == customerId).ToList();
                    totalAssetTL = 0;
                    foreach (var acc in accounts)
                    {
                        // Sadece vadesiz ve vadeli hesaplar
                        if (acc.AccountType == "Vadesiz" || acc.AccountType == "Vadeli")
                        {
                            totalAssetTL += acc.Balance;
                        }
                        // Döviz hesapları için ileride CurrencyID ile işlem yapılabilir
                    }
                    AssetTextBox.Text = totalAssetTL.ToString("N2");
                }
                CalculateResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Döviz kurları güncellenemedi: " + ex.Message);
            }
        }

        private void AssetOrCurrencyChanged(object sender, EventArgs e)
        {
            CalculateResult();
        }

        private void CalculateResult()
        {
            if (CurrencyComboBox.SelectedItem is string selectedCurrency && currencyRates.TryGetValue(selectedCurrency, out double rate))
            {
                double result = totalAssetTL / rate;
                ResultTextBlock.Text = result.ToString("N2", System.Globalization.CultureInfo.CurrentCulture) + $" {selectedCurrency}";
            }
            else
            {
                ResultTextBlock.Text = "0,00";
            }
        }
    }
}
