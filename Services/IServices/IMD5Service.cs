namespace OTPModule.Services.IServices;

public interface IMD5Service
{
    public string MD5Encrypt(string message);

    public string FindCollision();
}