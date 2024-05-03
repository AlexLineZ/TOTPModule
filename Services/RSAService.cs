using OTPModule.Dto;
using OTPModule.Services.IServices;
using System.Diagnostics;
using System.Numerics;

namespace OTPModule.Services
{
    public class RSAService:IRSAService
    {
        private Random random = new Random();

        private ulong Gcd(ulong a, ulong b)
        {
            while (b != 0)
            {
                ulong temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        private ulong ModInverse(ulong a, ulong m)
        {
            ulong m0 = m, x0 = 0, x1 = 1;
            while (a > 1)
            {
                ulong q = a / m;
                ulong temp = m;
                m = a % m;
                a = temp;
                temp = x0;
                x0 = x1 - q * x0;
                x1 = temp;
            }
            return x1 < 0 ? x1 + m0 : x1;
        }

        private bool IsPrime(ulong n, int k = 5)
        {
            if (n <= 1)
                return false;
            else if (n <= 3)
                return true;
            else if (n % 2 == 0)
                return false;

            ulong d = n - 1;
            while (d % 2 == 0)
                d /= 2;

            for (int i = 0; i < k; i++)
            {
                ulong a = RandomNumber(2, n - 1);
                ulong x = ModPow(a, d, n);
                if (x == 1 || x == n - 1)
                    continue;
                for (int j = 0; j < (int)Math.Ceiling(Math.Log(n - 1, 2)); j++)
                {
                    x = ModPow(x, 2, n);
                    if (x == n - 1)
                        break;
                }
                if (x != n - 1)
                    return false;
            }
            return true;
        }

        private ulong GeneratePrime(int keyLength = 128)
        {
            ulong p = RandomNumber(0, 1) | (1UL << keyLength - 1) | 1;

            while (!IsPrime(p))
                p = RandomNumber(0, 1) | (1UL << keyLength - 1) | 1;

            return p;
        }


        private ulong RandomNumber(ulong min, ulong max)
        {
            byte[] data = new byte[sizeof(ulong)];
            random.NextBytes(data);
            ulong result = BitConverter.ToUInt64(data, 0);
            return result % (max - min) + min;
        }

        private KeysDto GenerateKeyPair(int keyLength = 128)
        {
            ulong p = GeneratePrime(keyLength);
            ulong q = GeneratePrime(keyLength);

            ulong n = p * q;
            ulong phi = (p - 1) * (q - 1);

            ulong e = RandomNumber(1, phi);
            ulong g = Gcd(e, phi);
            while (g != 1)
            {
                e = RandomNumber(1, phi);
                g = Gcd(e, phi);
            }

            ulong d = ModInverse(e, phi);

            var openKey = new OpenKeyDto()
            {
                E = e,
                N = n
            };

            var closedKey = new PrivateKeyDto()
            {
                D = d,
                N = n
            };

            return new KeysDto() {
                ClosedKey = closedKey,
                OpenKey = openKey,
            };
        }

        public EncodeDto Encode(string plaintext)
        {
            var keys = GenerateKeyPair();

            var cipher = new ulong[plaintext.Length];
            for (int i = 0; i < plaintext.Length; i++)
                cipher[i] = ModPow(plaintext[i], keys.OpenKey.E, keys.OpenKey.N);

            return new EncodeDto() {
                Keys = keys,
                Message = string.Join("", cipher)
            };
        }

        public string Decode(DecodeDto decodeDto)
        {
            string plain = "";
            for (int i = 0; i < decodeDto.Message.Length; i++)
                plain += (char)ModPow(decodeDto.Message[i], decodeDto.PrivateKey.D, decodeDto.PrivateKey.N);

            return plain;
        }

        private ulong ModPow(ulong baseNum, ulong exponent, ulong modulus)
        {
            if (modulus == 1)
                return 0;
            ulong result = 1;
            baseNum = baseNum % modulus;
            while (exponent > 0)
            {
                if (exponent % 2 == 1)
                    result = (result * baseNum) % modulus;
                exponent = exponent >> 1;
                baseNum = (baseNum * baseNum) % modulus;
            }
            return result;
        }
    }
}
