using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Domain.Enums
{
    public enum CourseStatus
    {
        // Taslak - henüz yayınlanmamış
        Draft = 1,

        // Yayında - öğrenciler kayıt olabilir
        Published = 2,

        // Yayından kaldırılmış
        Unpublished = 3,

        // Arşivlenmiş - artık aktif değil
        Archived = 4
    }
}
