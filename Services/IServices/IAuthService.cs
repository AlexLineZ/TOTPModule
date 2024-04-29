using OTPModule.Dto;

namespace OTPModule.Services.IServices
{
    public interface IAuthService
    {
        public Task<TokenResponseDto> Register(UserRegisterDto registerDto);
        public Task<string> Login(LoginDto loginDto);
        public Task<TokenResponseDto> TwoFactorAuthentication(TOTPDto TOTPDto);
        public Task Logout(string token);
    }
}
