using Microsoft.AspNetCore.Mvc;
using OTPModule.Dto;
using OTPModule.Services.IServices;

namespace OTPModule.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DesController : ControllerBase
{
    private readonly IDesService _desService;

    public DesController(IDesService desService)
    {
        _desService = desService;
    }
    
    [HttpPost("encode")]
    public async Task<ActionResult<DesResponse>> Encode([FromBody] DesRequest request)
    {
        var result = await _desService.DesEncode(request.Input, request.Key);
        return Ok(new { Result = result });
    }

    [HttpPost("decode")]
    public async Task<ActionResult<DesResponse>> Decode([FromBody] DesRequest request)
    {
        var result = await _desService.DesDecode(request.Input, request.Key);
        return Ok(new { Result = result });
    }
    
    [HttpPost("brute")]
    public async Task<ActionResult<String>> Decode([FromQuery] String text, [FromQuery] String plainText)
    {
        var result = await _desService.BruteForceDecrypt(text, plainText);
        return Ok(new { Result = result });
    }
}