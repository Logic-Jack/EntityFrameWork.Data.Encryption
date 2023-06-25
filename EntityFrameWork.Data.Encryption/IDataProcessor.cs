namespace EntityFrameWork.Data.Encryption
{
    public interface IDataProcessor
    {
        string Encrypt(string value, bool isSymmetric);
        string Decrypt(string value);
    }
}