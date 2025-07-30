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

        private void CreateVadeliButton_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(NewPrincipalTextBox.Text, out decimal principal) || principal <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir ana para tutarı giriniz.");
                return;
            }

            if (InterestRateComboBox.SelectedItem == null)
            {
                MessageBox.Show("Lütfen faiz oranı seçiniz.");
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
                var existingVadeli = context.accounts.FirstOrDefault(a => a.CustomerID == customerId && a.AccountType == "Vadeli");
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

                // Faiz oranını al
                string interestRateText = (InterestRateComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                float interestRate = float.Parse(interestRateText.Replace("%", ""));

                // Vade süresini al
                string maturityPeriodText = (MaturityPeriodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                int months = int.Parse(maturityPeriodText.Split(' ')[0]);
                DateTime maturityDate = DateTime.Now.AddMonths(months);

                // Vadeli hesap oluştur
                var vadeliHesap = new Accounts
                {
                    AccountType = "Vadeli",
                    Balance = (float)principal,
                    PrincipalAmount = (float)principal,
                    InterestRate = interestRate,
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
                
                // Formu temizle
                NewPrincipalTextBox.Text = "";
                InterestRateComboBox.SelectedIndex = -1;
                MaturityPeriodComboBox.SelectedIndex = -1;
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