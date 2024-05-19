public class Encrypt
{
    public string EncryptString(string input, int offset = 0, int variation = 0)
    {
        string output = "";
        foreach (char c in input)
        {
            if (c >= 'A' && c <= 'Z')
            {
                output += (char)(((c - 'A' + offset + 26) % 26) + 'A');
            }
            else
            {
                output += c;
            }
            offset += variation;
        }
        return output;
    }
}
