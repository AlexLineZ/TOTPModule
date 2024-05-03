using Microsoft.EntityFrameworkCore;
using QRCoder;
using OTPModule.Data;
using OTPModule.Dto;
using OTPModule.Entities;
using OTPModule.Migrations;
using OTPModule.Services.IServices;
using QRCoder;

namespace OTPModule.Services
{
    public class AuthService : IAuthService
    {
        UserDbContext _userDbContext;
        IOTPService _OTPService;
        public AuthService(UserDbContext userDbContext, IOTPService OTPService)
        {
            _userDbContext = userDbContext;
            _OTPService = OTPService;
        }

        public async Task<string> Login(LoginDto loginDto)
        {
            var user = await _userDbContext.userEntities.Include(u => u.SecretKey).FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null) { throw new Exception(); }
            
            var secretKey = user.SecretKey.SecretKey;

            return GenerateQR(secretKey);
        }

        private string GenerateQR(byte[] data)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

            Base64QRCode qrCode = new Base64QRCode(qrCodeData);

            return qrCode.GetGraphic(20);
        }

        public async Task<string> Register(UserRegisterDto registerDto)
        {
            var secretKey = new SecretKeyEntity()
            {
                Id = Guid.NewGuid(),
                SecretKey = await _OTPService.GenerateSecretKey()
            };

            var user = new UserEntity()
            {
                Email = registerDto.Email,
                Id = Guid.NewGuid(),
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                SecretKey = secretKey
            };
            _userDbContext.userEntities.Add(user);
            await _userDbContext.SaveChangesAsync();

            return GenerateQR(secretKey.SecretKey);
        }

        public async Task<TokenResponseDto> TwoFactorAuthentication(TOTPDto TOTPDto)
        {
            var user = await _userDbContext.userEntities.Include(u => u.SecretKey).FirstOrDefaultAsync(u => u.Email == TOTPDto.Email);
            if (user == null) { throw new Exception(); }

            var secretKey = user.SecretKey.SecretKey;

            var isRight = _OTPService.Verify(TOTPDto.Code, secretKey);

            if (isRight) { return new TokenResponseDto() { Token = "" }; }
            throw new Exception();
        }
    }
}
