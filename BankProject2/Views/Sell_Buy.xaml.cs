using BankProject2.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

namespace BankProject2
{
    public partial class Sell_BuyPage : UserControl
    {
        private Dictionary<string, double> currencyRates = new Dictionary<string, double>();
        private bool _isBuyAmountChanging = false;
        private bool _isBuyTotalChanging = false;
        private bool _isSellAmountChanging = false;
        private bool _isSellTotalChanging = false;

        public Sell_BuyPage()
        {
            InitializeComponent();
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
                BuyCurrencyComboBox.SelectionChanged += (s, e) => { UpdateCurrencyRateText(BuyCurrencyComboBox, BuyCurrencyRateText); BuyAmountTextBox.Text = BuyTotalTextBox.Text = string.Empty; };
                SellCurrencyComboBox.ItemsSource = currencyRates.Keys;
                SellCurrencyComboBox.SelectedIndex = 0;
                SellCurrencyComboBox.SelectionChanged += (s, e) => { UpdateCurrencyRateText(SellCurrencyComboBox, SellCurrencyRateText); SellAmountTextBox.Text = SellTotalTextBox.Text = string.Empty; };
                UpdateCurrencyRateText(BuyCurrencyComboBox, BuyCurrencyRateText);
                UpdateCurrencyRateText(SellCurrencyComboBox, SellCurrencyRateText);
            }
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
                }
                else
                {
                    BuyTotalTextBox.Text = string.Empty;
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
                }
                else
                {
                    BuyAmountTextBox.Text = string.Empty;
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
                }
                else
                {
                    SellTotalTextBox.Text = string.Empty;
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
                }
                else
                {
                    SellAmountTextBox.Text = string.Empty;
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
    }
}
