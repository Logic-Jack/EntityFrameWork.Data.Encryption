using EntityFrameWork.Data.Encryption.Attribute;
using EntityFrameWork.Data.Encryption.Converter;
using EntityFrameWork.Data.Encryption.Models;
using EntityFrameWork.Data.Encryption.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameWork.Data.Encryption.Extensions
{
    internal static class ModelBuilderExtensions
    {
        internal static ModelBuilder UseEncryption(this ModelBuilder modelBuilder, EncryptionOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            IDataProcessor dataProcessor = new DataProcessor(options);

            foreach (IMutableEntityType entityType in modelBuilder.Model.FindEntityTypes(typeof(IEncryptedModel)))
            {
                var properties = entityType.GetProperties();

                foreach (IMutableProperty property in properties.Where(p => p.ClrType == typeof(string) && !IsDiscriminatorOrNull(p)))
                {
                    object[] attributes = property.PropertyInfo!.GetCustomAttributes(typeof(EncryptPropertyAttribute), false);
                    if (attributes.Any())
                    {
                        if (attributes.FirstOrDefault(a => a.GetType() == typeof(EncryptPropertyAttribute)) is EncryptPropertyAttribute hasAttribute)
                        {
                            var sym = hasAttribute.IsSymmetric;
                            EncryptionConverter encryptionConverter = new(dataProcessor, sym);
                            property.SetValueConverter(encryptionConverter);
                        }
                    }
                }
            }

            return modelBuilder;
        }

        private static bool IsDiscriminatorOrNull(IMutableProperty property) => property.Name == "Discriminator" || property.PropertyInfo == null;
    }
}
