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

                using (var context = new BankDbContext())
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
                                CurrencyDate = DateTime.Now,
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
