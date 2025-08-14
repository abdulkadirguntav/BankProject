# VakıfBank Dijital Bankacılık Projesi

Bu proje, WPF (Windows Presentation Foundation) kullanılarak geliştirilmiş bir dijital bankacılık uygulamasıdır. MySQL veritabanı kullanılarak müşteri hesapları, kredi kartları, döviz işlemleri ve daha fazlası yönetilmektedir.

## 🚀 Özellikler

### Ana Özellikler
- **Kullanıcı Girişi**: Telefon numarası ve şifre ile güvenli giriş
- **Yeni Hesap Açma**: Kapsamlı kayıt formu ile yeni müşteri hesabı oluşturma
- **Vadesiz Hesap**: Otomatik vadesiz hesap açılımı
- **Kredi Kartı**: İsteğe bağlı kredi kartı başvurusu
- **Döviz İşlemleri**: Güncel kurlar ve alım-satım işlemleri
- **Havale/EFT**: Hesaplar arası para transferi
- **Vadeli Hesap**: Faizli vadeli hesap seçenekleri

### Yeni Eklenen Özellikler
- **📝 Kayıt Ol Sekmesi**: Ana menüye eklenen kayıt sekmesi
- **Kapsamlı Kayıt Formu**: Ad, soyad, telefon, şifre, aylık gelir bilgileri
- **Otomatik Hesap Açılımı**: Kayıt sonrası otomatik vadesiz hesap oluşturma
- **Kredi Kartı Seçeneği**: Kayıt sırasında kredi kartı isteği
- **Akıllı Risk Skorlama**: Aylık gelire göre otomatik risk skoru hesaplama

## 🛠️ Teknolojiler

- **Frontend**: WPF (Windows Presentation Foundation)
- **Backend**: C# .NET 8.0
- **Veritabanı**: MySQL
- **ORM**: Entity Framework 6
- **Test Framework**: MSTest
- **UI Framework**: XAML

## 📋 Gereksinimler

- Windows 10/11
- .NET 8.0 SDK
- MySQL Server 8.0+
- Visual Studio 2022 veya Visual Studio Code

## 🚀 Kurulum

### 1. Projeyi Klonlayın
```bash
git clone [repository-url]
cd BankProject2
```

### 2. Veritabanını Kurun
MySQL'de aşağıdaki komutları çalıştırın:

```sql
CREATE DATABASE Bank;
USE Bank;

-- Müşteri tablosu
CREATE TABLE Customer (
    CustomerID INT NOT NULL AUTO_INCREMENT,
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    PhoneNumber VARCHAR(20) UNIQUE,
    CustomerPassword VARCHAR(255) NOT NULL,
    MonthlyIncome DOUBLE DEFAULT 0,
    PRIMARY KEY (CustomerID)
);

-- Döviz tablosu
CREATE TABLE Currency (
    CurrencyID INT NOT NULL AUTO_INCREMENT,
    CurrencyCode VARCHAR(10) NOT NULL,
    CurrencyDate DATE,
    RateToTRY DOUBLE,
    PRIMARY KEY (CurrencyID)
);

-- Hesaplar tablosu
CREATE TABLE Accounts (
    AccountID INT NOT NULL AUTO_INCREMENT,
    IBAN VARCHAR(34) UNIQUE,
    AccountType VARCHAR(50) NOT NULL,
    Balance DOUBLE DEFAULT 0,
    StartDate DATE DEFAULT NULL,
    MaturityDate DATE DEFAULT NULL,
    InterestRate DOUBLE DEFAULT NULL,
    IsBroken BOOLEAN DEFAULT FALSE,
    PrincipalAmount DOUBLE DEFAULT NULL,
    AccruedInterest DOUBLE DEFAULT NULL,
    CurrencyID INT,
    CustomerID INT,
    PRIMARY KEY (AccountID),
    FOREIGN KEY (CurrencyID) REFERENCES Currency(CurrencyID),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);

-- Kredi kartı tablosu
CREATE TABLE CreditCard (
    CreditCardID INT NOT NULL AUTO_INCREMENT,
    CustomerID INT,
    CardNumber VARCHAR(20) UNIQUE,
    CardExpiry DATE,
    CardCVV VARCHAR(4),
    `Limit` DOUBLE DEFAULT 0,
    CurrentDebt DOUBLE DEFAULT 0,
    RiskScore INT DEFAULT 0,
    LatePaymentCount INT DEFAULT 0,
    PRIMARY KEY (CreditCardID),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);

-- İşlemler tablosu
CREATE TABLE Transactions (
    TransactionID INT NOT NULL AUTO_INCREMENT,
    FromAccountID INT,
    ToAccountID INT,
    CreditCardID INT,
    Amount DOUBLE,
    TransactionType VARCHAR(50),
    TransactionDate DATE,
    Fee DOUBLE,
    Description VARCHAR(255),
    PRIMARY KEY (TransactionID),
    FOREIGN KEY (FromAccountID) REFERENCES Accounts(AccountID),
    FOREIGN KEY (ToAccountID) REFERENCES Accounts(AccountID),
    FOREIGN KEY (CreditCardID) REFERENCES CreditCard(CreditCardID)
);

-- Kredi tablosu
CREATE TABLE Loan (
    LoanID INT NOT NULL AUTO_INCREMENT,
    LoanStatus VARCHAR(50) NOT NULL,
    InterestRate DOUBLE,
    Principal DOUBLE,
    TermMonths INT,
    CustomerID INT,
    PRIMARY KEY (LoanID),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);

-- Kredi ödemeleri tablosu
CREATE TABLE LoanPayment (
    LoanPaymentID INT NOT NULL AUTO_INCREMENT,
    Amount DOUBLE,
    PaidDate DATE,
    LoanID INT,
    PRIMARY KEY (LoanPaymentID),
    FOREIGN KEY (LoanID) REFERENCES Loan(LoanID)
);
```

### 3. Veritabanı Bağlantısını Yapılandırın
`BankProject2/Data/BankDbContext.cs` dosyasında connection string'i güncelleyin:

```csharp
public class BankDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySQL("Server=localhost;Database=Bank;Uid=your_username;Pwd=your_password;");
    }
}
```

### 4. Projeyi Derleyin
```bash
dotnet build
```

## 🧪 Test'leri Çalıştırma

### Test'leri Çalıştırın
```bash
cd BankProject2.Tests
dotnet test
```

### Test Kapsamı
Proje aşağıdaki alanları kapsayan kapsamlı test'lere sahiptir:

#### Model Test'leri
- **Customer**: Müşteri oluşturma ve özellik doğrulama
- **Accounts**: Hesap türleri ve özellikler
- **CreditCard**: Kredi kartı oluşturma ve risk skorlama
- **Currency**: Döviz kurları ve validasyon
- **Loan**: Kredi işlemleri
- **Transactions**: Para transferi ve işlem geçmişi

#### Business Logic Test'leri
- **Risk Skorlama**: Aylık gelire göre otomatik risk hesaplama
- **IBAN Üretimi**: Türkiye standartlarında IBAN oluşturma
- **Kart Numarası Üretimi**: Visa formatında kart numarası
- **CVV Üretimi**: Güvenli CVV kodu

#### Validation Test'leri
- **Telefon Numarası**: Format ve uzunluk kontrolü
- **Şifre**: Minimum uzunluk ve karakter kontrolü
- **Zorunlu Alanlar**: Boş değer kontrolü

#### Integration Test'leri
- **Kayıt İşlemi**: Müşteri ve hesap oluşturma
- **Giriş İşlemi**: Kimlik doğrulama
- **Kredi Kartı Başvurusu**: Otomatik kart oluşturma

## 🎯 Kullanım

### 1. Uygulamayı Başlatın
```bash
cd BankProject2
dotnet run
```

### 2. Yeni Hesap Açın
- Ana menüden "📝 Kayıt Ol" sekmesine tıklayın
- Gerekli bilgileri doldurun:
  - Ad ve Soyad
  - Telefon numarası
  - Şifre (en az 6 karakter)
  - Aylık gelir
  - Kredi kartı isteği (opsiyonel)
- "Hesap Aç" butonuna tıklayın

### 3. Giriş Yapın
- Ana menüden telefon numaranız ve şifrenizi girin
- "Giriş Yap" butonuna tıklayın

### 4. Bankacılık İşlemlerini Kullanın
- **🏦 Banka**: Hesap bilgileri ve işlemler
- **💳 Kredi Kart**: Kart limiti ve borç durumu
- **💱 Döviz**: Güncel kurlar
- **📈 Güncel**: Piyasa bilgileri
- **🔄 Al - Sat**: Döviz alım-satım

## 🔒 Güvenlik Özellikleri

- **Şifre Validasyonu**: Minimum 6 karakter zorunluluğu
- **Telefon Numarası Kontrolü**: Benzersiz telefon numarası
- **Risk Skorlama**: Gelir bazlı otomatik risk değerlendirmesi
- **Güvenli Veri Saklama**: Entity Framework ile güvenli veri işleme

## 📊 Veritabanı Şeması

Proje aşağıdaki ana tabloları içerir:

- **Customer**: Müşteri bilgileri
- **Accounts**: Hesap detayları (Vadesiz, Vadeli, Döviz)
- **CreditCard**: Kredi kartı bilgileri
- **Currency**: Döviz kurları
- **Transactions**: İşlem geçmişi
- **Loan**: Kredi bilgileri
- **LoanPayment**: Kredi ödemeleri

## 🐛 Bilinen Sorunlar

- Test'ler çalıştırılırken veritabanı bağlantısı gerekli
- MySQL servisinin çalışır durumda olması gerekiyor

## 🤝 Katkıda Bulunma

1. Projeyi fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'inizi push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## 📞 İletişim

Proje hakkında sorularınız için:
- GitHub Issues kullanın
- Pull Request gönderin

## 🔄 Güncelleme Geçmişi

### v2.0.0 (Güncel)
- ✅ Kayıt ol sekmesi eklendi
- ✅ Otomatik hesap açılımı
- ✅ Kredi kartı başvuru seçeneği
- ✅ Kapsamlı Unit Test'ler
- ✅ Risk skorlama sistemi
- ✅ IBAN ve kart numarası üretimi

### v1.0.0
- ✅ Temel bankacılık işlemleri
- ✅ Müşteri girişi
- ✅ Hesap yönetimi
- ✅ Döviz işlemleri
