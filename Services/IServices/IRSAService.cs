using OTPModule.Dto;
using System;

namespace OTPModule.Services.IServices
{
    public interface IRSAService
    {
        public KeysDto GenerateKeys();

        public string Encode(EncodeMessageDto encodeMessageDto);

        public string Decode(DecodeMessageDto decodeMessageDto);
        public PrivateKeyDto GetPrivateKeyByPublic(OpenKeyDto openKey);
    }
}
