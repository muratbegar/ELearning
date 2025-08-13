using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.Enums;
using ELearningIskoop.Shared.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.ValueObjects;

namespace ELearningIskoop.Users.Domain.Entitites
{
    // User aggregate root - Identity framework ile entegre
    public class User : BaseEntity
    {
        private readonly List<RefreshToken> _refreshTokens = new();
        private readonly List<UserRole> _userRoles = new();
        private readonly List<UserLogin> _loginHistory = new ();

        protected User(){}

        private User(Email email, PersonName name, string passwordHash,int? createdBy=null)
        {
          ObjectId = 0;
          Email = email;
          Name = name;
          Username = email.Value.Split('@')[0];
          Status = UserStatus.Inactive;//Email doğrulması bekliyor
          SecurityStamp = GenerateSecurityStamp();
          CreatedBy = createdBy;
          CreatedAt = DateTime.UtcNow;
          FailedLoginAttempts = 0;

        }
        //Properties
        public Email Email { get; private set; } = null;
        public PersonName Name { get; private set; } = null;
        public string Username { get; private set; } = string.Empty;

        public HashedPassword Password { get; private set; } = null;

        public UserStatus Status { get;private set; }

        public string SecurityStamp { get; private set; } = string.Empty;

        //Security && Login
        public int FailedLoginAttempts { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public DateTime? LockedUntil { get; private set; }
        public bool IsLocked => LockedUntil.HasValue && LockedUntil > DateTime.UtcNow;


        //Profile
        public string? ProfilePictureUrl { get; private set; }
        public string? Bio { get; private set; }
        public string? PhoneNumber { get;private set; }
        public DateTime? EmailVerifiedAt { get; private set; }
        public bool IsMailVerified => EmailVerifiedAt.HasValue;


        //Navigation Properties
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
        public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();
        public IReadOnlyCollection<UserLogin> LoginHistory => _loginHistory.AsReadOnly();


        // Yeni kullanıcı oluşturur
        public static User Create(Email email, PersonName name, string plainPassword, int? createdBy = null)
        {
            if(string.IsNullOrWhiteSpace(plainPassword))
                throw new DomainException("Password cannot be empty");

            var hashedPassword = HashedPassword.Create(plainPassword);
            var user = new User(email, name, hashedPassword, createdBy);

            user.AddDomainEvent(new UserCreatedDomainEvent(user.ObjectId, email));
            return user;
        }

        // Şifre doğrulama
        public bool VerifyPassword(string plainPassword)
        {
            return Password.Verify(plainPassword);
        }

        // Şifre değiştirme
        public void ChangePassword(string currentPassword, string newPassword, int? changedBy = null)
        {
            if (!VerifyPassword(currentPassword))
                throw new BusinessRuleViolationException("INVALID_PASSWORD", "Current password is incorrect");

            Password = HashedPassword.Create(newPassword);
            SecurityStamp = GenerateSecurityStamp();
            SetUpdatedInfo();



            //Şifre değişince tüm refresh tokenları iptal et
            RevokeAllRefreshTokens("Password changed");
            AddDomainEvent(new UserPasswordChangedDomainEvent(ObjectId));
        }


        // Şifre sıfırlama (Admin veya forgot password flow)
        public void ResetPassword(string newPassword, int? resetBy = null)
        {
            Password = HashedPassword.Create(newPassword);
            SecurityStamp = GenerateSecurityStamp();
            FailedLoginAttempts = 0;
            LockedUntil = null;
            SetUpdatedInfo(resetBy);
            RevokeAllRefreshTokens("Password reset");

            AddDomainEvent(new UserPasswordResetDomainEvent(Id));

        }


        // Login denemesi kaydet
        public LoginResult AttemptLogin(string password, string ipAddress, int? userAgent = null)
        {
            //Hesap Kilitli mi
            if (IsLocked)
            {
                return LoginResult.Failed($"Account is locked until {LockedUntil:yyyy-MM-dd HH:mm:ss} UTC");
            }

            //Aktif mi
            if (Status != UserStatus.Active)
            {
                return LoginResult.Failed($"Account is {Status}");
            }

            // Şifre doğru mu?
            if (!VerifyPassword(password))
            {
                RecordFailedLogin(ipAddress);
                return LoginResult.Failed("Invalid credentials");
            }

            // Başarılı login
            RecordSuccessfulLogin(ipAddress, userAgent);
            return LoginResult.Success();
        }



        // Başarılı login kaydet
        private void RecordSuccessfulLogin(string ipAddress, int? userAgent)
        {
            FailedLoginAttempts = 0;
            LastLoginAt = DateTime.UtcNow;
            
            var loginRecord = UserLogin.Create(ObjectId,ipAddress, userAgent,true);
            _loginHistory.Add(loginRecord);


            // Son 1000 login kaydını tut
            if (_loginHistory.Count > 1000)
            {
                var oldestLogin = _loginHistory.OrderBy(l => l.CreatedAt).First();
                _loginHistory.Remove(oldestLogin);
            }

            AddDomainEvent(new UserLoggedDomainEvent(ObjectId,ipAddress));
        }

        // Başarısız login kaydet
        private void RecordFailedLogin(string ipAdress)
        {
            FailedLoginAttempts++;

            var loginRecord = UserLogin.Create(ObjectId,ipAdress,null,false);
            _loginHistory.Add(loginRecord);

            //5 denemeden sonra kitle
            if (FailedLoginAttempts >= 5)
            {
                LockAccount(TimeSpan.FromMinutes(15));
            }

            AddDomainEvent(new UserLoginFailedDomainEvent(ObjectId,FailedLoginAttempts));

        }

        // Hesabı kilitle
        public void LockAccount(TimeSpan duration, int? lockedBy = null)
        {
            LockedUntil = DateTime.UtcNow.Add(duration);
            SetUpdatedInfo();
            AddDomainEvent(new UserLockedDomainEvent(ObjectId, LockedUntil.Value));

        }

        // Hesap kilidini aç
        public void UnlockAccount(int? unlockedBy = null)
        {
            LockedUntil = null;
            FailedLoginAttempts = 0;
            SetUpdatedInfo(unlockedBy);

            AddDomainEvent(new UserUnlockedDomainEvent(ObjectId));
        }

        // Email doğrulama
        public void VerifyEmail(int? verifiedBy = null)
        {
            if (IsMailVerified)
                throw new BusinessRuleViolationException("EMAIL_ALREADY_VERIFIED", "Email is already verified");

            EmailVerifiedAt = DateTime.UtcNow;
            Status = UserStatus.Active;
            SetUpdatedInfo(verifiedBy);

            AddDomainEvent(new UserEmailVerifiedDomainEvent(ObjectId, Email));
        }

        // Refresh token ekle
        public RefreshToken AddRefreshToken(string ipAddress, int daysToExpire = 7)
        {
            var token = RefreshToken.Generate();
            var refreshToken = RefreshToken.Create(ObjectId, token, ipAddress, daysToExpire);
            _refreshTokens.Add(refreshToken);

            var activeTokens = _refreshTokens.Where(x => x.IsActive).OrderByDescending(x => x.CreatedAt).Skip(5)
                .ToList();

            foreach (var oldToken in activeTokens)
            {
                oldToken.Revoke("Exceeded token limit", ipAddress);
            }

            return refreshToken;
        }


        // Refresh token iptal et
        public void RevokeRefreshToken(string token, string reason, string? ipAddress = null)
        {
            var refreshToken = _refreshTokens.SingleOrDefault(x => x.Token == token);
            if (refreshToken == null || !refreshToken.IsActive)
                throw new BusinessRuleViolationException("INVALID_TOKEN", "Invalid or inactive refresh token");

            refreshToken.Revoke(reason,ipAddress);
        }

        // Tüm refresh token'ları iptal et
        public void RevokeAllRefreshToken(string reason, string? ipAddress = null)
        {
            foreach (var token in _refreshTokens.Where(x=>x.IsActive))
            {
                token.Revoke(reason,ipAddress);
            }
        }

        // Rol ekle
        public void AssignRole(Role role, int? assignedBy = null)
        {
            if(_userRoles.Any(ur=>ur.RoleId == (role == Role.ObjectId)))
                return;//zaten rolü var

            var userRole = UserRole.Create(ObjectId, role.ObjectId);
            _userRoles.Add(userRole);
            SetUpdatedInfo(assignedBy);

            AddDomainEvent(new UserRoleAssignedDomainEvent(object, role.Name));
        }


        // Rol kaldır
        public void RemoveRule(int roleId, int? removedBy = null)
        {
            var userRole = _userRoles.FirstOrDefault(x => x.ObjectId);
            if (userRole == null) return;
            else
            {
                _userRoles.Remove(userRole);
                SetUpdatedInfo(removedBy);
                AddDomainEvent(new UserRoleRemovedDomainEvent(ObjectId,roleId));
            }
        }

        // Kullanıcı rollerini kontrol et
        public bool HasRole(string roleName)
        {
            return _userRoles.Any(x => x.Role.Name == roleName);
        }


        // Profil güncelle
        public void UpdateProfile(PersonName? name = null,
            string? bio = null,
            string? phoneNumber = null,
            string? profilePictureUrl = null,
            string? updatedBy = null)
        {
            if (name != null) Name = name;
            if (bio != null) Bio = bio;
            if(phoneNumber!=null) PhoneNumber = phoneNumber;
            if (profilePictureUrl != null) ProfilePictureUrl = profilePictureUrl;

            SetUpdatedInfo();
            AddDomainEvent(new UserProfileUpdatedDomainEvent(ObjectId));
        }


        // Hesap durumunu değiştir
        public void ChangeStatus(UserStatus newStatus, int? changedBy = null)
        {
            if(Status == newStatus)
                return;

            var oldStatus = Status;
            Status = newStatus;
            SetUpdatedInfo(changedBy);
            AddDomainEvent(new UserStatusChangedDomainEvent(ObjectId, oldStatus, newStatus));
        }

        private static string GenerateSecurityStamp()
        {
            return Guid.NewGuid().ToString("N");
        }

    }
}
