using System.Configuration;
using System.Data;
using System.Windows;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using BankProject2.Data;
using BankProject2.Models;
using System.Globalization;
namespace BankProject2
{
    public partial class App : Application
    {
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
                    if (alreadyExists)
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
