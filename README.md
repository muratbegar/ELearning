------------------------------------GENEL--------------------------------------

Common/ → temel domain building blocks (Entity, ValueObject, Result pattern vs.)

Exceptions/ → bütün exception tiplerini burada toplayıp API’de global handler ile kullanabiliriz

Interfaces/ → modüller arası bağımsız soyutlamalar (repo, unit of work, audit, aggregate root)

Utils/ → extension, helper, guard pattern

Logging → ILogger soyutlaması + Serilog adapter (başka log sistemi kullanmak istersek kolayca değişir)

Caching → memory, redis vs. için soyutlama

Security → password hashing, token üretim, encryption

Auditing → createdBy, updatedBy, history takip mekanizmaları

Notifications → e-posta, sms, push notification soyutlamaları

Localization → dil desteği (TR/EN vb.)
------------------------------------GENEL--------------------------------------

--------------------------------------ENUM-----------------------------------------


// Tüm enum değerlerini al
var allLevels = Enumeration.GetAll<CourseLevel>();

// Value'dan enum bul
var level = Enumeration.FromValue<CourseLevel>(2); // Intermediate

// Name'den enum bul
var status = Enumeration.FromDisplayName<EnrollmentStatus>("Active");

// Comparison
if (CourseLevel.Beginner < CourseLevel.Advanced) // true
{
    Console.WriteLine("Beginner is less than Advanced");
}

// Equality check
if (status == EnrollmentStatus.Active) // true
{
    Console.WriteLine("Enrollment is active");
}

if (Enumeration.ContainsValue<CourseLevel>(5)) // false
{
    Console.WriteLine("Value exists");
}

--------------------------------------ENUM-----------------------------------------

-------------------------------------VALUE OBJECT---------------------------------
// Equality test
var email1 = new Email("test@example.com");
var email2 = new Email("test@example.com");
var email3 = new Email("other@example.com");

Console.WriteLine(email1 == email2); // True
Console.WriteLine(email1 == email3); // False
Console.WriteLine(email1.Equals(email2)); // True

// HashCode test
var set = new HashSet<Email> { email1, email2, email3 };
Console.WriteLine(set.Count); // 2 (email1 ve email3)

// Collection içinde value object
var addresses = new List<Address>
{
    new Address("St1", "City1", "Zip1", "Country1"),
    new Address("St1", "City1", "Zip1", "Country1"), // Duplicate
    new Address("St2", "City2", "Zip2", "Country2")
};

var distinctAddresses = addresses.Distinct().ToList();
Console.WriteLine(distinctAddresses.Count); // 2

// Owned type olarak kullanım
modelBuilder.Entity<User>(entity =>
{
    entity.OwnsOne(u => u.Address, address =>
    {
        address.Property(a => a.Street).HasMaxLength(100);
        address.Property(a => a.City).HasMaxLength(50);
    });
});

-------------------------------------VALUE OBJECT---------------------------------

-----------------------------------TransactorBy-----------------------------------

var createdBy = new TransactorBy(123, "john.doe");
var systemUser = TransactorBy.System();

var fromFactory = TransactorBy.FromValues(456, "jane.smith");

if (CreatedBy.TryCreate(789, "bob.wilson", out var result))
{
    Console.WriteLine($"Created: {result}");
}

-----------------------------------TransactorBy-----------------------------------


-------------------------------------Result ---------------------------------

// Basit kullanım
var successResult = Result.Success();
var failureResult = Result.Failure("Something went wrong");

// Generic result
var userResult = Result.Success(new User());
var errorResult = Result.Failure<User>("User not found", 404);

// Functional programming style
var result = await GetUserById(1)
    .Then(user => ValidateUser(user))
    .Then(validUser => SaveUser(validUser))
    .OnSuccess(() => Console.WriteLine("User saved"))
    .OnFailure(error => Console.WriteLine($"Error: {error}"));


-------------------------------------Result ---------------------------------

-------------------------------------BusinessRuleException ---------------------------------
 
 // Basit kural ihlali
throw new BusinessRuleException("Kullanıcı bakiyesi yetersiz", "INSUFFICIENT_BALANCE");

// Detay ekleyerek
throw new BusinessRuleException("Kullanıcı bakiyesi yetersiz", "INSUFFICIENT_BALANCE")
    .WithParameter("CurrentBalance", 50)
    .WithParameter("RequiredAmount", 100)
    .WithParameter("UserId", 12345)

   

// Kaynak zaten var
throw BusinessRuleException.ResourceAlreadyExists("User", "john@example.com");

// Geçersiz durum geçişi
throw BusinessRuleException.InvalidTransition("Pending", "Deleted", "Order");

// Kota aşımı
throw BusinessRuleException.QuotaExceeded("MonthlyRequests", 1050, 1000);

// Bağımlılık hatası
throw BusinessRuleException.DependencyExists("User", "UserProfile");

 -------------------------------------BusinessRuleException ---------------------------------