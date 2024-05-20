public class Encrypt
{
    public string EncryptString(char input, int offset = 0)
    {
        string output = "";
        input = char.ToUpper(input);
        output += (char)(((input - 'A' + offset + 26) % 26) + 'A');
        return output;
    }
}