# VakıfBank Dijital Bankacılık Projesi

Bu proje, WPF (Windows Presentation Foundation) kullanılarak geliştirilmiş kapsamlı bir dijital bankacılık uygulamasıdır. MySQL veritabanı ile entegre çalışarak modern bankacılık işlemlerini simüle eder.

## 🚀 Ana Özellikler

### Müşteri İşlemleri
- **Güvenli Giriş**: Telefon numarası ve şifre ile kimlik doğrulama
- **Yeni Hesap Açma**: Kapsamlı kayıt formu ile müşteri hesabı oluşturma
- **Otomatik Vadesiz Hesap**: Kayıt sonrası otomatik hesap açılımı

### Bankacılık Hizmetleri
- **Kredi Kartı Yönetimi**: Limit ayarlama, borç takibi ve risk skorlama
- **Döviz İşlemleri**: Güncel kurlar ve alım-satım işlemleri
- **Para Transferi**: Hesaplar arası havale/EFT işlemleri
- **Vadeli Hesap**: Faizli vadeli hesap seçenekleri
- **Kredi İşlemleri**: Kredi başvurusu ve ödeme takibi

### Akıllı Özellikler
- **Risk Skorlama**: Aylık gelire göre otomatik risk değerlendirmesi
- **IBAN Üretimi**: Türkiye standartlarında otomatik IBAN oluşturma
- **Kart Numarası Üretimi**: Visa formatında güvenli kart numarası
- **Otomatik Validasyon**: Telefon numarası ve şifre format kontrolü

## 🛠️ Teknolojiler

- **Frontend**: WPF (Windows Presentation Foundation) - XAML
- **Backend**: C# .NET 8.0
- **Veritabanı**: MySQL
- **ORM**: Entity Framework 6
- **Test Framework**: MSTest
- **Platform**: Windows 10/11

## 📋 Sistem Gereksinimleri

- Windows 10/11 işletim sistemi
- .NET 8.0 SDK
- MySQL Server 8.0+
- Visual Studio 2022 veya Visual Studio Code

## 🎯 Kullanım Senaryoları

### Yeni Müşteri Kaydı
1. Ana menüden "Kayıt Ol" sekmesine tıklayın
2. Kişisel bilgilerinizi girin (ad, soyad, telefon, şifre)
3. Aylık gelir bilginizi belirtin
4. İsteğe bağlı kredi kartı başvurusu yapın
5. Sistem otomatik olarak vadesiz hesap açacaktır

### Mevcut Müşteri Girişi
1. Telefon numaranız ve şifrenizle giriş yapın
2. Ana bankacılık menüsüne erişin
3. İstediğiniz hizmeti seçin

### Bankacılık İşlemleri
- **Hesap Bilgileri**: Bakiye sorgulama ve işlem geçmişi
- **Kredi Kartı**: Limit değiştirme ve borç ödeme
- **Döviz İşlemleri**: Güncel kurları görme ve alım-satım
- **Para Transferi**: Hesaplar arası transfer işlemleri

## 🔒 Güvenlik Özellikleri

- **Şifre Güvenliği**: Minimum 6 karakter zorunluluğu
- **Benzersiz Kimlik**: Telefon numarası ile benzersiz müşteri tanımlama
- **Risk Değerlendirmesi**: Gelir bazlı otomatik risk skorlama
- **Güvenli Veri İşleme**: Entity Framework ile güvenli veritabanı işlemleri

## 📊 Veritabanı Yapısı

Proje aşağıdaki ana veri tablolarını kullanır:
- **Müşteri Bilgileri**: Kişisel ve finansal bilgiler
- **Hesap Detayları**: Vadesiz, vadeli ve döviz hesapları
- **Kredi Kartı Bilgileri**: Kart detayları ve borç durumu
- **Döviz Kurları**: Güncel döviz bilgileri
- **İşlem Geçmişi**: Tüm para transferleri ve işlemler
- **Kredi Bilgileri**: Kredi başvuruları ve ödemeler

## 🧪 Test Kapsamı

Proje kapsamlı unit test'ler içerir:
- **Model Test'leri**: Veri modellerinin doğruluğu
- **İş Mantığı Test'leri**: Risk skorlama ve hesaplama algoritmaları
- **Validasyon Test'leri**: Giriş verilerinin kontrolü
- **Entegrasyon Test'leri**: Sistem bileşenlerinin birlikte çalışması
