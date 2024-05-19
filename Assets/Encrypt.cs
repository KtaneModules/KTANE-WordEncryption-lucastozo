public class Encrypt
{
    public string EncryptString(char input, int offset = 0)
    {
        return (char)(((input - 'A' + offset + 26) % 26) + 'A');
    }
}