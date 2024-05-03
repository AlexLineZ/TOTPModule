using OTPModule.Dto;

namespace OTPModule.Services.IServices
{
    public interface IAuthService
    {
        public Task<string> Register(UserRegisterDto registerDto);
        public Task<string> Login(LoginDto loginDto);
        public Task<string> LoginQR(LoginDto loginDto);
        public Task<TokenResponseDto> TwoFactorAuthentication(TOTPDto TOTPDto);
    }
}
