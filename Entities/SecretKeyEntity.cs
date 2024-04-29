namespace OTPModule.Entities
{
    public class SecretKeyEntity
    {
        public Guid Id { get; set; }
        public byte[] SecretKey { get; set; }
    }
}
