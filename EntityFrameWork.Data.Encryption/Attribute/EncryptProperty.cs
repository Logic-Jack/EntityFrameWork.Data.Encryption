namespace EntityFrameWork.Data.Encryption.Attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EncryptPropertyAttribute : System.Attribute
    {
        public bool IsSymmetric { get; set; }
        public EncryptPropertyAttribute(bool isSymmetric = false)
        {
            IsSymmetric = isSymmetric;
        }
    }
}
