using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Shared.Domain.Enums
{
    public enum ContentType
    {
        Video = 1,          // Video dersi
        Document = 2,      // Doküman dersi
        Quiz = 3,          // Sınav dersi
        Interactive = 4,    // Ödev dersi
        Audio = 5, //ses
        LiveStream = 6,   // Canlı oturum
        Assignment = 7, // Ödev
    }

    public static class ContentTypeExtensions
    {
        // İçerik türünün açıklamasını döner
        public static string GetDescription(this ContentType type)
        {
            return type switch
            {
                ContentType.Video => "Video dersi",
                ContentType.Document => "Doküman dersi",
                ContentType.Quiz => "Sınav dersi",
                ContentType.Interactive => "Etkileşimli ders",
                ContentType.Audio => "Ses dersi",
                ContentType.LiveStream => "Canlı oturum",
                ContentType.Assignment => "Ödev",
                _ => "Bilinmeyen içerik türü"
            };
        }

        // İzleme süresi ölçülebilir mi?
        public static bool IsTimeMeasurable(this ContentType type)
        {
            return type switch
            {
                ContentType.Video => true,          // Video dersleri izlenebilir
                ContentType.Document => false,      // Doküman dersleri izlenemez
                ContentType.Quiz => false,          // Sınav dersleri izlenemez
                ContentType.Interactive => true,    // Etkileşimli dersler izlenebilir
                ContentType.Audio => true,          // Ses dersleri izlenebilir
                ContentType.LiveStream => true,     // Canlı oturumlar izlenebilir
                ContentType.Assignment => false,    // Ödevler izlenemez
                _ => false                          // Bilinmeyen türler izlenemez
            };
        }

        // Etkileşimli içerik mi?
        public static bool IsInteractive(this ContentType type)
        {
            return type switch
            {
                ContentType.Interactive => true,    // Etkileşimli dersler etkileşimlidir
                ContentType.Quiz => true,           // Sınav dersleri de etkileşimlidir
                ContentType.Assignment => true,     // Ödevler de etkileşimlidir
                _ => false                          // Diğer türler etkileşimli değildir
            };
        }
    }
}
