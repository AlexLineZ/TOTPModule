namespace OTPModule.Dto
{
    public class DecodeMessageDto
    {
        public List<string> Message { get; set; }
        public PrivateKeyDto PrivateKey { get; set; }
    }
}
