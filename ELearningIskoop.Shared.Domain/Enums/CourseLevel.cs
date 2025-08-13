using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Shared.Domain.Enums
{
    // Kurs zorluk seviyeleri
    public enum CourseLevel
    {
        // Başlangıç seviyesi
        Beginner = 1,

        // Orta seviye
        Intermediate = 2,

        // İleri seviye
        Advanced = 3,

        // Uzman seviyesi
        Expert = 4

    }

    public static class CourseLevelExtensions
    {
        // Zorluk seviyesinin açıklamasını döner
        public static string GetDescription(this CourseLevel level)
        {
            return level switch
            {
                CourseLevel.Beginner => "Başlangıç seviyesi - yeni başlayanlar için",
                CourseLevel.Intermediate => "Orta seviye - temel bilgisi olanlar için",
                CourseLevel.Advanced => "İleri seviye - deneyimli kullanıcılar için",
                CourseLevel.Expert => "Uzman seviyesi - profesyoneller için",
                _ => "Bilinmeyen seviye"
            };
        }

        // Seviye sıra numarası
        public static int GetOrder(this CourseLevel level)
        {
            return level switch
            {
                CourseLevel.Beginner => 1,
                CourseLevel.Intermediate => 2,
                CourseLevel.Advanced => 3,
                CourseLevel.Expert => 4,
                _ => 0 // Bilinmeyen seviye için varsayılan sıra numarası
            };
        }

        // Önceki seviye
        public static CourseLevel? GetPreviousLevel(this CourseLevel level)
        {
            return level switch
            {
                CourseLevel.Beginner => null, // Başlangıç seviyesi için önceki seviye yok
                CourseLevel.Intermediate => CourseLevel.Beginner,
                CourseLevel.Advanced => CourseLevel.Intermediate,
                CourseLevel.Expert => CourseLevel.Advanced,
                _ => null // Bilinmeyen seviye için null döner
            };
        }
    }
}
