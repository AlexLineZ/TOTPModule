namespace OTPModule.Services.IServices
{
    public interface ICryptoService
    {
        public string RSAEncode();
        public string DESEncode();
        public string MD5Encode();
    }
}
