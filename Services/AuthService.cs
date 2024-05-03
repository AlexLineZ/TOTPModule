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

            return _OTPService.GenerateTotp(secretKey);
        }

        public Task Logout(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<QrCodeDto> Register(UserRegisterDto registerDto)
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
            
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(secretKey.SecretKey, QRCodeGenerator.ECCLevel.Q);

            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            string qrCodeBase64 = qrCode.GetGraphic(20);

            return new QrCodeDto { Qr = qrCodeBase64 };
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
