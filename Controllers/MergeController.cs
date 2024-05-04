using Microsoft.AspNetCore.Mvc;
using OTPModule.Dto;
using OTPModule.Services.IServices;

namespace OTPModule.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MergeController: ControllerBase
{
    private readonly IMD5Service _md5Service;
    private readonly IRSAService _rsaService;
    private readonly IDesService _desService;

    public MergeController(IMD5Service md5Service, IRSAService rsaService, IDesService desService)
    {
        _md5Service = md5Service;
        _rsaService = rsaService;
        _desService = desService;
    }
    
    [HttpPost("merge_encode")]
    public async Task<ActionResult<String>> MergeEncode([FromBody] MergeEncodeDto merge)
    {
        var desString = await _desService.DesEncode(merge.Message, merge.DesKey);
        var desMessage = desString.Output;
        
        var openRsaKeys = _rsaService.GenerateKeys().OpenKey;
        var rsaString = _rsaService.Encode(new EncodeMessageDto { Message = desMessage, OpenKey = openRsaKeys });

        var md5String = _md5Service.MD5Encrypt(rsaString);
        
        return Ok(new { Result = md5String });
    }
}