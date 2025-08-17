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

        private Dictionary<string, double?> currencyRates = new Dictionary<string, double?>();

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
                    
                    // Sadece kullanıcının sahip olduğu dövizleri listele
                    var userOwnedCurrencies = GetUserOwnedCurrencies(context);
                    CurrencyComboBox.ItemsSource = userOwnedCurrencies;
                    if (userOwnedCurrencies.Any())
                        CurrencyComboBox.SelectedIndex = 0;
                }
                UpdateSelectedCurrencyBalance();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Döviz kurları güncellenemedi: " + ex.Message);
            }
        }

        private List<string> GetUserOwnedCurrencies(BankDbContext context)
        {
            var userCurrencyAccounts = context.accounts
                .Where(a => a.CustomerID == customerId && a.AccountType.StartsWith("Döviz-") && a.Balance > 0)
                .Select(a => a.AccountType.Substring("Döviz-".Length))
                .Distinct()
                .ToList();
            return userCurrencyAccounts;
        }

        private void AssetOrCurrencyChanged(object sender, EventArgs e)
        {
            UpdateSelectedCurrencyBalance();
        }

        private void UpdateSelectedCurrencyBalance()
        {
            if (CurrencyComboBox.SelectedItem is string selectedCurrency)
            {
                using (var context = new BankDbContext())
                {
                    var accountTypeCode = $"Döviz-{selectedCurrency}";
                    var dovizHesap = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == accountTypeCode);
                    if (dovizHesap != null)
                    {
                        // Virgül sonrası 2 hane göster
                        SelectedCurrencyBalanceText.Text = $"{dovizHesap.Balance:N2} {selectedCurrency}";
                    }
                    else
                    {
                        SelectedCurrencyBalanceText.Text = $"0,00 {selectedCurrency}";
                    }
                }
            }
        }
    }
}
