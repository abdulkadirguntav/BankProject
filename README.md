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

