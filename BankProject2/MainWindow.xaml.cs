using BankProject2.Data;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using BankProject2.Models;
using System.Windows.Data;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace BankProject2
{
    public partial class MainWindow : Window
    {
        private bool isLoggedIn = false;
        private Customer currentUser = null;

        public MainWindow()
        {
            InitializeComponent();
            LoadHomePage();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            if (isLoggedIn)
            {
                MessageBox.Show("Giriş yaptıktan sonra ana sayfaya tekrar erişemezsiniz.");
                return;
            }
            LoadHomePage();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (isLoggedIn)
            {
                MessageBox.Show("Zaten giriş yapmış durumdasınız.");
                return;
            }
            LoadRegisterPage();
        }

        private void Banka_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoggedIn)
            {
                MessageBox.Show("Lütfen önce giriş yapınız.");
                return;
            }
            MainContent.Content = new BankPage(currentUser.CustomerID);
        }

        private void CreditCard_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoggedIn)
            {
                MessageBox.Show("Lütfen önce giriş yapınız.");
                return;
            }
            MainContent.Content = new CreditCardPage(currentUser.CustomerID);
        }

        private void LoadHomePage()
        {
            if (isLoggedIn) return;

            var panel = new StackPanel
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F6FB")),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 40, 0, 0),
                Width = 420
            };

            // Başlık
            var titlePanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 18) };
            var icon = new TextBlock { Text = "📱", FontSize = 38, Margin = new Thickness(0, 0, 12, 0), VerticalAlignment = VerticalAlignment.Center };
            var title = new TextBlock { Text = "Giriş Yap", FontSize = 30, FontWeight = FontWeights.Bold, Foreground = Brushes.Black, VerticalAlignment = VerticalAlignment.Center };
            titlePanel.Children.Add(icon);
            titlePanel.Children.Add(title);
            panel.Children.Add(titlePanel);

            // Açıklama
            panel.Children.Add(new TextBlock
            {
                Text = "Telefon numaranız ve şifreniz ile giriş yapın.",
                FontSize = 15,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888")),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 18)
            });

            // Telefon alanı
            panel.Children.Add(new TextBlock { Text = "Telefon Numarası", FontSize = 15, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444")), Margin = new Thickness(0, 0, 0, 4) });

            var phoneBox = new TextBox
            {
                Width = 340,
                Height = 40,
                FontSize = 15,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 16)
            };

            // Sadece sayı girilsin
            phoneBox.PreviewTextInput += (s, e) =>
            {
                e.Handled = !e.Text.All(char.IsDigit);
            };
            DataObject.AddPastingHandler(phoneBox, (s, e) =>
            {
                if (e.DataObject.GetDataPresent(DataFormats.Text))
                {
                    string pasted = (string)e.DataObject.GetData(DataFormats.Text);
                    if (!pasted.All(char.IsDigit))
                        e.CancelCommand();
                }
                else
                {
                    e.CancelCommand();
                }
            });

            panel.Children.Add(phoneBox);

            // Şifre alanı
            panel.Children.Add(new TextBlock { Text = "Şifre", FontSize = 15, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444")), Margin = new Thickness(0, 0, 0, 4) });

            var passwordBox = new PasswordBox
            {
                Width = 340,
                Height = 40,
                FontSize = 15,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 16)
            };
            panel.Children.Add(passwordBox);

            // Giriş Butonu
            var loginBtn = new Button
            {
                Content = "Giriş Yap",
                Width = 180,
                Height = 38,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F7D90F")),
                Foreground = Brushes.Black,
                FontWeight = FontWeights.SemiBold,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand,
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            loginBtn.Click += (s, e) =>
            {
                string phone = phoneBox.Text;
                string password = passwordBox.Password;
                using (var context = new BankDbContext())
                {
                    var user = context.customer.FirstOrDefault(c => c.PhoneNumber == phone && c.CustomerPassword == password);
                    if (user != null)
                    {
                        isLoggedIn = true;
                        currentUser = user;
                        MainContent.Content = new BankPage(user.CustomerID);
                    }
                    else
                    {
                        MessageBox.Show("Telefon numarası veya şifre yanlış!");
                    }
                }
            };
            panel.Children.Add(loginBtn);

            MainContent.Content = panel;
        }

        private void LoadRegisterPage()
        {
            if (isLoggedIn) return;

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };

            var panel = new StackPanel
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F6FB")),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 20),
                Width = 500
            };

            // Başlık
            var titlePanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 20) };
            var icon = new TextBlock { Text = "📝", FontSize = 38, Margin = new Thickness(0, 0, 12, 0), VerticalAlignment = VerticalAlignment.Center };
            var title = new TextBlock { Text = "Yeni Hesap Aç", FontSize = 30, FontWeight = FontWeights.Bold, Foreground = Brushes.Black, VerticalAlignment = VerticalAlignment.Center };
            titlePanel.Children.Add(icon);
            titlePanel.Children.Add(title);
            panel.Children.Add(titlePanel);

            // Açıklama
            panel.Children.Add(new TextBlock
            {
                Text = "Bilgilerinizi doldurun ve hesabınızı oluşturun.",
                FontSize = 15,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888")),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 25)
            });

            // Ad alanı
            panel.Children.Add(new TextBlock { Text = "Ad *", FontSize = 15, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444")), Margin = new Thickness(0, 0, 0, 4) });
            var firstNameBox = new TextBox
            {
                Width = 420,
                Height = 40,
                FontSize = 15,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 16)
            };
            panel.Children.Add(firstNameBox);

            // Soyad alanı
            panel.Children.Add(new TextBlock { Text = "Soyad *", FontSize = 15, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444")), Margin = new Thickness(0, 0, 0, 4) });
            var lastNameBox = new TextBox
            {
                Width = 420,
                Height = 40,
                FontSize = 15,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 16)
            };
            panel.Children.Add(lastNameBox);

            // Telefon alanı
            panel.Children.Add(new TextBlock { Text = "Telefon Numarası *", FontSize = 15, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444")), Margin = new Thickness(0, 0, 0, 4) });
            var phoneBox = new TextBox
            {
                Width = 420,
                Height = 40,
                FontSize = 15,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 16)
            };
            // Sadece sayı girilsin
            phoneBox.PreviewTextInput += (s, e) =>
            {
                e.Handled = !e.Text.All(char.IsDigit);
            };
            DataObject.AddPastingHandler(phoneBox, (s, e) =>
            {
                if (e.DataObject.GetDataPresent(DataFormats.Text))
                {
                    string pasted = (string)e.DataObject.GetData(DataFormats.Text);
                    if (!pasted.All(char.IsDigit))
                        e.CancelCommand();
                }
                else
                {
                    e.CancelCommand();
                }
            });
            panel.Children.Add(phoneBox);

            // Şifre alanı
            panel.Children.Add(new TextBlock { Text = "Şifre *", FontSize = 15, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444")), Margin = new Thickness(0, 0, 0, 4) });
            var passwordBox = new PasswordBox
            {
                Width = 420,
                Height = 40,
                FontSize = 15,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 16)
            };
            panel.Children.Add(passwordBox);

            // Şifre tekrar alanı
            panel.Children.Add(new TextBlock { Text = "Şifre Tekrar *", FontSize = 15, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444")), Margin = new Thickness(0, 0, 0, 4) });
            var passwordConfirmBox = new PasswordBox
            {
                Width = 420,
                Height = 40,
                FontSize = 15,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 16)
            };
            panel.Children.Add(passwordConfirmBox);

            // Aylık gelir alanı
            panel.Children.Add(new TextBlock { Text = "Aylık Gelir (TL)", FontSize = 15, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444")), Margin = new Thickness(0, 0, 0, 4) });
            var incomeBox = new TextBox
            {
                Width = 420,
                Height = 40,
                FontSize = 15,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 16),
                Text = "0"
            };
            // Sadece sayı girilsin
            incomeBox.PreviewTextInput += (s, e) =>
            {
                e.Handled = !e.Text.All(c => char.IsDigit(c) || c == '.');
            };
            panel.Children.Add(incomeBox);

            // Kredi kartı isteği
            var creditCardPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0, 0, 0, 20) };
            var creditCardCheckBox = new CheckBox
            {
                Content = "Kredi kartı istiyorum",
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0)
            };
            creditCardPanel.Children.Add(creditCardCheckBox);
            panel.Children.Add(creditCardPanel);

            // Butonlar
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) };

            // Geri dön butonu
            var backBtn = new Button
            {
                Content = "Geri Dön",
                Width = 150,
                Height = 40,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888")),
                Foreground = Brushes.White,
                FontWeight = FontWeights.SemiBold,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand,
                Margin = new Thickness(0, 0, 10, 0)
            };
            backBtn.Click += (s, e) => LoadHomePage();
            buttonPanel.Children.Add(backBtn);

            // Kayıt ol butonu
            var registerBtn = new Button
            {
                Content = "Hesap Aç",
                Width = 150,
                Height = 40,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F7D90F")),
                Foreground = Brushes.Black,
                FontWeight = FontWeights.SemiBold,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };

            registerBtn.Click += (s, e) =>
            {
                // Validasyon
                if (string.IsNullOrWhiteSpace(firstNameBox.Text) || string.IsNullOrWhiteSpace(lastNameBox.Text) || 
                    string.IsNullOrWhiteSpace(phoneBox.Text) || string.IsNullOrWhiteSpace(passwordBox.Password))
                {
                    MessageBox.Show("Lütfen tüm zorunlu alanları doldurun!");
                    return;
                }

                if (passwordBox.Password != passwordConfirmBox.Password)
                {
                    MessageBox.Show("Şifreler eşleşmiyor!");
                    return;
                }

                if (passwordBox.Password.Length < 6)
                {
                    MessageBox.Show("Şifre en az 6 karakter olmalıdır!");
                    return;
                }

                if (phoneBox.Text.Length < 6)
                {
                    MessageBox.Show("Geçerli bir telefon numarası giriniz!");
                    return;
                }

                float monthlyIncome = 0;
                if (!float.TryParse(incomeBox.Text, out monthlyIncome))
                {
                    monthlyIncome = 0;
                }

                try
                {
                    using (var context = new BankDbContext())
                    {
                        // Telefon numarası kontrolü
                        var existingUser = context.customer.FirstOrDefault(c => c.PhoneNumber == phoneBox.Text);
                        if (existingUser != null)
                        {
                            MessageBox.Show("Bu telefon numarası zaten kayıtlı!");
                            return;
                        }

                        // Yeni müşteri oluştur
                        var newCustomer = new Customer
                        {
                            FirstName = firstNameBox.Text.Trim(),
                            LastName = lastNameBox.Text.Trim(),
                            PhoneNumber = phoneBox.Text.Trim(),
                            CustomerPassword = passwordBox.Password,
                            MonthlyIncome = monthlyIncome
                        };

                        context.customer.Add(newCustomer);
                        context.SaveChanges();

                        // Vadesiz hesap oluştur
                        var newAccount = new Accounts
                        {
                            AccountType = "Vadesiz",
                            Balance = 0,
                            IBAN = GenerateIBAN(),
                            CustomerID = newCustomer.CustomerID
                        };

                        context.accounts.Add(newAccount);
                        context.SaveChanges();

                        // Kredi kartı isteği varsa
                        if (creditCardCheckBox.IsChecked == true)
                        {
                            var newCreditCard = new CreditCard
                            {
                                CustomerID = newCustomer.CustomerID,
                                //CardNumber = GenerateCardNumber(),
                                //CardExpiry = DateTime.Now.AddYears(3),
                                //CardCVV = GenerateCVV(),
                                Limit = monthlyIncome * 2, // Aylık gelirin 2 katı
                                CurrentDebt = 0,
                                RiskScore = CalculateRiskScore(monthlyIncome),
                                LatePaymentCount = 0
                            };

                            context.creditCard.Add(newCreditCard);
                            context.SaveChanges();
                        }

                        MessageBox.Show("Hesabınız başarıyla oluşturuldu! Giriş yapabilirsiniz.");
                        LoadHomePage();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata oluştu: {ex.Message}");
                }
            };
            buttonPanel.Children.Add(registerBtn);

            panel.Children.Add(buttonPanel);
            scrollViewer.Content = panel;
            MainContent.Content = scrollViewer;
        }

        // Yardımcı metodlar
        private string GenerateIBAN()
        {
            // Basit IBAN oluşturma (TR + 2 kontrol hanesi + 4 banka kodu + 16 hesap no)
            Random random = new Random();
            string bankCode = "0015"; // VakıfBank kodu
            string accountNumber = random.Next(100000000, 999999999).ToString() + random.Next(10000000, 99999999).ToString();
            return "TR" + random.Next(10, 99) + bankCode + accountNumber;
        }

        //private string GenerateCardNumber()
        //{
        //    Random random = new Random();
        //    string cardNumber = "4"; // Visa kartı
        //    for (int i = 0; i < 15; i++)
        //    {
        //        cardNumber += random.Next(0, 10);
        //    }
        //    return cardNumber;
        //}

        private string GenerateCVV()
        {
            Random random = new Random();
            return random.Next(100, 1000).ToString();
        }

        private int CalculateRiskScore(float monthlyIncome)
        {
            if (monthlyIncome >= 10000) return 1; // Düşük risk
            if (monthlyIncome >= 5000) return 2; // Orta risk
            return 3; // Yüksek risk
        }

        private void Currency_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoggedIn)
            {
                MessageBox.Show("Lütfen önce giriş yapınız.");
                return;
            }
            MainContent.Content = new CurrencyPage(currentUser.CustomerID);
        }

        private void Current_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CurrentPage();
        }

        public void Sell_Buy_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoggedIn)
            {
                MessageBox.Show("Lütfen önce giriş yapınız.");
                return;
            }
            MainContent.Content = new Sell_BuyPage(currentUser.CustomerID);
        }

        private void log_out_btnClick(object sender, RoutedEventArgs e)
        {
            Logout();
        }

        public void Logout()
        {
            isLoggedIn = false;
            currentUser = null;
            MessageBox.Show("Başarıyla çıkış yaptınız.");
            LoadHomePage();
        }
    }
}
