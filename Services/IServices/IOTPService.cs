namespace OTPModule.Services.IServices
{
    public interface IOTPService
    {
        public Task<byte[]> GenerateSecretKey();
        public string GenerateTotp(byte[] secretKey);
        public bool Verify(string currentTotp, byte[] secretKey);
    }
}
