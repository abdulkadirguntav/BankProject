using System;
using System.Windows.Controls;
using System.Windows;
using BankProject2.Data;
using BankProject2.Models;
using System.Linq;

namespace BankProject2
{
    public partial class VadeliHesap : UserControl
    {
        private int customerId;
        
        public VadeliHesap(int customerId)
        {
            InitializeComponent();
            this.customerId = customerId;
            LoadVadeliHesapData();
            LoadVadesizBalance();
        }

        private void LoadVadesizBalance()
        {
            using (var context = new BankDbContext())
            {
                var vadesiz = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadesiz");
                VadesizBalanceText.Text = vadesiz != null ? $"{vadesiz.Balance:N2} TL" : "0,00 TL";
            }
        }

        private void LoadVadeliHesapData()
        {
            using (var context = new BankDbContext())
            {
                var vadeliHesap = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadeli" && a.IsBroken != true);
                
                if (vadeliHesap != null)
                {
                    // Biriken faizi hesapla ve güncelle
                    UpdateAccruedInterest(context, vadeliHesap);
                    
                    // Bakiye bilgilerini göster
                    PrincipalAmountText.Text = (vadeliHesap.PrincipalAmount ?? 0).ToString("N2") + " TL";
                    AccruedInterestText.Text = (vadeliHesap.AccruedInterest ?? 0).ToString("N2") + " TL";
                    InterestRateText.Text = (vadeliHesap.InterestRate ?? 0).ToString("N1") + "%";
                    MaturityDateText.Text = vadeliHesap.MaturityDate?.ToString("dd.MM.yyyy") ?? "-";
                    
                    // Toplam tutar ve kalan gün hesapla
                    float totalAmount = (vadeliHesap.PrincipalAmount ?? 0) + (vadeliHesap.AccruedInterest ?? 0);
                    TotalAmountText.Text = totalAmount.ToString("N2") + " TL";
                    
                    int remainingDays = Math.Max(0, (int)(vadeliHesap.MaturityDate.Value - DateTime.Now).TotalDays);
                    RemainingDaysText.Text = remainingDays.ToString() + " gün";
                    
                    // Vadeli hesap varsa buton metnini güncelle
                    VadeliButton.Content = "Vadeli Hesabı Boz";
                }
                else
                {
                    PrincipalAmountText.Text = "-";
                    AccruedInterestText.Text = "-";
                    InterestRateText.Text = "-";
                    MaturityDateText.Text = "-";
                    TotalAmountText.Text = "-";
                    RemainingDaysText.Text = "-";
                    
                    // Vadeli hesap yoksa buton metnini güncelle
                    VadeliButton.Content = "Vadeli Hesap Oluştur";
                }
            }
        }

        private void UpdateAccruedInterest(BankDbContext context, Accounts vadeliHesap)
        {
            if (vadeliHesap.StartDate == null || vadeliHesap.MaturityDate == null || vadeliHesap.InterestRate == null || vadeliHesap.PrincipalAmount == null)
                return;

            // Bugünden itibaren geçen gün sayısını hesapla
            int daysPassed = (int)(DateTime.Now - vadeliHesap.StartDate.Value).TotalDays;
            int totalDays = (int)(vadeliHesap.MaturityDate.Value - vadeliHesap.StartDate.Value).TotalDays;
            
            // Günlük faiz oranı (yıllık faiz / 365 gün)
            float dailyInterestRate = (float)vadeliHesap.InterestRate / 365f;
            
            // Biriken faizi hesapla
            float accruedInterest = (float)vadeliHesap.PrincipalAmount * dailyInterestRate * daysPassed;
            
            // Vade tarihi geçmişse maksimum faiz
            if (DateTime.Now >= vadeliHesap.MaturityDate)
            {
                accruedInterest = (float)vadeliHesap.PrincipalAmount * (float)vadeliHesap.InterestRate / 100f * (totalDays / 365f);
            }
            
            // Negatif faiz olmasın
            if (accruedInterest < 0) accruedInterest = 0;
            
            // Biriken faizi güncelle
            vadeliHesap.AccruedInterest = accruedInterest;
            
            // Balance alanını da güncelle (ana para + biriken faiz)
            vadeliHesap.Balance = (float)vadeliHesap.PrincipalAmount + accruedInterest;
            
            context.SaveChanges();
        }

        private void NewPrincipalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateVadeliHesap();
        }

        private void MaturityPeriodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateVadeliHesap();
        }

        private void CalculateVadeliHesap()
        {
            if (!decimal.TryParse(NewPrincipalTextBox.Text, out decimal principal) || principal <= 0)
            {
                ClearCalculationResults();
                return;
            }

            if (MaturityPeriodComboBox.SelectedItem == null)
            {
                ClearCalculationResults();
                return;
            }

            // Vade süresini al
            string maturityPeriodText = (MaturityPeriodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            int months = int.Parse(maturityPeriodText.Split(' ')[0]);
            DateTime maturityDate = DateTime.Now.AddMonths(months);
            
            // Faiz oranını belirle
            float interestRate = GetInterestRateByPeriod(months);

            // Hesaplama sonuçlarını göster
            CalculatedInterestRateText.Text = $"%{interestRate:N1}";
            CalculatedMaturityDateText.Text = maturityDate.ToString("dd.MM.yyyy");
            
            float totalInterest = CalculateTotalInterest((float)principal, interestRate, months);
            TotalInterestText.Text = $"{totalInterest:N2} TL";
            FinalAmountText.Text = $"{(float)principal + totalInterest:N2} TL";
        }

        private float GetInterestRateByPeriod(int months)
        {
            // Vade süresine göre faiz oranı
            return months switch
            {
                3 => 15.0f,   // %15 yıllık
                6 => 17.0f,   // %17 yıllık
                12 => 20.0f,  // %20 yıllık
                24 => 22.0f,  // %22 yıllık
                _ => 15.0f    // Varsayılan
            };
        }

        private float CalculateTotalInterest(float principal, float interestRate, int months)
        {
            // Basit faiz hesaplama (yıllık faiz oranı / 12 ay * ay sayısı)
            float annualInterest = principal * (interestRate / 100f);
            float totalInterest = annualInterest * (months / 12f);
            
            return totalInterest;
        }

        private void ClearCalculationResults()
        {
            CalculatedInterestRateText.Text = "-";
            CalculatedMaturityDateText.Text = "-";
            TotalInterestText.Text = "-";
            FinalAmountText.Text = "-";
        }

        private void VadeliButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new BankDbContext())
            {
                var vadeliHesap = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadeli" && a.IsBroken != true);
                
                if (vadeliHesap != null)
                {
                    // Vadeli hesap varsa, boz
                    BreakVadeliHesap(context, vadeliHesap);
                }
                else
                {
                    // Vadeli hesap yoksa, oluştur
                    CreateVadeliHesap(context);
                }
            }
        }

        private void CreateVadeliHesap(BankDbContext context)
        {
            if (!decimal.TryParse(NewPrincipalTextBox.Text, out decimal principal) || principal <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir ana para tutarı giriniz.");
                return;
            }

            if (MaturityPeriodComboBox.SelectedItem == null)
            {
                MessageBox.Show("Lütfen vade süresi seçiniz.");
                return;
            }

            // Mevcut vadeli hesap var mı kontrol et (sadece aktif olanları)
            var existingVadeli = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadeli" && a.IsBroken != true);
            if (existingVadeli != null)
            {
                MessageBox.Show("Zaten bir aktif vadeli hesabınız bulunmaktadır.\n\nÖnce mevcut vadeli hesabınızı bozmalısınız.");
                return;
            }

            // Vadesiz hesaptan para çek
            var vadesizHesap = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadesiz");
            if (vadesizHesap == null)
            {
                MessageBox.Show("Vadesiz hesabınız bulunamadı.");
                return;
            }

            if (vadesizHesap.Balance < (float)principal)
            {
                MessageBox.Show("Vadesiz hesabınızda yeterli bakiye bulunmamaktadır.");
                return;
            }

            try
            {
                // Vade süresini al
                string maturityPeriodText = (MaturityPeriodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                int months = int.Parse(maturityPeriodText.Split(' ')[0]);
                DateTime maturityDate = DateTime.Now.AddMonths(months);
                
                // Faiz oranını belirle
                float interestRate = GetInterestRateByPeriod(months);

                // Vadeli hesap oluştur
                var vadeliHesap = new Accounts
                {
                    AccountType = "Vadeli",
                    Balance = (float)principal, // Başlangıçta ana para
                    PrincipalAmount = (float)principal,
                    InterestRate = interestRate,
                    StartDate = DateTime.Now,
                    MaturityDate = maturityDate,
                    AccruedInterest = 0,
                    IsBroken = false,
                    CustomerID = customerId,
                    IBAN = GenerateIBAN()
                };

                // Önce vadeli hesabı ekle ve kaydet
                context.accounts.Add(vadeliHesap);
                context.SaveChanges(); // Bu satırı ekledik

                // Vadesiz hesaptan para çek
                vadesizHesap.Balance -= (float)principal;

                // İşlem kaydı oluştur (vadeli hesap ID'si artık mevcut)
                context.transactions.Add(new Transactions
                {
                    TransactionType = "Vadeli Hesap Açılışı",
                    TransactionDate = DateTime.Now,
                    Amount = (float)principal,
                    FromAccountID = vadesizHesap.AccountID,
                    ToAccountID = vadeliHesap.AccountID,
                    Description = $"Vadeli hesap açıldı. Faiz oranı: %{interestRate}, Vade: {maturityDate:dd.MM.yyyy}"
                });

                context.SaveChanges();

                MessageBox.Show("Vadeli hesap başarıyla oluşturuldu!");
                LoadVadeliHesapData();
                LoadVadesizBalance();
                
                // Formu temizle
                NewPrincipalTextBox.Text = "";
                MaturityPeriodComboBox.SelectedIndex = -1;
                ClearCalculationResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Vadeli hesap oluşturulurken hata oluştu: {ex.Message}\n\nLütfen tekrar deneyiniz.");
            }
        }

        private void BreakVadeliHesap(BankDbContext context, Accounts vadeliHesap)
        {
            try
            {
                // Vade tarihi gelmiş mi kontrol et
                if (DateTime.Now < vadeliHesap.MaturityDate)
                {
                    var result = MessageBox.Show("Vade tarihi henüz gelmemiş. Erken bozma yapmak istediğinizden emin misiniz?\n\nErken bozma durumunda faiz kaybı yaşanabilir.", 
                        "Erken Bozma Onayı", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    
                    if (result != MessageBoxResult.Yes)
                    {
                        return;
                    }
                }

                // Vadesiz hesabı bul
                var vadesizHesap = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadesiz");
                if (vadesizHesap == null)
                {
                    MessageBox.Show("Vadesiz hesabınız bulunamadı.");
                    return;
                }

                // Vadeli hesabı boz
                vadeliHesap.IsBroken = true;
                
                // Vade tarihi gelmişse tam faiz, gelmemişse kısmi faiz hesapla
                float totalAmount = vadeliHesap.PrincipalAmount ?? 0;
                if (DateTime.Now >= vadeliHesap.MaturityDate)
                {
                    // Vade gelmiş, tam faiz
                    totalAmount += vadeliHesap.AccruedInterest ?? 0;
                }
                else
                {
                    // Vade gelmemiş, kısmi faiz (basit hesaplama)
                    int remainingDays = (int)(vadeliHesap.MaturityDate.Value - DateTime.Now).TotalDays;
                    int totalDays = (int)(vadeliHesap.MaturityDate.Value - vadeliHesap.StartDate.Value).TotalDays;
                    float partialInterest = (vadeliHesap.AccruedInterest ?? 0) * (remainingDays / (float)totalDays);
                    totalAmount += partialInterest;
                }

                // Vadesiz hesaba para aktar
                vadesizHesap.Balance += totalAmount;

                // İşlem kaydı oluştur
                context.transactions.Add(new Transactions
                {
                    TransactionType = "Vadeli Hesap Bozma",
                    TransactionDate = DateTime.Now,
                    Amount = totalAmount,
                    FromAccountID = vadeliHesap.AccountID,
                    ToAccountID = vadesizHesap.AccountID,
                    Description = $"Vadeli hesap bozuldu. Ana para + faiz: {totalAmount:N2} TL"
                });

                context.SaveChanges();

                MessageBox.Show($"Vadeli hesap başarıyla bozuldu!\n\nVadesiz hesabınıza {totalAmount:N2} TL aktarıldı.");
                
                // Verileri yenile
                LoadVadeliHesapData();
                LoadVadesizBalance();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Vadeli hesap bozulurken hata oluştu: {ex.Message}\n\nLütfen tekrar deneyiniz.");
            }
        }

        private string GenerateIBAN()
        {
            // Benzersiz IBAN oluşturma
            Random random = new Random();
            string iban;
            bool isUnique = false;
            
            do
            {
                iban = "TR" + random.Next(100000000, 999999999).ToString();
                
                // Bu IBAN'ın daha önce kullanılıp kullanılmadığını kontrol et
                using (var context = new BankDbContext())
                {
                    isUnique = !context.accounts.Any(a => a.IBAN == iban);
                }
            } while (!isUnique);
            
            return iban;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadVadeliHesapData();
            LoadVadesizBalance();
        }
    }
} 