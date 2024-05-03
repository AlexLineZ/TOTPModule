using Microsoft.AspNetCore.Mvc;
using OTPModule.Dto;
using OTPModule.Services.IServices;

namespace OTPModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<FileResult> Register([FromBody] UserRegisterDto userRegister)
        {
            var qr = await _authService.Register(userRegister);
            byte[] imageBytes = Convert.FromBase64String(qr);

            return File(imageBytes, "image/png");
        }

        [HttpPost("login")]
        public async Task<FileResult> Login([FromBody] LoginDto loginCredentials)
        {
            var qr = await _authService.Login(loginCredentials);
            byte[] imageBytes = Convert.FromBase64String(qr);

            return File(imageBytes, "image/png");
        }

        [HttpPost("twoFactorAuthentication")]
        public async Task<ActionResult<TokenResponseDto>> TwoFactorAuthentication([FromBody] TOTPDto TOTPDto)
        {
            var token = await _authService.TwoFactorAuthentication(TOTPDto);
            return Ok(token);
        }
    }
}
