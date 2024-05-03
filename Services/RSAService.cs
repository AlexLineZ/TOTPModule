using OTPModule.Dto;
using OTPModule.Services.IServices;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace OTPModule.Services
{
    public class RSAService:IRSAService
    {
        Random _random;
        public RSAService(Random random) {
            _random = random;
        }

        public ulong GeneratePrime()
        {
            var key = RandomULong(2, 500);
            while (!IsPrime(key)) {
                key = RandomULong(2, 500);
            }
            return key;
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
                ulong a = RandomULong(2, n - 1);
                ulong x = ModPow(a, d, n);

                if (x == 1 || x == n - 1)
                    continue;

                bool isWitness = false;
                for (ulong j = 1; j < n - 1; j *= 2)
                {
                    x = (x * x) % n;
                    if (x == n - 1)
                    {
                        isWitness = true;
                        break;
                    }
                }
                if (!isWitness)
                    return false;
            }
            return true;
        }

        private ulong ModPow(ulong value, ulong exponent, ulong modulus)
        {
            if (modulus == 1) return 0;
            ulong result = 1;
            value = value % modulus;
            while (exponent > 0)
            {
                if (exponent % 2 == 1)
                {
                    result = (result * value) % modulus;
                }
                exponent = exponent/2;
                value = (value * value) % modulus;
            }
            return result;
        }

        public KeysDto GenerateKeys()
        {
            ulong p = GeneratePrime();
            ulong q = GeneratePrime();
            var n = p * q;
            var m = (p-1) * (q-1);
            ulong e = CalculateE(m);
            ulong d = CalculateD(e, m);

            var privateKey = new PrivateKeyDto()
            {
                D = d,
                N = n
            };

            var openKey = new OpenKeyDto()
            {
                E = e,
                N = n
            };

            return new KeysDto()
            {
                ClosedKey = privateKey,
                OpenKey = openKey
            };
        }

        public string Encode(EncodeMessageDto encodeMessageDto)
        {
            string result = "";

            BigInteger bi;

            for (int i = 0; i < encodeMessageDto.Message.Length; i++)
            {
                int index = encodeMessageDto.Message[i];

                bi = new BigInteger(index);
                bi = BigInteger.Pow(bi, (int)encodeMessageDto.OpenKey.E);

                BigInteger n_ = new(encodeMessageDto.OpenKey.N);

                bi = bi % n_;

                result+=(char)bi;
            }

            return result;
        }

        public string Decode(DecodeMessageDto decodeMessageDto)
        {
            string result = "";

            BigInteger bi;

            foreach (char item in decodeMessageDto.Message)
            {
                bi = new BigInteger((int)item);
                bi = BigInteger.Pow(bi, (int)decodeMessageDto.PrivateKey.D);

                BigInteger n_ = new BigInteger((int)decodeMessageDto.PrivateKey.N);

                bi = bi % n_;

                int index = Convert.ToInt32(bi.ToString());

                result += (char)index;
            }

            return result;
        }

        private ulong ModInverse(ulong a, ulong m)
        {
            ulong result = a % m;
            for (ulong x = 1; x < m; x++)
            {
                if ((result * x) % m == 1)
                {
                    return x;
                }
            }
            return 1;
        }

        private ulong CalculateD(ulong e, ulong m)
        {
            return ModInverse(e, m);
        }

        private ulong GCD(ulong a, ulong b)
        {
            while (b != 0)
            {
                ulong temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        private ulong RandomULong(ulong minValue, ulong maxValue)
        {
            ulong range = maxValue - minValue;
            var bytes = new byte[sizeof(ulong)];
            _random.NextBytes(bytes);
            ulong result = BitConverter.ToUInt64(bytes, 0);

            return (result % range) + minValue;
        }

        private ulong CalculateE(ulong m)
        {
            ulong e = RandomULong(2, m);

            while (GCD(e, m) != 1)
            {
                e = RandomULong(2, m);
            }

            return e;
        }

        public PrivateKeyDto GetPrivateKeyByPublic(OpenKeyDto openKey)
        {
            ulong e = openKey.E;
            ulong n = openKey.N;

            ulong d = 0;

            for (ulong i = 1; i < n; i++)
            {
                if ((i * e) % EulerPhi(n) == 1)
                {
                    d = i;
                    break;
                }
            }

            return new PrivateKeyDto()
            {
                D = d,
                N = n
            };
        }

        private ulong EulerPhi(ulong n)
        {
            ulong result = n;
            for (ulong i = 2; i * i <= n; i++)
            {
                if (n % i == 0)
                {
                    while (n % i == 0)
                    {
                        n /= i;
                    }
                    result -= result / i;
                }
            }
            if (n > 1)
            {
                result -= result / n;
            }
            return result;
        }
    }
}
