using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTPModule.Dto;
using OTPModule.Services.IServices;

namespace OTPModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RSAController : ControllerBase
    {
        private readonly IRSAService _rsaService;
        public RSAController(IRSAService RSAService)
        {
            _rsaService = RSAService;
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
