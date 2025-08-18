# Bank Project 2

Bu proje, WPF teknolojisi kullanılarak geliştirilmiş bir bankacılık uygulamasıdır. SQLite veritabanı kullanarak dosya tabanlı veri saklama yapar.

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
dotnet run --project BankProject2
```

## Demo Hesap Bilgileri

Uygulama ilk çalıştırıldığında otomatik olarak bir demo hesap oluşturulur:

- **Telefon Numarası:** 5551234567
- **Şifre:** 123456

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

## Katkıda Bulunma

1. Bu repository'yi fork edin
2. Yeni bir branch oluşturun (`git checkout -b feature/yeni-ozellik`)
3. Değişikliklerinizi commit edin (`git commit -am 'Yeni özellik eklendi'`)
4. Branch'inizi push edin (`git push origin feature/yeni-ozellik`)
5. Pull Request oluşturun

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## İletişim

Sorularınız için issue açabilir veya pull request gönderebilirsiniz.
