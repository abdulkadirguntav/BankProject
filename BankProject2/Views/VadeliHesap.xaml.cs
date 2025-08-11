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
            
            // Faiz oranını vade süresine göre belirle
            float interestRate = GetInterestRateByPeriod(months);
            
            // Vade tarihini hesapla
            DateTime maturityDate = DateTime.Now.AddMonths(months);
            
            // Faiz hesapla
            float totalInterest = CalculateTotalInterest((float)principal, interestRate, months);
            float finalAmount = (float)principal + totalInterest;
            
            // Sonuçları göster
            CalculatedInterestRateText.Text = interestRate.ToString("N1") + "%";
            CalculatedMaturityDateText.Text = maturityDate.ToString("dd.MM.yyyy");
            TotalInterestText.Text = totalInterest.ToString("N2") + " TL";
            FinalAmountText.Text = finalAmount.ToString("N2") + " TL";
        }

        private float GetInterestRateByPeriod(int months)
        {
            // Vade süresine göre faiz oranı belirle
            switch (months)
            {
                case 3: return 8.5f;  // 3 ay için %8.5
                case 6: return 9.0f;  // 6 ay için %9.0
                case 12: return 9.5f; // 12 ay için %9.5
                case 24: return 10.0f; // 24 ay için %10.0
                default: return 9.0f;
            }
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
                // Mevcut vadeli hesap var mı kontrol et
                var existingVadeli = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadeli" && (a.IsBroken == false || a.IsBroken == null));
                if (existingVadeli != null)
                {
                    MessageBox.Show("Zaten bir vadeli hesabınız bulunmaktadır.");
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

                // Önce vadeli hesabı ekle ve ID oluşturulsun
                context.accounts.Add(vadeliHesap);
                context.SaveChanges();

                // İşlem kaydı oluştur (artık ToAccountID mevcut)
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
                
                // Formu temizle
                NewPrincipalTextBox.Text = "";
                MaturityPeriodComboBox.SelectedIndex = -1;
                ClearCalculationResults();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadVadeliHesapData();
        }

        private string GenerateIBAN()
        {
            // Basit IBAN oluşturma (gerçek uygulamada daha karmaşık olmalı)
            Random random = new Random();
            return "TR" + random.Next(100000000, 999999999).ToString();
        }
    }
} 