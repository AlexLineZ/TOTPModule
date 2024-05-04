namespace OTPModule.Services.IServices;

using System.Text;

public class MD5Service: IMD5Service
{
    private readonly uint[] shifts = { 7, 12, 17, 22, 5, 9, 14, 20, 4, 11, 16, 23, 6, 10, 15, 21 };
    private readonly uint[] T = Enumerable.Range(1, 64).Select(i => (uint)(Math.Pow(2, 32) * Math.Abs(Math.Sin(i)))).ToArray();
    private uint F(uint x, uint y, uint z) => (x & y) | (~x & z);
    private uint G(uint x, uint y, uint z) => (x & z) | (y & ~z);
    private uint H(uint x, uint y, uint z) => x ^ y ^ z;
    private uint I(uint x, uint y, uint z) => y ^ (x | ~z);

    public string MD5Encrypt(string message)
    {
        byte[] data = PrepareMessage(Encoding.ASCII.GetBytes(message));
        uint[] buffer = { 0x67452301, 0xefcdab89, 0x98badcfe, 0x10325476 };

        ProcessDataBlocks(data, buffer);

        return BitConverter.ToString(BitConverter.GetBytes(buffer[0])
            .Concat(BitConverter.GetBytes(buffer[1]))
            .Concat(BitConverter.GetBytes(buffer[2]))
            .Concat(BitConverter.GetBytes(buffer[3]))
            .ToArray()).Replace("-", "").ToLower();
    }

    private byte[] PrepareMessage(byte[] data)
    {
        int originalLengthBits = data.Length * 8;
        data = data.Concat(new byte[] { 0x80 }).ToArray();
        while ((data.Length * 8) % 512 != 448)
        {
            data = data.Concat(new byte[] { 0x00 }).ToArray();
        }

        return data.Concat(BitConverter.GetBytes((uint)originalLengthBits))
                   .Concat(new byte[4]).ToArray();
    }

    private void ProcessDataBlocks(byte[] data, uint[] buffer)
    {
        for (int k = 0; k < data.Length / 64; k++)
        {
            uint[] X = new uint[16];
            for (int j = 0; j < 16; j++)
            {
                X[j] = BitConverter.ToUInt32(data, (k * 64) + (j * 4));
            }

            uint[] ABCD = { buffer[0], buffer[1], buffer[2], buffer[3] };

            for (int i = 0; i < 64; i++)
            {
                uint f = 0;
                int g = 0;
                
                if (i < 16) { f = F(ABCD[1], ABCD[2], ABCD[3]); g = i; }
                else if (i < 32) { f = G(ABCD[1], ABCD[2], ABCD[3]); g = (5 * i + 1) % 16; }
                else if (i < 48) { f = H(ABCD[1], ABCD[2], ABCD[3]); g = (3 * i + 5) % 16; }
                else { f = I(ABCD[1], ABCD[2], ABCD[3]); g = (7 * i) % 16; }

                uint temp = ABCD[3];
                ABCD[3] = ABCD[2];
                ABCD[2] = ABCD[1];
                ABCD[1] += RotateLeft((ABCD[0] + f + X[g] + T[i]), (int)shifts[i % 16]);
                ABCD[0] = temp;
            }

            for (int i = 0; i < 4; i++)
            {
                buffer[i] += ABCD[i];
            }
        }
    }
    
    private static readonly int MaxAttempts = 10000000;
    private static readonly Random random = new Random();
    
    public string FindCollision()
    {
        Dictionary<string, string> hashes = new Dictionary<string, string>();
        
        for (int i = 0; i < MaxAttempts; i++)
        {
            string randomMessage = GetRandomString();
            string hash = MD5Encrypt(randomMessage);

            if (hashes.ContainsKey(hash))
            {
                return $"Collision found:\n" 
                       + $"Message 1: {hashes[hash]}\n" 
                       + $"Message 2: {randomMessage}\n" 
                       + $"Hash: {hash}\n";
            }
            hashes[hash] = randomMessage;
        }

        return "No collisions were found";
    }
    
    
    private uint RotateLeft(uint x, int n) => (x << n) | (x >> (32 - n));

    private static string GetRandomString()
    {
        byte[] bytes = new byte[16];
        random.NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}