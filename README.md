# Bank Project 2

Bu proje, WPF teknolojisi kullanılarak geliştirilmiş bir bankacılık uygulamasıdır. SQLite veritabanı kullanarak dosya tabanlı veri saklama yapar.
NOT: Projeyi ilk yaptığım zaman mentorumun söylemesi üzerine MySQL kullanmıştım. 
fakat bu projeyi inceleyeceklerde benim kendi tasarladığım veritabanı olmayacağı için stajın son gününde veritabanını SQLite'a çevirmek zorunda kaldım.

## Özellikler

- Müşteri girişi ve kimlik doğrulama
- Vadesiz hesap yönetimi
- Vadeli hesap oluşturma ve yönetimi
- Döviz alım-satım işlemleri
- Kredi kartı yönetimi
- Para transferi işlemleri
- Güncel döviz kurları takibi

## Gereksinimler

- .NET 8.0 SDK
- Visual Studio 2022 veya Visual Studio Code
- Windows 10/11

## Kurulum ve Çalıştırma

### 1. Projeyi İndirin
```bash
git clone https://github.com/kullaniciadi/BankProject2.git
cd BankProject2
```

### 2. Projeyi Açın
- Visual Studio 2022 ile `BankProject2.sln` dosyasını açın
- Veya Visual Studio Code ile proje klasörünü açın

### 3. Bağımlılıkları Yükleyin
```bash
dotnet restore
```

### 4. Projeyi Çalıştırın
```bash
dotnet run --project ./BankProject2
```

## Demo Hesap Bilgileri

Uygulama ilk çalıştırıldığında otomatik olarak iki demo hesap oluşturulur:

### Müşteri 1 - Ahmet Yılmaz
- **Telefon Numarası:** 5551234567
- **Şifre:** 123456
- **Vadesiz Hesap:** 5.000 TL
- **Vadeli Hesap:** 10.000 TL (60 gün vadeli)
- **Kredi Kartı:** 15.000 TL limit, 2.500 TL borç

### Müşteri 2 - Ayşe Demir
- **Telefon Numarası:** 5559876543
- **Şifre:** 123456
- **Vadesiz Hesap:** 3.000 TL
- **Vadeli Hesap:** 8.000 TL (45 gün vadeli)
- **Kredi Kartı:** 12.000 TL limit, 1.800 TL borç

## Test Senaryoları

### Para Transferi Testi
1. Ahmet hesabına giriş yapın
2. Transfer sayfasına gidin
3. Ayşe'nin IBAN'ını kullanarak transfer yapın
4. Ayşe hesabına giriş yaparak transferi kontrol edin

### Kredi Kartı Borç Ödeme
1. Ahmet hesabına giriş yapın
2. Kredi kartı sayfasına gidin
3. Borç ödeme işlemi yapın
4. Kalan borç miktarını kontrol edin

### Döviz Alım-Satım
1. Herhangi bir hesaba giriş yapın
2. Döviz sayfasına gidin
3. USD, EUR, GBP alım-satım işlemleri yapın
4. Hesap bakiyesindeki değişimi kontrol edin

### Vadeli Hesap İşlemleri
1. Vadeli hesap sayfasına gidin
2. Yeni vadeli hesap oluşturun
3. Faiz hesaplamalarını kontrol edin

## Veritabanı

- Proje SQLite veritabanı kullanır
- Veritabanı dosyası (`BankDatabase.db`) uygulama klasöründe otomatik oluşturulur
- İlk çalıştırmada örnek veriler otomatik olarak eklenir

## Proje Yapısı

```
BankProject2/
├── BankProject2/           # Ana uygulama
│   ├── Data/              # Veritabanı bağlantısı
│   ├── Models/            # Veri modelleri
│   ├── Views/             # WPF sayfaları
│   └── MainWindow.xaml    # Ana pencere
├── BankProject2.Tests2/   # Test projesi
└── README.md
```

## Test Etme

Testleri çalıştırmak için:
```bash
dotnet test
```
