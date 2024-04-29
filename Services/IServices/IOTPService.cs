namespace OTPModule.Services.IServices
{
    public interface IOTPService
    {
        public byte[] GenerateSecretKey();
        public string GenerateTotp(byte[] secretKey);
        public string Base32Encode(byte[] data);
        public bool Verify(string currentTotp, byte[] secretKey);
    }
}
