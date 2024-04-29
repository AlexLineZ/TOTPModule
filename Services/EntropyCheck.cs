using OTPModule.Services.IServices;
using System.Security.Cryptography;

namespace OTPModule.Services
{
    public class EntropyCheck: IEntropyCheck
    {
        public bool IsEntropySufficient()
        {
            try
            {
                byte[] randomNumber = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
