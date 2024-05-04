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
        public async Task<ActionResult<TokenResponseDto>> Register([FromBody] UserRegisterDto userRegister)
        {
            var token = await _authService.Register(userRegister);
            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto loginCredentials)
        {
            var TOTP = await _authService.Login(loginCredentials);
            return Ok(TOTP);
        }

        [HttpPost("twoFactorAuthentication")]
        public async Task<ActionResult<TokenResponseDto>> TwoFactorAuthentication([FromBody] TOTPDto TOTPDto)
        {
            var token = await _authService.TwoFactorAuthentication(TOTPDto);
            return Ok(token);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            //string token = HttpContext.Request.Headers["Authorization"];
            //var user = await _authService.GetUser(token);
            return Ok();
        }
    }
}
