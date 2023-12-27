using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserService.Data;
using UserService.Model;

namespace UserService.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context) {
            _context=context;
        }
        public UserVM Add(UserVM user)
        {
            
            var userNew = new User
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = user.Password,
                Gender = user.Gender,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Avatar = user.Avatar,
                IsFirstLogin = user.IsFirstLogin,
                Address = user.Address,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };
            _context.Users.Add(userNew);
            _context.SaveChanges();
            return new UserVM
            {
                Id = userNew.Id,
                Email = userNew.Email,
                FirstName = userNew.FirstName,
                LastName = userNew.LastName,
                Password = userNew.Password,
                Gender = userNew.Gender,
                Phone = userNew.Phone,
                DateOfBirth = userNew.DateOfBirth,
                Avatar = userNew.Avatar,
                IsFirstLogin = userNew.IsFirstLogin,
                Address = userNew.Address,
                IsActive = userNew.IsActive,
                CreatedAt = userNew.CreatedAt,
                UpdatedAt = userNew.UpdatedAt,
            };
        }

        public void Delete(string id)
        {
            var user = _context.Users.SingleOrDefault(user => user.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public List<UserVM> getAll()
        {
            
            var users = _context.Users.Select(user => new UserVM
            {
                Id=user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = user.Password,
                Gender = user.Gender,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Avatar = user.Avatar,
                IsFirstLogin = user.IsFirstLogin,
                Address = user.Address,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            });
            return users.ToList();
        }

        public UserVM getById(string id)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(user => user.Id == id);
                if(user == null)
                {
                    return null;
                }
                UserVM us = new UserVM
                {
                    Id= user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Password = user.Password,
                    Gender = user.Gender,
                    Phone = user.Phone,
                    DateOfBirth = user.DateOfBirth,
                    Avatar = user.Avatar,
                    IsFirstLogin = user.IsFirstLogin,
                    Address = user.Address,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                };
                return us;
            }
            catch
            {
                return null;
            }
        }

        public void Update(UserVM user)
        {
            var userEdit = _context.Users.SingleOrDefault(u => u.Id == user.Id);
            userEdit.Avatar=user.Avatar;
            _context.Users.Update(userEdit);
            _context.SaveChanges();
        }

        public User Validate(UserLogin model)
        {
            var user= _context.Users.SingleOrDefault(p=>p.Email==model.Email && p.Password==model.Password);
            if (user != null)
            {
                User u= new User
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Password = user.Password,
                    Gender = user.Gender,
                    Phone = user.Phone,
                    DateOfBirth = user.DateOfBirth,
                    Avatar = user.Avatar,
                    IsFirstLogin = user.IsFirstLogin,
                    Address = user.Address,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                };
                return u;
            }
            return null;
        }
        public async Task<TokenModel> GenerateToken(User u, string secretKey)
        {

            var jwtToken = new JwtSecurityTokenHandler();
            var secretKeyByte = Encoding.UTF8.GetBytes(secretKey);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim("Id",u.Id),
                        new Claim(JwtRegisteredClaimNames.Email,u.Email),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),

                    }),
                Expires = DateTime.UtcNow.AddMinutes(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyByte), SecurityAlgorithms.HmacSha512Signature)

            };

            var token = jwtToken.CreateToken(tokenDescription);
            var accessToken = jwtToken.WriteToken(token);
            var refreshToken = GenerateRefreshToken();
            var refToken = new RefreshToken
            {
                id = Guid.NewGuid().ToString(),
                jwt = token.Id,
                UserId = u.Id,
                Token = refreshToken,
                isUse = false,
                isRevoked = false,
                CreatedAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddHours(1),
            };
            await _context.RefreshTokens.AddAsync(refToken);
            await _context.SaveChangesAsync();
            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken=refreshToken,
            };

        }

        public async Task<string> GenerateRefreshToken(TokenModel model, string secretKey)
        {
            var jwtToken = new JwtSecurityTokenHandler();
            var secretKeyByte = Encoding.UTF8.GetBytes(secretKey);
            var param = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyByte),

                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false,
            };
            try
            {
                // check 1: AccessToken valid format
                var tokenVerification = jwtToken.ValidateToken(model.AccessToken, param, out var validatedToken);
                // check 2: alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512,
                        StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        return "Invalid compare";
                    }
                }
                // check expire
                var utcExpire = long.Parse(tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expDate = ConvertToDate(utcExpire);
                if (expDate > DateTime.UtcNow)
                {
                    return "Access token has not yet expired";
                }
                // check refresh exist
                var storedToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == model.RefreshToken);
                if (storedToken == null)
                {
                    return "";
                }
                // check refresh token is use/revoked
                if (storedToken.isUse)
                {
                    return "Refresh token does not exist";
                }
                if (storedToken.isRevoked)
                {
                    return "Refresh token has been used";
                }
                // check accessToken id 
                var jti = tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.jwt != jti)
                {
                    return "efresh token has been revoked";
                }
                // update token
                storedToken.isUse = true;
                storedToken.isRevoked = true;
                _context.Update(storedToken);
                await _context.SaveChangesAsync();
                // create Token
                var usId = storedToken.UserId;
                var us = await _context.Users.SingleOrDefaultAsync(x => x.Id == usId);
                var token = await GenerateToken(us, secretKey);

                return "Success";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private DateTime ConvertToDate(long utcExpire)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpire).ToUniversalTime();
            return dateTimeInterval;
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
            }
            return Convert.ToBase64String(random);
        }

        public UserVM Register(RegisterUser user)
        {
            var r = _context.Roles.FirstOrDefault(r => r.RoleName == user.Role);
            var u = new User
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = user.Password,
                Gender = user.Gender,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Avatar = "default.png",
                IsFirstLogin = true,
                Address = user.Address,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RoleId=r.Id,


            };
            if (u == null)
            {
                return null;
            }
            _context.Users.Add(u);
            _context.SaveChanges();
            return new UserVM
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Password = u.Password,
                Gender = u.Gender,
                Phone = u.Phone,
                DateOfBirth = u.DateOfBirth,
                Avatar = u.Avatar,
                IsFirstLogin = u.IsFirstLogin,
                Address = u.Address,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                RoleId=u.RoleId,
            };
        }

        
    }
}
