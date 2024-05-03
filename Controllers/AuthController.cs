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
        private readonly IRSAService _rsaService;
        public AuthController(IAuthService authService, IRSAService RSAService)
        {
            _authService = authService;
            _rsaService = RSAService;
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

        [HttpPost("generateKeys")]
        public async Task<ActionResult<KeysDto>> GenerateKeys()
        {
            return Ok(_rsaService.GenerateKeys());
        }

        [HttpPost("encodeMessage")]
        public async Task<ActionResult<string>> EncodeMessage([FromBody] EncodeMessageDto encodeMessageDto)
        { 
            return Ok(_rsaService.Encode(encodeMessageDto));
        }

        [HttpPost("decodeMessage")]
        public async Task<ActionResult<string>> DecodeMessage([FromBody] DecodeMessageDto decodeMessageDto)
        {
            return Ok(_rsaService.Decode(decodeMessageDto));
        }

        [HttpPost("getPrivateKeyByPublic")]
        public async Task<ActionResult<PrivateKeyDto>> GetPrivateKeyByPublic([FromBody] OpenKeyDto openKey)
        {
            return Ok(_rsaService.GetPrivateKeyByPublic(openKey));
        }
    }
}
