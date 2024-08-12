-Proje Özeti

Bu proje, .NET Core kullanılarak geliştirilen bir RESTful API uygulamasıdır. Projenin temel amacı, dijital ürün satışlarını yönetmek ve kullanıcıların sadakat puanları ile alışveriş yapabilmelerini sağlamaktır. Proje, SOLID prensiplerine uygun olarak tasarlanmış ve katmanlı mimari yapısı benimsenmiştir.

-Ana Özellikler

Kullanıcı Rolleri: Admin ve normal kullanıcı rolleri bulunur.
Ürün Yönetimi: Ürün ekleme, güncelleme, silme ve listeleme işlemleri.
Sadakat Puanları: Kullanıcıların alışverişlerden kazandığı puanlar.
Kupon Sistemi: Kupon kodları kullanılarak indirim sağlama.
Sipariş Yönetimi: Sipariş oluşturma, güncelleme ve listeleme.
JWT Tabanlı Kimlik Doğrulama: Kullanıcı kimlik doğrulaması için JWT kullanımı.
Swagger Desteği: API dokümantasyonu ve test için Swagger entegrasyonu.
Validation: FluentValidation kütüphanesi ile giriş doğrulamaları.
Kurulum ve Çalıştırma
Projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları izleyebilirsiniz:

-Gereksinimler
.NET 8.0 SDK
SQL Server

-Kurulum Adımları

1-Proje Kaynağını Klonlayın:

git clone https://github.com/furkngrsy/Papara-.Net-Bootcamp-FinalCase.git

2-Veritabanı Bağlantısını Ayarlayın:

appsettings.json dosyasındaki MsSqlConnection bağlantı dizesini yerel SQL Server ayarlarınıza göre güncelleyin.

3-Veritabanını Güncelleyin:

dotnet ef database update

4-Projeyi Çalıştırın:

dotnet run

5-Swagger UI'yi Kullanın:

Tarayıcınızdan https://localhost:5001/swagger/index.html adresine giderek API'yi keşfedin.

-Proje Yapısı

Proje, aşağıdaki gibi bir katmanlı mimariyi takip etmektedir:

Papara_Final_Project.Api: API katmanı, Controller sınıflarını içerir.
Papara_Final_Project.Services: İş mantığı katmanı, Service sınıflarını içerir.
Papara_Final_Project.Repositories: Veri erişim katmanı, Repository sınıflarını içerir.
Papara_Final_Project.Models: Veri modellerini içerir.
Papara_Final_Project.DTOs: Veri transfer objelerini içerir.
Papara_Final_Project.Validations: FluentValidation kullanılarak doğrulama kurallarını içerir.
Papara_Final_Project.UnitOfWorks: Unit of Work deseninin implementasyonunu içerir.
