using BankProject2.Data;
using System.Windows.Controls;
using BankProject2.Models;
using System.Linq;
using System;

namespace BankProject2
{
    public partial class BankPage : UserControl
    {
        private int _customerId;
        
        public BankPage(int customerId)
        {
            InitializeComponent();
            _customerId = customerId;
            LoadAccountData();
        }

        private void LoadAccountData()
        {
            using (var context = new BankDbContext())
            {
                // Vadeli hesap faiz hesaplama ve ödeme
                ProcessVadeliInterest(context);
                
                // Maaş yatırma kontrolü
                ProcessSalaryPayment(context);
                
                // Vadesiz hesap
                var vadesiz = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == "Vadesiz");
                var vadesizBalanceText = this.FindName("VadesizBalanceText") as TextBlock;
                if (vadesizBalanceText != null)
                    vadesizBalanceText.Text = vadesiz != null ? vadesiz.Balance.ToString("N2") + " TL" : "-";
                var vadesizTransGrid = this.FindName("VadesizTransactionDataGrid") as DataGrid;
                if (vadesizTransGrid != null)
                {
                    if (vadesiz != null)
                    {
                        var vadesizTrans = context.transactions
                            .Where(t => t.FromAccountID == vadesiz.AccountID || t.ToAccountID == vadesiz.AccountID)
                            .OrderByDescending(t => t.TransactionDate)
                            .ToList();
                        vadesizTransGrid.ItemsSource = vadesizTrans;
                    }
                    else
                    {
                        vadesizTransGrid.ItemsSource = null;
                    }
                }

                // Vadeli hesap
                var vadeli = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == "Vadeli");
                var vadeliBalanceText = this.FindName("VadeliBalanceText") as TextBlock;
                if (vadeliBalanceText != null)
                    vadeliBalanceText.Text = vadeli != null ? vadeli.Balance.ToString("N2") + " TL" : "-";
                var vadeliTransGrid = this.FindName("VadeliTransactionDataGrid") as DataGrid;
                if (vadeliTransGrid != null)
                {
                    if (vadeli != null)
                    {
                        var vadeliTrans = context.transactions
                            .Where(t => t.FromAccountID == vadeli.AccountID || t.ToAccountID == vadeli.AccountID)
                            .OrderByDescending(t => t.TransactionDate)
                            .ToList();
                        vadeliTransGrid.ItemsSource = vadeliTrans;
                    }
                    else
                    {
                        vadeliTransGrid.ItemsSource = null;
                    }
                }
            }
        }

        private void ProcessVadeliInterest(BankDbContext context)
        {
            var vadeliHesaplar = context.accounts
                .Where(a => a.CustomerID == _customerId && a.AccountType == "Vadeli" && !a.IsBroken.Value)
                .ToList();

            foreach (var vadeliHesap in vadeliHesaplar)
            {
                // Vade dolmuş mu kontrol et
                if (vadeliHesap.MaturityDate.HasValue && vadeliHesap.MaturityDate.Value <= DateTime.Now)
                {
                    // Daha önce faiz ödemesi yapılmış mı kontrol et
                    var existingPayment = context.transactions
                        .FirstOrDefault(t => t.FromAccountID == vadeliHesap.AccountID && 
                                           t.TransactionType == "Vadeli Faiz Ödemesi" &&
                                           t.TransactionDate.Date == DateTime.Now.Date);
                    
                    if (existingPayment != null)
                    {
                        continue; // Bugün zaten faiz ödemesi yapılmış
                    }
                    
                    // Faiz hesapla
                    float faizTutari = CalculateInterest(vadeliHesap);
                    
                    if (faizTutari > 0)
                    {
                        // Vadesiz hesabı bul
                        var vadesizHesap = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == "Vadesiz");
                        
                        if (vadesizHesap != null)
                        {
                            // Faizi vadesiz hesaba aktar
                            vadesizHesap.Balance += faizTutari;
                            
                            // Vadeli hesabı güncelle
                            vadeliHesap.Balance = vadeliHesap.PrincipalAmount ?? 0; // Ana para kalır
                            vadeliHesap.AccruedInterest = 0; // Biriken faiz sıfırla
                            
                            // İşlem kaydı oluştur
                            context.transactions.Add(new Transactions
                            {
                                TransactionType = "Vadeli Faiz Ödemesi",
                                TransactionDate = DateTime.Now,
                                Amount = faizTutari,
                                FromAccountID = vadeliHesap.AccountID,
                                ToAccountID = vadesizHesap.AccountID,
                                Description = $"Vadeli hesap vadesi doldu. Faiz tutarı: {faizTutari:N2} TL"
                            });
                        }
                    }
                }
            }
            
            context.SaveChanges();
        }

        private float CalculateInterest(Accounts vadeliHesap)
        {
            if (!vadeliHesap.PrincipalAmount.HasValue || !vadeliHesap.InterestRate.HasValue || !vadeliHesap.MaturityDate.HasValue || !vadeliHesap.StartDate.HasValue)
                return 0;

            float anaPara = vadeliHesap.PrincipalAmount.Value;
            float faizOrani = vadeliHesap.InterestRate.Value;
            
            // Vade dolmuş mu kontrol et
            if (vadeliHesap.MaturityDate.Value > DateTime.Now)
                return 0; // Vade henüz dolmamış
            
            // Vade dolmuş, faiz hesapla
            // Basit faiz hesaplama (yıllık)
            // Vade süresini hesapla (gün cinsinden)
            TimeSpan vadeSuresi = vadeliHesap.MaturityDate.Value - vadeliHesap.StartDate.Value;
            int gunSayisi = (int)vadeSuresi.TotalDays;
            
            // Yıllık faiz hesaplama
            float yillikFaiz = anaPara * (faizOrani / 100f);
            float gunlukFaiz = yillikFaiz / 365f;
            float toplamFaiz = gunlukFaiz * gunSayisi;
            
            return toplamFaiz;
        }

        private void ProcessSalaryPayment(BankDbContext context)
        {
            var customer = context.customer.FirstOrDefault(c => c.CustomerID == _customerId);
            if (customer == null || customer.MonthlyIncome <= 0)
                return;

            var vadesizHesap = context.accounts.FirstOrDefault(a => a.CustomerID == _customerId && a.AccountType == "Vadesiz");
            if (vadesizHesap == null)
                return;

            // Bugünün tarihi
            DateTime today = DateTime.Now;
            
            // Basit maaş yatırma kontrolü (her gün kontrol et, gerçek uygulamada daha karmaşık olabilir)
            // Maaş yatırma günü kontrolü (varsayılan 15. gün)
            if (today.Day == 15) // Sabit maaş günü
            {
                // Maaş yatır
                vadesizHesap.Balance += customer.MonthlyIncome;
                
                // İşlem kaydı oluştur
                context.transactions.Add(new Transactions
                {
                    TransactionType = "Maaş Yatırma",
                    TransactionDate = today,
                    Amount = customer.MonthlyIncome,
                    FromAccountID = null, // Banka tarafından yatırılıyor
                    ToAccountID = vadesizHesap.AccountID,
                    Description = $"Maaş yatırıldı: {customer.MonthlyIncome:N2} TL"
                });
                
                context.SaveChanges();
            }
        }

        private void ParaGonderButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var window = new System.Windows.Window
            {
                Content = new TransferPage(_customerId),
                Width = 400,
                Height = 500,
                Title = "Para Gönder"
            };
            
            // Transfer penceresi kapandığında bakiyeleri yenile
            window.Closed += (s, args) => LoadAccountData();
            window.ShowDialog();
        }

        private void VadeliHesapButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var window = new System.Windows.Window
            {
                Content = new VadeliHesap(_customerId),
                Width = 600,
                Height = 700,
                Title = "Vadeli Hesap İşlemleri"
            };
            
            // Vadeli hesap penceresi kapandığında bakiyeleri yenile
            window.Closed += (s, args) => LoadAccountData();
            window.ShowDialog();
        }

        private int GetCustomerId() => _customerId;
    }
}
