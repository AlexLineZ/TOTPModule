using Microsoft.AspNetCore.Mvc;
using OTPModule.Dto;

namespace OTPModule.Services.IServices;

public interface IDesService
{
    public Task<DesResponse> DesEncode(string s, string key);
    
    public Task<DesResponse> DesDecode(string s, string key);
}