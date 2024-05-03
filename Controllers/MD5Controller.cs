using Microsoft.AspNetCore.Mvc;
using OTPModule.Services.IServices;

namespace OTPModule.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MD5Controller: ControllerBase
{
    private readonly IMD5Service _md5Service;

    public MD5Controller(IMD5Service md5Service)
    {
        _md5Service = md5Service;
    }
    
    [HttpPost("encode")]
    public async Task<ActionResult<String>> Encode([FromQuery] String message)
    {
        var result = _md5Service.MD5Encrypt(message);
        return Ok(new { Result = result });
    }
    
    [HttpPost("collision")]
    public async Task<ActionResult<String>> FindCollision()
    {
        var result = _md5Service.FindCollision();
        return Ok(new { Result = result });
    }
}