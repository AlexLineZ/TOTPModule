namespace OTPModule.Services.IServices;

using System.Text;

public class MD5Service: IMD5Service
{
    private static readonly int MaxAttempts = 10000000;
    private static readonly Random random = new Random();
    
    uint A = 0x67452301;
    uint B = 0xEFCDAB89;
    uint C = 0x98BADCFE;
    uint D = 0x10325476;
    
    public string MD5Encrypt(string message)
    {
        A = 0x67452301;
        B = 0xEFCDAB89;
        C = 0x98BADCFE;
        D = 0x10325476;
        
        byte[] inputBytes = Encoding.UTF8.GetBytes(message);
        inputBytes = PadBytes(inputBytes);
        ProcessBlocks(inputBytes);
        return GetHashString();
    }

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

    private string GetHashString()
    {
        return $"{A:X8}{B:X8}{C:X8}{D:X8}".ToLower();
    }
    
    private byte[] PadBytes(byte[] input)
    {
        long originalLength = input.Length * 8;
        
        int paddingSize = 1;
        int totalLength = (input.Length + paddingSize + 8);
        int mod = totalLength % 64;
        if (mod != 0)
        {
            paddingSize += 64 - mod;
        }
        
        byte[] paddedInput = new byte[input.Length + paddingSize];
        Buffer.BlockCopy(input, 0, paddedInput, 0, input.Length);
        paddedInput[input.Length] = 0x80;
        
        byte[] lengthBytes = BitConverter.GetBytes(originalLength);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(lengthBytes);
        }
        Buffer.BlockCopy(lengthBytes, 0, paddedInput, paddedInput.Length - 8, 8);

        return paddedInput;
    }
    
    private void ProcessBlocks(byte[] inputBytes)
    {
        for (int i = 0; i < inputBytes.Length / 64; i++)
        {
            uint[] X = new uint[16];
            for (int j = 0; j < 16; j++)
            {
                X[j] = BitConverter.ToUInt32(inputBytes, i * 64 + j * 4);
            }

            uint AA = A, BB = B, CC = C, DD = D;
            
            for (int j = 0; j < 16; j++)
            {
                uint k = (uint)j;
                uint s = new uint[] { 7, 12, 17, 22 }[j % 4];
                AA = RotateLeft(AA + F(BB, CC, DD) + X[k] + T[j], (int)s) + BB;
                (AA, BB, CC, DD) = (DD, AA, BB, CC);
            }
            
            for (int j = 16; j < 32; j++)
            {
                uint k = (uint)((5 * j + 1) % 16);
                uint s = new uint[] { 5, 9, 14, 20 }[(j - 16) % 4];
                AA = RotateLeft(AA + G(BB, CC, DD) + X[k] + T[j], (int)s) + BB;
                (AA, BB, CC, DD) = (DD, AA, BB, CC);
            }
            
            for (int j = 32; j < 48; j++)
            {
                uint k = (uint)((3 * j + 5) % 16);
                uint s = new uint[] { 4, 11, 16, 23 }[(j - 32) % 4];
                AA = RotateLeft(AA + H(BB, CC, DD) + X[k] + T[j], (int)s) + BB;
                (AA, BB, CC, DD) = (DD, AA, BB, CC);
            }
            
            for (int j = 48; j < 64; j++)
            {
                uint k = (uint)((7 * j) % 16);
                uint s = new uint[] { 6, 10, 15, 21 }[(j - 48) % 4];
                AA = RotateLeft(AA + I(BB, CC, DD) + X[k] + T[j], (int)s) + BB;
                (AA, BB, CC, DD) = (DD, AA, BB, CC);
            }

            A += AA;
            B += BB;
            C += CC;
            D += DD;
        }
    }
    
    private uint F(uint x, uint y, uint z) => (x & y) | (~x & z);
    private uint G(uint x, uint y, uint z) => (x & z) | (y & ~z);
    private uint H(uint x, uint y, uint z) => x ^ y ^ z;
    private uint I(uint x, uint y, uint z) => y ^ (x | ~z);
    
    private uint RotateLeft(uint x, int n) => (x << n) | (x >> (32 - n));
    
    private readonly uint[] T = Enumerable.Range(1, 64).Select(i => (uint)(Math.Pow(2, 32) * Math.Abs(Math.Sin(i)))).ToArray();

    private static string GetRandomString()
    {
        byte[] bytes = new byte[16];
        random.NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}