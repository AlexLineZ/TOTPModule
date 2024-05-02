using Newtonsoft.Json;
using OTPModule.Dto;
using OTPModule.Services.IServices;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace OTPModule.Services
{
    public class OTPService : IOTPService
    {
        private const int Step = 30;
        private const int DigitLength = 6;

        private HttpClient _httpClient;
        private IEntropyCheck _entropyCheck;

        public OTPService(HttpClient httpClient, IEntropyCheck entropyCheck)
        {
            _httpClient = httpClient;
            _entropyCheck = entropyCheck;
        }

        // https://habr.com/ru/articles/534064 
        public async Task<byte[]> GenerateSecretKey()
        {
            if (_entropyCheck.IsEntropySufficient())
            {
                return GenerateSecretKeyWithEntropy();
            }
            else
            {
                return await GetSecretKeyUsingRandomAPI();
            }
        }

        private byte[] GenerateSecretKeyWithEntropy()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] secretKey = new byte[20];
                rng.GetBytes(secretKey);
                return secretKey;
            }
        }

        private long GetTimeStep()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() / Step;
        }

        private async Task<byte[]> GetSecretKeyUsingRandomAPI() {
           var httpResponse = await _httpClient.GetAsync("https://randomuser.me/api/?results=1");

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception("Произошла ошибка");
            };

            var users = JsonConvert.DeserializeObject<RootObject>(await httpResponse.Content.ReadAsStringAsync());

            var email = users.Results[0].Email;
            var nationality = users.Results[0].Nat;

            byte[] bytes = Encoding.UTF8.GetBytes(email + nationality);

            return bytes;
        }

        public string GenerateTotp(byte[] secretKey)
        {
            long timestep = GetTimeStep();
            byte[] timestepBytes = BitConverter.GetBytes(timestep);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(timestepBytes);

            using (var hmac = new HMACSHA1(secretKey))
            {
                byte[] hash = hmac.ComputeHash(timestepBytes);
                int offset = hash[hash.Length - 1] & 0xf;
                int binary = (hash[offset] & 0x7f) << 24
                             | (hash[offset + 1] & 0xff) << 16
                             | (hash[offset + 2] & 0xff) << 8
                             | (hash[offset + 3] & 0xff);

                int otp = binary % (int)Math.Pow(10, DigitLength);
                return otp.ToString(new string('0', DigitLength));
            }
        }

        public bool Verify(string currentTotp, byte[] secretKey)
        {
            var newTotp = GenerateTotp(secretKey);

            return newTotp == currentTotp;
        }
    }
}
