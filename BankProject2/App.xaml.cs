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
                    
                    // Örnek müşteriler ekle (eğer yoksa)
                    if (!context.customer.Any())
                    {
                        // Demo Müşteri 1
                        var customer1 = new Customer
                        {
                            FirstName = "Ahmet",
                            LastName = "Yılmaz",
                            PhoneNumber = "5551234567",
                            CustomerPassword = "123456",
                            MonthlyIncome = 15000f
                        };
                        context.customer.Add(customer1);

                        // Demo Müşteri 2
                        var customer2 = new Customer
                        {
                            FirstName = "Ayşe",
                            LastName = "Demir",
                            PhoneNumber = "5559876543",
                            CustomerPassword = "123456",
                            MonthlyIncome = 12000f
                        };
                        context.customer.Add(customer2);
                        
                        context.SaveChanges();

                        // Müşteri ID'lerini al
                        var ahmet = context.customer.First(c => c.PhoneNumber == "5551234567");
                        var ayse = context.customer.First(c => c.PhoneNumber == "5559876543");

                        // Ahmet'in hesapları
                        var ahmetVadesiz = new Accounts
                        {
                            CustomerID = ahmet.CustomerID,
                            AccountType = "Vadesiz",
                            Balance = 5000f,
                            IBAN = "TR123456789012345678901234"
                        };
                        context.accounts.Add(ahmetVadesiz);

                        var ahmetVadeli = new Accounts
                        {
                            CustomerID = ahmet.CustomerID,
                            AccountType = "Vadeli",
                            Balance = 10000f,
                            StartDate = DateTime.Now.AddDays(-30),
                            MaturityDate = DateTime.Now.AddDays(60),
                            InterestRate = 0.15f,
                            PrincipalAmount = 10000f,
                            AccruedInterest = 125f,
                            IBAN = "TR123456789012345678901235"
                        };
                        context.accounts.Add(ahmetVadeli);

                        // Ayşe'nin hesapları
                        var ayseVadesiz = new Accounts
                        {
                            CustomerID = ayse.CustomerID,
                            AccountType = "Vadesiz",
                            Balance = 3000f,
                            IBAN = "TR987654321098765432109876"
                        };
                        context.accounts.Add(ayseVadesiz);

                        var ayseVadeli = new Accounts
                        {
                            CustomerID = ayse.CustomerID,
                            AccountType = "Vadeli",
                            Balance = 8000f,
                            StartDate = DateTime.Now.AddDays(-15),
                            MaturityDate = DateTime.Now.AddDays(45),
                            InterestRate = 0.12f,
                            PrincipalAmount = 8000f,
                            AccruedInterest = 40f,
                            IBAN = "TR987654321098765432109877"
                        };
                        context.accounts.Add(ayseVadeli);

                        context.SaveChanges();

                        // Kredi kartları
                        var ahmetKart = new CreditCard
                        {
                            CustomerID = ahmet.CustomerID,
                            CardNumber = "4532123456789012",
                            CardExpiry = DateTime.Now.AddYears(3),
                            CardCVV = "123",
                            Limit = 15000f,
                            CurrentDebt = 2500f,
                            RiskScore = 750,
                            LatePaymentCount = 0
                        };
                        context.creditCard.Add(ahmetKart);

                        var ayseKart = new CreditCard
                        {
                            CustomerID = ayse.CustomerID,
                            CardNumber = "4532987654321098",
                            CardExpiry = DateTime.Now.AddYears(2),
                            CardCVV = "456",
                            Limit = 12000f,
                            CurrentDebt = 1800f,
                            RiskScore = 800,
                            LatePaymentCount = 0
                        };
                        context.creditCard.Add(ayseKart);

                        context.SaveChanges();

                        // Örnek işlemler
                        var ahmetVadesizAccount = context.accounts.First(a => a.CustomerID == ahmet.CustomerID && a.AccountType == "Vadesiz");
                        var ayseVadesizAccount = context.accounts.First(a => a.CustomerID == ayse.CustomerID && a.AccountType == "Vadesiz");

                        // Ahmet'ten Ayşe'ye transfer
                        var transfer1 = new Transactions
                        {
                            FromAccountID = ahmetVadesizAccount.AccountID,
                            ToAccountID = ayseVadesizAccount.AccountID,
                            Amount = 500f,
                            TransactionType = "Transfer",
                            TransactionDate = DateTime.Now.AddDays(-2),
                            Description = "Ahmet'ten Ayşe'ye transfer"
                        };
                        context.transactions.Add(transfer1);

                        // Ayşe'den Ahmet'e transfer
                        var transfer2 = new Transactions
                        {
                            FromAccountID = ayseVadesizAccount.AccountID,
                            ToAccountID = ahmetVadesizAccount.AccountID,
                            Amount = 200f,
                            TransactionType = "Transfer",
                            TransactionDate = DateTime.Now.AddDays(-1),
                            Description = "Ayşe'den Ahmet'e transfer"
                        };
                        context.transactions.Add(transfer2);

                        // Para yatırma işlemleri
                        var deposit1 = new Transactions
                        {
                            FromAccountID = null,
                            ToAccountID = ahmetVadesizAccount.AccountID,
                            Amount = 1000f,
                            TransactionType = "Deposit",
                            TransactionDate = DateTime.Now.AddDays(-5),
                            Description = "ATM'den para yatırma"
                        };
                        context.transactions.Add(deposit1);

                        var deposit2 = new Transactions
                        {
                            FromAccountID = null,
                            ToAccountID = ayseVadesizAccount.AccountID,
                            Amount = 800f,
                            TransactionType = "Deposit",
                            TransactionDate = DateTime.Now.AddDays(-3),
                            Description = "Şube'den para yatırma"
                        };
                        context.transactions.Add(deposit2);

                        context.SaveChanges();
                    }
                    
                    // Örnek döviz kurları ekle (eğer yoksa)
                    if (!context.currency.Any())
                    {
                        var sampleCurrencies = new List<Currency>
                        {
                            new Currency { CurrencyCode = "USD", CurrencyDate = DateTime.Today, RateToTRY = 32.15 },
                            new Currency { CurrencyCode = "EUR", CurrencyDate = DateTime.Today, RateToTRY = 35.20 },
                            new Currency { CurrencyCode = "GBP", CurrencyDate = DateTime.Today, RateToTRY = 41.50 },
                            new Currency { CurrencyCode = "JPY", CurrencyDate = DateTime.Today, RateToTRY = 0.21 },
                            new Currency { CurrencyCode = "CHF", CurrencyDate = DateTime.Today, RateToTRY = 36.80 }
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
