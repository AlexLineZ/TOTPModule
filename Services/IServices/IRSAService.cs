using OTPModule.Dto;
using System;

namespace OTPModule.Services.IServices
{
    public interface IRSAService
    {
        public EncodeDto Encode(string plaintext);

        public string Decode(DecodeDto decodeDto);
    }
}
