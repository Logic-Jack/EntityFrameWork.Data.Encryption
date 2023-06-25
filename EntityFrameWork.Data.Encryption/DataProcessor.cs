using EntityFrameWork.Data.Encryption.Exceptions;
using EntityFrameWork.Data.Encryption.Options;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace EntityFrameWork.Data.Encryption
{
    internal sealed class DataProcessor : IDataProcessor
    {
        private EncryptionOptions _encryptionOptions;        
        public DataProcessor(EncryptionOptions options)
        {
            _encryptionOptions = options;
        }

        public string Encrypt(string value, bool isSymmetric)
            => EncryptProperty(value, _encryptionOptions, isSymmetric);

        public string Decrypt(string value)
            => DecryptProperty(value, _encryptionOptions);

        private static string EncryptProperty(string value, EncryptionOptions options, bool isSymmetric)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            byte[] saltStringBytes = isSymmetric ? options.FixSalt : GenerateBitsOfRandomEntropy(options.SaltLength);
            byte[] ivStringBytes = isSymmetric ? options.IV : GenerateBitsOfRandomEntropy(options.IV.Length);

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(value);

            string password = string.IsNullOrWhiteSpace(options.Password) ? throw new ArgumentNullException("Password was null or white space") : options.Password;
            byte[] keyBytes = GenerateRfc2898DeriveBytesArray(password, saltStringBytes, saltStringBytes.Length);

            using (ICryptoTransform encryptor = GetEncryptor(keyBytes, ivStringBytes, options.KeySize))
            {
                using (MemoryStream memoryStream = new())
                {
                    using (CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();

                        byte[] cipherTextBytes = saltStringBytes;
                        cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                        cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                        memoryStream.Close();
                        cryptoStream.Close();

                        return Convert.ToBase64String(cipherTextBytes);
                    };
                };
            };
        }

        private static string DecryptProperty(string value, EncryptionOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            int saltLength = options.FixSalt.Length;
            int ivLength = options.IV.Length;

            byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(value);
            byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(saltLength).ToArray();
            byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(saltLength).Take(ivLength).ToArray();
            byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(saltLength + ivLength).ToArray();

            string password = string.IsNullOrWhiteSpace(options.Password) ? throw new ArgumentNullException("Password was null or white space") : options.Password;

            byte[] keyBytes = GenerateRfc2898DeriveBytesArray(password, saltStringBytes, saltStringBytes.Length);
            using (ICryptoTransform decryptor = GetDecryptor(keyBytes, ivStringBytes, saltStringBytes.Length))
            {
                using (MemoryStream memoryStream = new(cipherTextBytes))
                {
                    using (CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        Span<byte> plainTextBytes = new byte[cipherTextBytes.Length];
                        int decryptedByteCount = 0;
                        while (decryptedByteCount < plainTextBytes.Length)
                        {
                            int readedBytes = cryptoStream.Read(plainTextBytes.Slice(decryptedByteCount));
                            if (readedBytes == 0)
                                break;
                            decryptedByteCount += readedBytes;
                        }

                        memoryStream.Close();
                        cryptoStream.Close();

                        return Encoding.UTF8.GetString(plainTextBytes.ToArray(), 0, decryptedByteCount);
                    };

                };
            };
        }

        private static byte[] GenerateBitsOfRandomEntropy(int byteSize)
        {
            byte[] randomBytes = new byte[byteSize];
            using (var rngCsp = RandomNumberGenerator.Create())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        private static byte[] GenerateRfc2898DeriveBytesArray(string password, byte[] salt, int outputSize)
        {
            using Rfc2898DeriveBytes generatedPassword = new(password, salt);
            return generatedPassword.GetBytes(outputSize);
        }

        private static ICryptoTransform GetEncryptor(byte[] key, byte[] iv, int keysize)
        {
            using Aes aes = Aes.Create();
            aes.KeySize = keysize;
            return aes.CreateEncryptor(key, iv);
        }

        private static ICryptoTransform GetDecryptor(byte[] key, byte[] iv, int keysize)
        {
            using Aes aes = Aes.Create();
            aes.KeySize = keysize;
            return aes.CreateDecryptor(key, iv);
        }

        private static void ValidateOptions(EncryptionOptions options)
        {
            List<string> validations = new List<string>();

            if (!(options.KeySize == 128 || options.KeySize == 256))
            {
                throw new EncryptionOptionsException($"invalid keysize must be 128 or 256 but {options.KeySize} was provided");
            }

            if (options.IV.Length != 16)
            {
                validations.Add("IV array has a incorrect length - should be 16 bytes long");
            }

            if (options.FixSalt.Length != (options.KeySize / 8))
            {

            }
        } 
    }
}

