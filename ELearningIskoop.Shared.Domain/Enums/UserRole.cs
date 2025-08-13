using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Shared.Domain.Enums
{
    public enum enUserRole
    {
        // Öğrenci - kurs satın alabilir, derslerine katılabilir
        Student = 1,

        // Eğitmen - kurs oluşturabilir, öğrenci ilerleme raporu alabilir
        Instructor = 2,

        // Yönetici - tüm kursları yönetebilir, kullanıcıları yönetebilir
        Admin = 3,

        //Moderatör - içerik denetimi yapabilir
        Moderator = 4,

        // Misafir - kursları görüntüleyebilir, ancak satın alma veya katılma yetkisi yoktur
        Guest = 5

        
    }

    // UserRole enum için extension metodlar
    public static class UserRoleExtensions
    {
        //Rol açıklamasını döner
        public static string GetDescription(this enUserRole role)
        {
            return role switch
            {
                enUserRole.Student => "Öğrenci - kurs satın alabilir, derslerine katılabilir",
                enUserRole.Instructor => "Eğitmen - kurs oluşturabilir, öğrenci ilerleme raporu alabilir",
                enUserRole.Admin => "Yönetici - tüm kursları yönetebilir, kullanıcıları yönetebilir",
                enUserRole.Moderator => "Moderatör - içerik denetimi yapabilir",
                enUserRole.Guest => "Misafir - kursları görüntüleyebilir, ancak satın alma veya katılma yetkisi yoktur",
                _ => "Bilinmeyen rol"
            };
        }

        // Yönetici yetkisi var mı?
        public static bool HasAdminPrivileges(this enUserRole role)
        {
            return role == enUserRole.Admin || role == enUserRole.Moderator;
        }

        // İçerik oluşturabilir mi?
        public static bool CanCreateContent(this enUserRole role) =>
            role == enUserRole.Instructor || role.HasAdminPrivileges();
    }
}
