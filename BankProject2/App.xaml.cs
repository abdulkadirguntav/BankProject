using System.Configuration;
using System.Data;
using System.Windows;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using BankProject2.Data;
using BankProject2.Models;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace BankProject2
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Veritabanını başlat ve örnek verileri ekle
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using (var context = new BankDbContext())
                {
                    // Veritabanını oluştur
                    context.Database.EnsureCreated();
                    
                    // Örnek müşteri ekle (eğer yoksa)
                    if (!context.customer.Any())
                    {
                        var sampleCustomer = new Customer
                        {
                            FirstName = "Demo",
                            LastName = "User",
                            PhoneNumber = "5551234567",
                            CustomerPassword = "123456",
                            MonthlyIncome = 10000f
                        };
                        context.customer.Add(sampleCustomer);
                        context.SaveChanges();
                    }
                    
                    // Örnek döviz kurları ekle (eğer yoksa)
                    if (!context.currency.Any())
                    {
                        var sampleCurrencies = new List<Currency>
                        {
                            new Currency { CurrencyCode = "USD", CurrencyDate = DateTime.Today, RateToTRY = 32.15 },
                            new Currency { CurrencyCode = "EUR", CurrencyDate = DateTime.Today, RateToTRY = 35.20 },
                            new Currency { CurrencyCode = "GBP", CurrencyDate = DateTime.Today, RateToTRY = 41.50 }
                        };
                        context.currency.AddRange(sampleCurrencies);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veritabanı başlatılırken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static async Task FetchAndSaveExchangeRatesAsync()
        {
            string url = "https://www.tcmb.gov.tr/kurlar/today.xml";

            using (HttpClient client = new HttpClient())
            {
                string xmlString;
                try
                {
                    xmlString = await client.GetStringAsync(url);
                }
                catch
                {
                    return;
                }

                XDocument doc;
                try
                {
                    doc = XDocument.Parse(xmlString);
                }
                catch
                {
                    return;
                }

                // XML'in tarihini al
                string dateStr = doc.Root.Attribute("Date").Value; // "08/12/2025" formatında gelir
                DateTime xmlDate = DateTime.ParseExact(dateStr, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                using (var context = new BankDbContext())
                {
                    // Eğer bu tarih zaten kayıtlıysa ekleme
                    bool alreadyExists = context.currency.Any(c => c.CurrencyDate.Date == xmlDate.Date);
                    if (!alreadyExists) // Düzeltme: alreadyExists true ise ekleme yapmamalı
                    {
                        foreach (var currency in doc.Descendants("Currency"))
                        {
                            string kod = currency.Attribute("Kod").Value;
                            string buyingStr = currency.Element("ForexBuying").Value;

                            if (double.TryParse(buyingStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double rate))
                            {
                                var newCurrency = new Currency
                                {
                                    CurrencyCode = kod,
                                    CurrencyDate = xmlDate, // Burada XML'den gelen tarih
                                    RateToTRY = rate
                                };

                                context.currency.Add(newCurrency);
                            }
                        }
                        context.SaveChanges();
                    }
                    
                }
            }
        }

    }
}
