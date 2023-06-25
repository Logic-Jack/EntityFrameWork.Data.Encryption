using EntityFrameWork.Data.Encryption.Options;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFrameWork.Data.Encryption.Converter
{
    internal class EncryptionConverter : ValueConverter<string, string>
    {
        public EncryptionConverter(IDataProcessor processor, bool isSymmetric, ConverterMappingHints? mappingHints = null)
            : base(x => processor.Encrypt(x, isSymmetric), x => processor.Decrypt(x), mappingHints)
        {
        }
    }
}
