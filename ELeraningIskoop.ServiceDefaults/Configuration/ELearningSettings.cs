using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELeraningIskoop.ServiceDefaults.Configuration
{
    public class ELearningSettings
    {
        public const string SectionName = "ELearningIskoop";

        //Dosya upload ayarları
        public FileUploadSettings FileUpload { get; set; } = new();


        // Email ayarları
        public EmailSettings Email { get; set; } = new();

        // Cache ayarları
        public CacheSettings Cache { get; set; } = new();

        // Security ayarları
        public SecuritySettings Security { get; set; } = new();
    }

    // Dosya upload ayarları
    public class FileUploadSettings
    {
        // Maksimum dosya boyutu(MB)
        public int MaxFileSizeMB { get; set; } = 100;

        // İzin verilen dosya uzantıları
        public string[] AllowedExtensions { get; set; } =
        {
            ".jpg", ".jpeg", ".png", ".gif", // Resimler
            ".mp4", ".mov", ".avi",          // Videolar
            ".pdf", ".doc", ".docx",         // Dokümanlar
            ".mp3", ".wav"                   // Ses dosyaları
        };

        // Upload klasörü
        public string UploadPath { get; set; } = "uploads";

    }

    // Email ayarları
    public class EmailSettings
    {
        // SMTP server
        public string SmtpServer { get; set; } = "";

        // SMTP port
        public int SmtpPort { get; set; } = 587;

        // Email kullanıcı adı
        public string Username { get; set; } = "";

        // Email şifre
        public string Password { get; set; } = "";

        // Gönderen email adresi
        public string FromEmail { get; set; } = "";

        // Gönderen adı
        public string FromName { get; set; } = "ELearning Platform";
    }


    //Cache ayarları
    public class CacheSettings
    {
        // Default cache süresi (dakika)
        public int DefaultDurationMinutes { get; set; } = 30;

        // Uzun süreli cache (saat)
        public int LongDurationHours { get; set; } = 24;

        // Redis connection string
        public string? RedisConnectionString { get; set; }

    }

    // Security ayarları
    public class SecuritySettings
    {

        // JWT secret key
        public string JwtSecretKey { get; set; } = "";

        // JWT token süresi (dakika)
        public int JwtExpirationMinutes { get; set; } = 60;

        // Password minimum uzunluk
        public int PasswordMinLength { get; set; } = 8;

        // Account lockout süresi (dakika)
        public int LockoutDurationMinutes { get; set; } = 15;
    }
}
