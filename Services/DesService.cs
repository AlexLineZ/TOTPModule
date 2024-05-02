using OTPModule.Dto;
using OTPModule.Services.IServices;

namespace OTPModule.Services;

public class DesService: IDesService
{
    private const int SizeOfBlock = 128;
    private const int SizeOfChar = 16;
    private const int ShiftKey = 2;
    private const int QuantityOfRounds = 16;
    string[] Blocks;
    
    public async Task<DesResponse> DesEncode(string input, string key)
    {
        return await ProcessDes(input, key, encode: true);
    }

    public async Task<DesResponse> DesDecode(string input, string key)
    {
        return await ProcessDes(input, key, encode: false);
    }

    private async Task<DesResponse> ProcessDes(string input, string key, bool encode)
    {
        input = StringToRightLength(input);
        CutStringIntoBlocks(input);
        key = CorrectKeyWord(key, input.Length / (2 * Blocks.Length));
        key = StringToBinaryFormat(key);

        for (int round = 0; round < QuantityOfRounds; round++)
        {
            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = encode ? EncodeDesOneRound(Blocks[i], key) : DecodeDesOneRound(Blocks[i], key);
            }
            key = encode ? KeyToNextRound(key) : KeyToPrevRound(key);
        }

        key = encode ? KeyToPrevRound(key) : KeyToNextRound(key);

        string result = CombineBlocks(Blocks);
        
        return new DesResponse
        {
            Output = StringFromBinaryToNormalFormat(result),
            Key = StringFromBinaryToNormalFormat(key)
        };
    }

    private string CombineBlocks(string[] blocks)
    {
        return string.Join("", blocks);
    }
    
    private string StringToRightLength(string input)
    {
        while (((input.Length * SizeOfChar) % SizeOfBlock) != 0)
            input += "#";

        return input;
    }
    
    private void CutStringIntoBlocks(string input)
    {
        Blocks = new string[(input.Length * SizeOfChar) / SizeOfBlock];

        int lengthOfBlock = input.Length / Blocks.Length;

        for (int i = 0; i < Blocks.Length; i++)
        {
            Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
            Blocks[i] = StringToBinaryFormat(Blocks[i]);
        }
    }
    
    private string StringToBinaryFormat(string input)
    {
        return string.Concat(input.Select(c => Convert.ToString(c, 2).PadLeft(SizeOfChar, '0')));
    }

    private string StringFromBinaryToNormalFormat(string input)
    {
        return Enumerable.Range(0, input.Length / SizeOfChar)
            .Select(i => Convert.ToChar(Convert.ToInt32(input.Substring(i * SizeOfChar, SizeOfChar), 2)).ToString())
            .Aggregate((x, y) => x + y);
    }

    private string CorrectKeyWord(string key, int requiredLength)
    {
        return key.Length > requiredLength ? key.Substring(0, requiredLength) : key.PadLeft(requiredLength, '0');
    }

    private string EncodeDesOneRound(string block, string key)
    {
        string L = block.Substring(0, block.Length / 2);
        string R = block.Substring(block.Length / 2);
        return R + XOR(L, F(R, key));
    }

    private string DecodeDesOneRound(string block, string key)
    {
        string L = block.Substring(0, block.Length / 2);
        string R = block.Substring(block.Length / 2);
        return XOR(F(L, key), R) + L;
    }

    private string F(string s1, string s2) => XOR(s1, s2);

    private string XOR(string s1, string s2)
    {
        return string.Concat(s1.Zip(s2, (c1, c2) => c1 == c2 ? '0' : '1'));
    }

    private string KeyToNextRound(string key)
    {
        return key.Substring(key.Length - ShiftKey) + key.Substring(0, key.Length - ShiftKey);
    }

    private string KeyToPrevRound(string key)
    {
        return key.Substring(ShiftKey) + key.Substring(0, ShiftKey);
    }
}