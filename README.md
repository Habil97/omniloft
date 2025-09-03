# Omniloft E-Ticaret Projesi

Bu proje, ASP.NET Core MVC ve SQL Server kullanılarak geliştirilmiş, modüler ve ölçeklenebilir bir e-ticaret web uygulamasıdır. Kod yapısı OOP prensiplerine uygun ve kolayca özelleştirilebilir şekilde tasarlanmıştır. Proje, hem son kullanıcı hem de yönetici (admin) paneli içerir.

## Özellikler

- **Kullanıcı Paneli:**  
  - Ürünleri görüntüleme, sepete ekleme, favorilere ekleme  
  - Sipariş oluşturma ve ödeme işlemleri  
  - Sipariş takibi, iade/değişim talepleri  
  - Kullanıcı profili ve favori ürünler yönetimi

- **Admin Paneli:**  
  - Ürün, kategori, kullanıcı, rol, slider, kampanya yönetimi  
  - Sipariş ve iade taleplerini görüntüleme  
  - E-bülten aboneliği ve log yönetimi

- **Teknik Yapı:**  
  - ASP.NET Core MVC  
  - SQL Server  
  - Entity Framework Core  
  - Serilog ile loglama  
  - Bootstrap ile responsive tasarım  
  - Session ve Cookie tabanlı kimlik doğrulama  
  - OOP ve modüler mimari

## Klasör Yapısı

- **Entities:** Veri tabanı modelleri
- **DataAccess:** Repository ve DbContext dosyaları
- **Services:** İş mantığı ve servis katmanı
- **Controllers:** MVC controller dosyaları
- **Views:** Razor view dosyaları (kullanıcı ve admin arayüzleri)
- **wwwroot:** Statik dosyalar (CSS, JS, görseller)
- **Migrations:** EF Core migration dosyaları
- **AdminPanel:** Yönetici paneli view dosyaları

## Kurulum

1. **Veritabanı Ayarı:**  
   - `appsettings.json` dosyasındaki `DefaultConnection` kısmını kendi SQL Server bağlantınıza göre düzenleyin.

2. **NuGet Paketleri:**  
   - Gerekli NuGet paketlerini yükleyin (`dotnet restore` veya Visual Studio ile).

3. **Migration ve Veritabanı:**  
   - Migration’ları uygulayın:  
     ```
     dotnet ef database update
     ```

4. **Projeyi Başlatın:**  
   - Visual Studio veya terminalden:  
     ```
     dotnet run
     ```

5. **Giriş:**  
   - Varsayılan admin ve kullanıcı hesapları migration ile oluşturulmuş olabilir. Gerekirse veritabanından ekleyin.

## Katkı ve Geliştirme

- Kodlar modüler ve kolayca değiştirilebilir yapıdadır.
- Yeni özellik eklemek için ilgili Entity, Repository, Service ve Controller dosyalarını oluşturabilirsiniz.
- Pull request ve issue açarak katkı sağlayabilirsiniz.

## Lisans

Bu proje MIT lisansı ile yayınlanmıştır.

---

**Omniloft E-Ticaret Projesi**  
Modern, güvenli ve ölçeklenebilir bir alışveriş deneyimi
