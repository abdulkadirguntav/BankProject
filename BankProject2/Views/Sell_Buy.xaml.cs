using BankProject2.Data;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Windows;

namespace BankProject2
{
    public partial class Sell_BuyPage : UserControl
    {
        private readonly int _customerId;

        // Kurları double (nullable değil) olarak tutuyoruz
        private Dictionary<string, double> currencyRates = new Dictionary<string, double>();

        private bool _isBuyAmountChanging = false;
        private bool _isBuyTotalChanging = false;
        private bool _isSellAmountChanging = false;
        private bool _isSellTotalChanging = false;

        private const double CommissionRate = 0.001;    // %0.1 komisyonl

        public Sell_BuyPage(int customerId)
        {
            InitializeComponent();
            _customerId = customerId;

            LoadCurrencyRates();

            BuyAmountTextBox.TextChanged += BuyAmountTextBox_TextChanged;
            BuyTotalTextBox.TextChanged += BuyTotalTextBox_TextChanged;
            BuyAmountTextBox.PreviewTextInput += AmountTextBox_PreviewTextInput;
            BuyTotalTextBox.PreviewTextInput += AmountTextBox_PreviewTextInput;

            SellAmountTextBox.TextChanged += SellAmountTextBox_TextChanged;
            SellTotalTextBox.TextChanged += SellTotalTextBox_TextChanged;
            SellAmountTextBox.PreviewTextInput += AmountTextBox_PreviewTextInput;
            SellTotalTextBox.PreviewTextInput += AmountTextBox_PreviewTextInput;
        }

        private void LoadCurrencyRates()
        {
            using (var context = new BankDbContext())
            {
                var rates = context.currency
                    .OrderByDescending(c => c.CurrencyDate)
                    .GroupBy(c => c.CurrencyCode)
                    .Select(g => g.First())
                    .ToList();

                // RateToTRY double? ise null’ları 0’a düşür
                currencyRates = rates.ToDictionary(c => c.CurrencyCode, c => (c.RateToTRY ?? 0d));

                // Alım combobox
                BuyCurrencyComboBox.ItemsSource = currencyRates.Keys.ToList();
                BuyCurrencyComboBox.SelectedIndex = BuyCurrencyComboBox.Items.Count > 0 ? 0 : -1;
                BuyCurrencyComboBox.SelectionChanged += (s, e) =>
                {
                    UpdateCurrencyRateText(BuyCurrencyComboBox, BuyCurrencyRateText);
                    UpdateSelectedBalances();
                    BuyAmountTextBox.Text = BuyTotalTextBox.Text = string.Empty;
                };

                // Satış combobox (kullanıcının sahip oldukları)
                SellCurrencyComboBox.ItemsSource = GetUserOwnedCurrencies(context);
                SellCurrencyComboBox.SelectedIndex = SellCurrencyComboBox.Items.Count > 0 ? 0 : -1;
                SellCurrencyComboBox.SelectionChanged += (s, e) =>
                {
                    UpdateCurrencyRateText(SellCurrencyComboBox, SellCurrencyRateText);
                    UpdateSelectedBalances();
                    SellAmountTextBox.Text = SellTotalTextBox.Text = string.Empty;
                };

                UpdateCurrencyRateText(BuyCurrencyComboBox, BuyCurrencyRateText);
                UpdateCurrencyRateText(SellCurrencyComboBox, SellCurrencyRateText);
                UpdateSelectedBalances();
            }
        }

        private System.Collections.IEnumerable GetUserOwnedCurrencies(BankDbContext context)
        {
            var codes = context.accounts
                .Where(a => a.CustomerID == _customerId && a.AccountType.StartsWith("Döviz-") && a.Balance > 0)
                .Select(a => a.AccountType.Substring("Döviz-".Length))
                .Distinct()
                .ToList();
            return codes;
        }

        // Al: Döviz miktarı değişti → TL hesapla
        private void BuyAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isBuyTotalChanging) return;
            _isBuyAmountChanging = true;

            if (BuyCurrencyComboBox.SelectedItem is string code && TryGetRate(code, out var rate))
            {
                if (double.TryParse(BuyAmountTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double amount))
                {
                    double total = amount * rate;
                    double fee = total * CommissionRate;

                    BuyTotalTextBox.Text = total.ToString("N2", CultureInfo.CurrentCulture);
                    BuyFeeText.Text = $"Komisyon: {fee.ToString("N2", CultureInfo.CurrentCulture)} TL";
                }
                else
                {
                    BuyTotalTextBox.Text = string.Empty;
                    BuyFeeText.Text = "Komisyon: 0,00 TL";
                }
            }

            _isBuyAmountChanging = false;
        }

        // Al: TL değişti → döviz hesapla
        private void BuyTotalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isBuyAmountChanging) return;
            _isBuyTotalChanging = true;

            if (BuyCurrencyComboBox.SelectedItem is string code && TryGetRate(code, out var rate))
            {
                if (double.TryParse(BuyTotalTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double total))
                {
                    double amount = rate == 0 ? 0 : total / rate;
                    double fee = total * CommissionRate;

                    BuyAmountTextBox.Text = amount.ToString("N4", CultureInfo.CurrentCulture);
                    BuyFeeText.Text = $"Komisyon: {fee.ToString("N2", CultureInfo.CurrentCulture)} TL";
                }
                else
                {
                    BuyAmountTextBox.Text = string.Empty;
                    BuyFeeText.Text = "Komisyon: 0,00 TL";
                }
            }

            _isBuyTotalChanging = false;
        }

        // Sat: Döviz miktarı değişti → TL hesapla
        private void SellAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSellTotalChanging) return;
            _isSellAmountChanging = true;

            if (SellCurrencyComboBox.SelectedItem is string code && TryGetRate(code, out var rate))
            {
                if (double.TryParse(SellAmountTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double amount))
                {
                    double total = amount * rate;
                    double fee = total * CommissionRate;

                    SellTotalTextBox.Text = total.ToString("N2", CultureInfo.CurrentCulture);
                    SellFeeText.Text = $"Komisyon: {fee.ToString("N2", CultureInfo.CurrentCulture)} TL";
                }
                else
                {
                    SellTotalTextBox.Text = string.Empty;
                    SellFeeText.Text = "Komisyon: 0,00 TL";
                }
            }

            _isSellAmountChanging = false;
        }

        // Sat: TL değişti → döviz hesapla
        private void SellTotalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSellAmountChanging) return;
            _isSellTotalChanging = true;

            if (SellCurrencyComboBox.SelectedItem is string code && TryGetRate(code, out var rate))
            {
                if (double.TryParse(SellTotalTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double total))
                {
                    double amount = rate == 0 ? 0 : total / rate;
                    double fee = total * CommissionRate;

                    SellAmountTextBox.Text = amount.ToString("N4", CultureInfo.CurrentCulture);
                    SellFeeText.Text = $"Komisyon: {fee.ToString("N2", CultureInfo.CurrentCulture)} TL";
                }
                else
                {
                    SellAmountTextBox.Text = string.Empty;
                    SellFeeText.Text = "Komisyon: 0,00 TL";
                }
            }

            _isSellTotalChanging = false;
        }

        private void UpdateCurrencyRateText(ComboBox combo, TextBlock rateText)
        {
            if (combo?.SelectedItem is string code && TryGetRate(code, out var rate))
            {
                rateText.Text = $"1 {code} = {rate.ToString("N2", CultureInfo.CurrentCulture)} TL";
            }
            else
            {
                rateText.Text = "-";
            }
        }

        private bool TryGetRate(string code, out double rate)
        {
            if (currencyRates.TryGetValue(code, out var r))
            {
                rate = r;
                return true;
            }
            rate = 0;
            return false;
        }

        private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Sadece sayı ve virgül/nokta
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c) && c != ',' && c != '.')
                    return false;
            }
            return true;
        }

        private void BuyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (BuyCurrencyComboBox.SelectedItem is not string code || !TryGetRate(code, out var rate)) return;
            if (!double.TryParse(BuyAmountTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double amount) || amount <= 0) return;
            //if (amount < MinTradeAmountBase) { MessageBox.Show($"Minimum işlem miktarı {MinTradeAmountBase} {code}"); return; }

            double tlCost = amount * rate;
            double fee = tlCost * CommissionRate;
            BuyFeeText.Text = $"Komisyon: {fee.ToString("N2", CultureInfo.CurrentCulture)} TL";

            using (var context = new BankDbContext())
            {
                var vadesiz = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == "Vadesiz");
                if (vadesiz == null) { MessageBox.Show("Vadesiz hesabınız bulunamadı."); return; }

                double totalDebit = tlCost + fee;
                if (vadesiz.Balance < (float)totalDebit) { MessageBox.Show("Yetersiz bakiye (TL)"); return; }

                var accountTypeCode = $"Döviz-{code}";
                var doviz = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == accountTypeCode);
                if (doviz == null)
                {
                    doviz = new Models.Accounts { CustomerID = _customerId, AccountType = accountTypeCode, Balance = 0, IBAN = GenerateSimpleIban() };
                    context.accounts.Add(doviz);
                    context.SaveChanges();
                }

                // Bakiye güncelle
                vadesiz.Balance -= (float)totalDebit;
                doviz.Balance += (float)amount;

                // İşlem kaydı
                context.transactions.Add(new Models.Transactions
                {
                    TransactionType = "FX-Buy",
                    TransactionDate = DateTime.Now,
                    Amount = (float)tlCost,
                    FromAccountID = vadesiz.AccountID,
                    ToAccountID = doviz.AccountID,
                    Fee = fee,
                    Description = $"{amount.ToString("N4", CultureInfo.CurrentCulture)} {code} alım"
                });

                context.SaveChanges();
                MessageBox.Show("Alım işlemi başarılı");
                UpdateSelectedBalances();
            }
        }

        private void SellButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SellCurrencyComboBox.SelectedItem is not string code || !TryGetRate(code, out var rate)) return;
            if (!double.TryParse(SellAmountTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double amount) || amount <= 0) return;
            //if (amount < MinTradeAmountBase) { MessageBox.Show($"Minimum işlem miktarı {MinTradeAmountBase} {code}"); return; }

            double tlProceeds = amount * rate;
            double fee = tlProceeds * CommissionRate;
            SellFeeText.Text = $"Komisyon: {fee.ToString("N2", CultureInfo.CurrentCulture)} TL";

            using (var context = new BankDbContext())
            {
                var vadesiz = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == "Vadesiz");
                if (vadesiz == null) { MessageBox.Show("Vadesiz hesabınız bulunamadı."); return; }

                var accountTypeCode = $"Döviz-{code}";
                var doviz = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == accountTypeCode);
                if (doviz == null || doviz.AccountID == 0) { MessageBox.Show("Döviz hesabınız bulunamadı."); return; }
                if (doviz.Balance < (float)amount) { MessageBox.Show("Yetersiz döviz bakiyesi"); return; }

                // Bakiye güncelle
                doviz.Balance -= (float)amount;
                vadesiz.Balance += (float)(tlProceeds - fee);

                // İşlem kaydı
                context.transactions.Add(new Models.Transactions
                {
                    TransactionType = "FX-Sell",
                    TransactionDate = DateTime.Now,
                    Amount = (float)tlProceeds,
                    FromAccountID = doviz.AccountID,
                    ToAccountID = vadesiz.AccountID,
                    Fee = fee,
                    Description = $"{amount.ToString("N4", CultureInfo.CurrentCulture)} {code} satış"
                });

                context.SaveChanges();
                MessageBox.Show("Satış işlemi başarılı");
                UpdateSelectedBalances();
            }
        }

        private void UpdateSelectedBalances()
        {
            using (var context = new BankDbContext())
            {
                // Buy sekmesi: vadesiz TL bakiyesi
                var vadesiz = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == "Vadesiz");
                var vadesizBal = vadesiz?.Balance ?? 0f;
                BuySelectedBalanceText.Text = $"Vadesiz: {vadesizBal.ToString("N2", CultureInfo.CurrentCulture)} TL";

                // Sell sekmesi: seçilen döviz bakiyesi
                if (SellCurrencyComboBox.SelectedItem is string sellCode)
                {
                    var accountTypeCode = $"Döviz-{sellCode}";
                    var doviz = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == accountTypeCode);
                    var dovizBal = doviz?.Balance ?? 0f;
                    SellSelectedBalanceText.Text = $"Bakiye: {dovizBal.ToString("N4", CultureInfo.CurrentCulture)} {sellCode}";
                }
                else
                {
                    SellSelectedBalanceText.Text = "Bakiye: -";
                }
            }
        }

        private string GenerateSimpleIban()
        {
            var rnd = new Random();
            return "TR" + rnd.Next(100000000, 999999999).ToString();
        }
    }
}
