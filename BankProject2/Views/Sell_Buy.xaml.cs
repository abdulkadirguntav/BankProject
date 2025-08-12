using BankProject2.Data;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

namespace BankProject2
{
    public partial class Sell_BuyPage : UserControl
    {
        private readonly int _customerId;
        private Dictionary<string, double> currencyRates = new Dictionary<string, double>();
        private bool _isBuyAmountChanging = false;
        private bool _isBuyTotalChanging = false;
        private bool _isSellAmountChanging = false;
        private bool _isSellTotalChanging = false;
        private const double CommissionRate = 0.001; // %0.1 komisyon
        private const double MinTradeAmountBase = 1.0; // 1 birim döviz altı işlemi engelle

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
                currencyRates = rates.ToDictionary(c => c.CurrencyCode, c => c.RateToTRY);
                BuyCurrencyComboBox.ItemsSource = currencyRates.Keys;
                BuyCurrencyComboBox.SelectedIndex = 0;
                BuyCurrencyComboBox.SelectionChanged += (s, e) => { UpdateCurrencyRateText(BuyCurrencyComboBox, BuyCurrencyRateText); UpdateSelectedBalances(); BuyAmountTextBox.Text = BuyTotalTextBox.Text = string.Empty; };
                SellCurrencyComboBox.ItemsSource = GetUserOwnedCurrencies(context);
                SellCurrencyComboBox.SelectedIndex = SellCurrencyComboBox.Items.Count > 0 ? 0 : -1;
                SellCurrencyComboBox.SelectionChanged += (s, e) => { UpdateCurrencyRateText(SellCurrencyComboBox, SellCurrencyRateText); UpdateSelectedBalances(); SellAmountTextBox.Text = SellTotalTextBox.Text = string.Empty; };
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

        // Al sekmesi: Döviz miktarı değişirse TL hesapla
        private void BuyAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isBuyTotalChanging) return;
            _isBuyAmountChanging = true;
            if (BuyCurrencyComboBox.SelectedItem is string selectedCurrency && currencyRates.TryGetValue(selectedCurrency, out double rate))
            {
                if (double.TryParse(BuyAmountTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double amount))
                {
                    BuyTotalTextBox.Text = (amount * rate).ToString("N2", CultureInfo.CurrentCulture);
                    var fee = (amount * rate) * CommissionRate;
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
        // Al sekmesi: TL değişirse döviz hesapla
        private void BuyTotalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isBuyAmountChanging) return;
            _isBuyTotalChanging = true;
            if (BuyCurrencyComboBox.SelectedItem is string selectedCurrency && currencyRates.TryGetValue(selectedCurrency, out double rate))
            {
                if (double.TryParse(BuyTotalTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double total))
                {
                    BuyAmountTextBox.Text = (total / rate).ToString("N4", CultureInfo.CurrentCulture);
                    var fee = total * CommissionRate;
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
        // Sat sekmesi: Döviz miktarı değişirse TL hesapla
        private void SellAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSellTotalChanging) return;
            _isSellAmountChanging = true;
            if (SellCurrencyComboBox.SelectedItem is string selectedCurrency && currencyRates.TryGetValue(selectedCurrency, out double rate))
            {
                if (double.TryParse(SellAmountTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double amount))
                {
                    SellTotalTextBox.Text = (amount * rate).ToString("N2", CultureInfo.CurrentCulture);
                    var fee = (amount * rate) * CommissionRate;
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
        // Sat sekmesi: TL değişirse döviz hesapla
        private void SellTotalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSellAmountChanging) return;
            _isSellTotalChanging = true;
            if (SellCurrencyComboBox.SelectedItem is string selectedCurrency && currencyRates.TryGetValue(selectedCurrency, out double rate))
            {
                if (double.TryParse(SellTotalTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double total))
                {
                    SellAmountTextBox.Text = (total / rate).ToString("N4", CultureInfo.CurrentCulture);
                    var fee = total * CommissionRate;
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
            if (combo.SelectedItem is string selectedCurrency && currencyRates.TryGetValue(selectedCurrency, out double rate))
            {
                rateText.Text = $"1 {selectedCurrency} = {rate.ToString("N2", CultureInfo.CurrentCulture)} TL";
            }
        }

        private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Sadece sayı ve virgül/nokta kabul et
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            // Sadece rakam, nokta ve virgül
            foreach (char c in text)
            {
                if (!char.IsDigit(c) && c != ',' && c != '.')
                    return false;
            }
            return true;
        }

        private void BuyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (BuyCurrencyComboBox.SelectedItem is not string selectedCurrency || !currencyRates.TryGetValue(selectedCurrency, out double rate)) return;
            if (!double.TryParse(BuyAmountTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double amount) || amount <= 0) return;
            if (amount < MinTradeAmountBase) { System.Windows.MessageBox.Show($"Minimum işlem miktarı {MinTradeAmountBase} {selectedCurrency}"); return; }

            double tlCost = amount * rate;
            double fee = tlCost * CommissionRate;
            BuyFeeText.Text = $"Komisyon: {fee.ToString("N2") } TL";

            using (var context = new BankDbContext())
            {
                // Vadesiz TL hesabı
                var vadesiz = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == "Vadesiz");
                if (vadesiz == null) { System.Windows.MessageBox.Show("Vadesiz hesabınız bulunamadı."); return; }
                double totalDebit = tlCost + fee;
                if (vadesiz.Balance < (float)totalDebit) { System.Windows.MessageBox.Show("Yetersiz bakiye (TL)"); return; }

                // Döviz hesabı (yoksa aç). Hesap türünü 'Döviz-XXX' olarak tutuyoruz
                var accountTypeCode = $"Döviz-{selectedCurrency}";
                var dovizHesap = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == accountTypeCode);
                if (dovizHesap == null)
                {
                    dovizHesap = new Models.Accounts { CustomerID = _customerId, AccountType = accountTypeCode, Balance = 0, IBAN = GenerateSimpleIban() };
                    context.accounts.Add(dovizHesap);
                    // Önce hesap kaydını oluştur ki AccountID üretilebilsin
                    context.SaveChanges();
                }

                // Bakiye güncelle
                vadesiz.Balance -= (float)totalDebit;
                dovizHesap.Balance += (float)amount;

                // İşlemler
                context.transactions.Add(new Models.Transactions
                {
                    TransactionType = "FX-Buy",
                    TransactionDate = System.DateTime.Now,
                    Amount = (float)tlCost,
                    FromAccountID = vadesiz.AccountID,
                    ToAccountID = dovizHesap.AccountID,
                    Fee = fee,
                    Description = $"{amount:N4} {selectedCurrency} alım"
                });

                context.SaveChanges();
                System.Windows.MessageBox.Show("Alım işlemi başarılı");
                UpdateSelectedBalances();
            }
        }

        private void SellButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SellCurrencyComboBox.SelectedItem is not string selectedCurrency || !currencyRates.TryGetValue(selectedCurrency, out double rate)) return;
            if (!double.TryParse(SellAmountTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double amount) || amount <= 0) return;
            if (amount < MinTradeAmountBase) { System.Windows.MessageBox.Show($"Minimum işlem miktarı {MinTradeAmountBase} {selectedCurrency}"); return; }

            double tlProceeds = amount * rate;
            double fee = tlProceeds * CommissionRate;
            SellFeeText.Text = $"Komisyon: {fee.ToString("N2")} TL";

            using (var context = new BankDbContext())
            {
                // Vadesiz TL hesabı
                var vadesiz = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == "Vadesiz");
                if (vadesiz == null) { System.Windows.MessageBox.Show("Vadesiz hesabınız bulunamadı."); return; }

                // Döviz hesabı
                var accountTypeCode = $"Döviz-{selectedCurrency}";
                var dovizHesap = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == accountTypeCode);
                if (dovizHesap == null || dovizHesap.AccountID == 0)
                {
                    System.Windows.MessageBox.Show("Döviz hesabınız bulunamadı.");
                    return;
                }
                if (dovizHesap.Balance < (float)amount) { System.Windows.MessageBox.Show("Yetersiz döviz bakiyesi"); return; }

                // Bakiye güncelle
                dovizHesap.Balance -= (float)amount;
                vadesiz.Balance += (float)(tlProceeds - fee);

                // İşlemler
                context.transactions.Add(new Models.Transactions
                {
                    TransactionType = "FX-Sell",
                    TransactionDate = System.DateTime.Now,
                    Amount = (float)tlProceeds,
                    FromAccountID = dovizHesap.AccountID,
                    ToAccountID = vadesiz.AccountID,
                    Fee = fee,
                    Description = $"{amount:N4} {selectedCurrency} satış"
                });

                context.SaveChanges();
                System.Windows.MessageBox.Show("Satış işlemi başarılı");
                UpdateSelectedBalances();
            }
        }

        private void UpdateSelectedBalances()
        {
            using (var context = new BankDbContext())
            {
                // Buy sekmesinde: vadesiz TL bakiyesi
                var vadesiz = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == "Vadesiz");
                BuySelectedBalanceText.Text = vadesiz != null ? $"Vadesiz: {vadesiz.Balance.ToString("N2")} TL" : "Vadesiz: -";

                // Sell sekmesinde: seçilen döviz bakiyesi
                if (SellCurrencyComboBox.SelectedItem is string sellCode)
                {
                    var accountTypeCode = $"Döviz-{sellCode}";
                    var doviz = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == accountTypeCode);
                    SellSelectedBalanceText.Text = doviz != null ? $"Bakiye: {doviz.Balance.ToString("N4")} {sellCode}" : $"Bakiye: 0 {sellCode}";
                }
                else
                {
                    SellSelectedBalanceText.Text = "Bakiye: -";
                }
            }
        }

        private string GenerateSimpleIban()
        {
            var rnd = new System.Random();
            return "TR" + rnd.Next(100000000, 999999999).ToString();
        }
    }
}
