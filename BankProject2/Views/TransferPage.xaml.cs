using System.Windows.Controls;
using System.Windows;
using BankProject2.Data;
using BankProject2.Models;
using System.Linq;
using System;

namespace BankProject2
{
    public partial class TransferPage : UserControl
    {
        private int customerId;
        
        public TransferPage(int customerId)
        {
            InitializeComponent();
            this.customerId = customerId;
            LoadBalances();
        }

        private void LoadBalances()
        {
            using (var context = new BankDbContext())
            {
                // Vadeli hesap faiz hesaplama
                ProcessVadeliInterest(context);
                
                var vadesiz = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadesiz");
                var vadeli = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadeli");
                VadesizBalanceText.Text = vadesiz != null ? vadesiz.Balance.ToString("N2") + " TL" : "-";
                VadeliBalanceText.Text = vadeli != null ? vadeli.Balance.ToString("N2") + " TL" : "-";
            }
        }

        private void ProcessVadeliInterest(BankDbContext context)
        {
            var vadeliHesaplar = context.accounts
                .Where(a => a.CustomerID == customerId && a.AccountType == "Vadeli" && !a.IsBroken.Value)
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
                        var vadesizHesap = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadesiz");
                        
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

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string iban = IbanTextBox.Text.Trim();
            string amountText = AmountTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            if (string.IsNullOrEmpty(iban) || string.IsNullOrEmpty(amountText) || !decimal.TryParse(amountText, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir IBAN ve tutar giriniz.");
                return;
            }

            using (var context = new BankDbContext())
            {
                // Hangi sekme seçili?
                bool isVadesiz = AccountTabControl.SelectedIndex == 0;
                var senderAccount = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == (isVadesiz ? "Vadesiz" : "Vadeli"));
                if (senderAccount == null)
                {
                    MessageBox.Show("Seçili hesap bulunamadı.");
                    return;
                }
                if (senderAccount.Balance < (float)amount)
                {
                    MessageBox.Show("Hesap bakiyesi yetersiz.");
                    return;
                }
                // Alıcı hesabı bul
                var receiverAccount = context.accounts.FirstOrDefault(a => a.IBAN == iban);
                if (receiverAccount == null)
                {
                    MessageBox.Show("Alıcı IBAN bulunamadı.");
                    return;
                }
                // Vadeli hesap ise vade kontrolü
                if (!isVadesiz)
                {
                    if (senderAccount.MaturityDate.HasValue && senderAccount.MaturityDate.Value > DateTime.Now)
                    {
                        var result = MessageBox.Show($"Vade bitiş tarihi: {senderAccount.MaturityDate:dd.MM.yyyy}\nFaiziniz yanacak, vadeyi bozmak istiyor musunuz?", "Vadeli Hesap Uyarı", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result != MessageBoxResult.Yes)
                            return;
                        // Faiz silinir, ana para vadesize aktarılır
                        var vadesiz = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadesiz");
                        if (vadesiz == null)
                        {
                            MessageBox.Show("Vadesiz hesabınız bulunamadı, işlem iptal edildi.");
                            return;
                        }
                        vadesiz.Balance += senderAccount.PrincipalAmount ?? 0;
                        senderAccount.Balance = 0;
                        senderAccount.PrincipalAmount = 0;
                        senderAccount.AccruedInterest = 0;
                        senderAccount.IsBroken = true;
                        context.transactions.Add(new Transactions
                        {
                            TransactionType = "Vadeli Bozma",
                            TransactionDate = DateTime.Now,
                            Amount = senderAccount.Balance,
                            FromAccountID = senderAccount.AccountID,
                            ToAccountID = vadesiz.AccountID,
                            Description = "Vadeli hesap bozuldu, ana para vadesize aktarıldı."
                        });
                        context.SaveChanges();
                        senderAccount = vadesiz; // Transfer artık vadesizden devam edecek
                    }
                }
                // Transfer işlemi
                senderAccount.Balance -= (float)amount;
                receiverAccount.Balance += (float)amount;
                context.transactions.Add(new Transactions
                {
                    TransactionType = "Transfer",
                    TransactionDate = DateTime.Now,
                    Amount = (float)amount,
                    FromAccountID = senderAccount.AccountID,
                    ToAccountID = receiverAccount.AccountID,
                    Description = description
                });
                context.SaveChanges();
                MessageBox.Show("Transfer başarılı.");
                
                // Transfer sonrası bakiyeleri yenile
                LoadBalances();
            }
        }
    }
} 