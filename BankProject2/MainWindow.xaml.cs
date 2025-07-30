using BankProject2;
using BankProject2.Data;
using BankProject2.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BankProject2
{
    public partial class MainWindow : Window
    {
        private bool isLoggedIn = false;
        private BankProject2.Models.Customer currentUser = null;
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

        private void Banka_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoggedIn)
            {
                MessageBox.Show("Lütfen önce giriş yapınız.");
                return;
            }
            int currentCustomerId = currentUser != null ? currentUser.CustomerID : 1;
            MainContent.Content = new BankPage(currentCustomerId);
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
            if (isLoggedIn)
            {
                // Giriş yapıldıysa ana sayfa gösterilmesin
                return;
            }
            var panel = new StackPanel
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F4F6FB")),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 40, 0, 0),
                Width = 420
            };

            var titlePanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 18) };
            var icon = new TextBlock { Text = "📱", FontSize = 38, Margin = new Thickness(0, 0, 12, 0), VerticalAlignment = VerticalAlignment.Center };
            var title = new TextBlock { Text = "Giriş Yap", FontSize = 30, FontWeight = FontWeights.Bold, Foreground = System.Windows.Media.Brushes.Black, VerticalAlignment = VerticalAlignment.Center };
            titlePanel.Children.Add(icon);
            titlePanel.Children.Add(title);
            panel.Children.Add(titlePanel);

            var desc = new TextBlock { Text = "Telefon numaranız ve şifreniz ile giriş yapın.", FontSize = 15, Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888")), HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 18) };
            panel.Children.Add(desc);

            var phoneLabel = new TextBlock { Text = "Telefon Numarası", FontSize = 15, FontWeight = FontWeights.SemiBold, Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#444")), Margin = new Thickness(0, 0, 0, 4) };
            panel.Children.Add(phoneLabel);
            var phoneBox = new TextBox { Width = 340, Height = 36, FontSize = 15, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(10), Margin = new Thickness(0, 0, 0, 16) };
            panel.Children.Add(phoneBox);

            var passLabel = new TextBlock { Text = "Şifre", FontSize = 15, FontWeight = FontWeights.SemiBold, Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#444")), Margin = new Thickness(0, 0, 0, 4) };
            panel.Children.Add(passLabel);
            var passwordBox = new PasswordBox { Width = 340, Height = 36, FontSize = 15, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(10), Margin = new Thickness(0, 0, 0, 16) };
            panel.Children.Add(passwordBox);

            var loginBtn = new Button
            {
                Content = "Giriş Yap",
                Width = 180,
                Height = 38,
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F7D90F")),
                Foreground = System.Windows.Media.Brushes.Black,
                FontWeight = FontWeights.SemiBold,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
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
                        BankPage bankPage = new BankPage(user.CustomerID);
                        MainContent.Content = bankPage;
                    }
                    else
                    {
                        isLoggedIn = false;
                        currentUser = null;
                        MessageBox.Show("Telefon numarası veya şifre yanlış!");
                    }
                }
            };
            panel.Children.Add(loginBtn);

            MainContent.Content = panel;
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

        private void Sell_Buy_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoggedIn)
            {
                MessageBox.Show("Lütfen önce giriş yapınız.");
                return;
            }
            MainContent.Content = new Sell_BuyPage();
        }
        
    }
}


