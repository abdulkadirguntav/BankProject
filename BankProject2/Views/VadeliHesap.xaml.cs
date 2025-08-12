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
                var vadeliHesap = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadeli");
                
                if (vadeliHesap != null)
                {
                    PrincipalAmountText.Text = vadeliHesap.PrincipalAmount?.ToString("N2") + " TL" ?? "-";
                    AccruedInterestText.Text = vadeliHesap.AccruedInterest?.ToString("N2") + " TL" ?? "-";
                    InterestRateText.Text = vadeliHesap.InterestRate?.ToString("N1") + "%" ?? "-";
                    MaturityDateText.Text = vadeliHesap.MaturityDate?.ToString("dd.MM.yyyy") ?? "-";
                }
                else
                {
                    PrincipalAmountText.Text = "-";
                    AccruedInterestText.Text = "-";
                    InterestRateText.Text = "-";
                    MaturityDateText.Text = "-";
                }
            }
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
            // Basit faiz hesaplama
            float annualInterest = principal * (interestRate / 100f);
            float monthlyInterest = annualInterest / 12f;
            float totalInterest = monthlyInterest * months;
            
            return totalInterest;
        }

        private void ClearCalculationResults()
        {
            CalculatedInterestRateText.Text = "-";
            CalculatedMaturityDateText.Text = "-";
            TotalInterestText.Text = "-";
            FinalAmountText.Text = "-";
        }

        private void CreateVadeliButton_Click(object sender, RoutedEventArgs e)
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

            using (var context = new BankDbContext())
            {
                // Mevcut vadeli hesap var mı kontrol et (sadece aktif olanları)
                var existingVadeli = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadeli" && a.IsBroken != true);
                if (existingVadeli != null)
                {
                    MessageBox.Show("Zaten bir aktif vadeli hesabınız bulunmaktadır.");
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
                    Balance = (float)principal,
                    PrincipalAmount = (float)principal,
                    InterestRate = interestRate,
                    StartDate = DateTime.Now,
                    MaturityDate = maturityDate,
                    AccruedInterest = 0,
                    IsBroken = false,
                    CustomerID = customerId,
                    IBAN = GenerateIBAN()
                };

                // Vadesiz hesaptan para çek
                vadesizHesap.Balance -= (float)principal;

                // İşlem kaydı oluştur
                context.transactions.Add(new Transactions
                {
                    TransactionType = "Vadeli Hesap Açılışı",
                    TransactionDate = DateTime.Now,
                    Amount = (float)principal,
                    FromAccountID = vadesizHesap.AccountID,
                    ToAccountID = vadeliHesap.AccountID,
                    Description = $"Vadeli hesap açıldı. Faiz oranı: %{interestRate}, Vade: {maturityDate:dd.MM.yyyy}"
                });

                context.accounts.Add(vadeliHesap);
                context.SaveChanges();

                MessageBox.Show("Vadeli hesap başarıyla oluşturuldu!");
                LoadVadeliHesapData();
                LoadVadesizBalance();
                
                // Formu temizle
                NewPrincipalTextBox.Text = "";
                MaturityPeriodComboBox.SelectedIndex = -1;
                ClearCalculationResults();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadVadeliHesapData();
            LoadVadesizBalance();
        }

        private string GenerateIBAN()
        {
            // Basit IBAN oluşturma (gerçek uygulamada daha karmaşık olmalı)
            Random random = new Random();
            return "TR" + random.Next(100000000, 999999999).ToString();
        }
    }
} 