namespace EntityFrameWork.Data.Encryption.Options
{
    internal class EncryptionOptions
    {
        public EncryptionOptions(string password)
        {
            Password = password;
        }
        public string Password { get; set; }
        /// <summary>
        /// byte array min length 128 bits(16 bytes), 192 bits(24 bytes), 256 bits(32 bytes). Changing this value will override the default fix IV
        /// </summary>
        public byte[] FixSalt { get; set; } = new byte[] { 0xea, 0xdf, 0xc1, 0x84, 0xc2, 0xf5, 0x42, 0xff, 0x20, 0x76, 0x59, 0x72, 0xfa, 0xb2, 0xae, 0x01, 0x36, 0x32, 0x47, 0x7b, 0xa7, 0x40, 0x65, 0xcf, 0x96, 0x72, 0x7c, 0x77, 0xf8, 0x9c, 0x6c, 0x0e };

        /// <summary>
        /// byte array of 128bits(16 bytes). Changing this value will override the default fix IV
        /// </summary>
        public byte[] IV { get; set; } = new byte[] { 0x07, 0xca, 0x28, 0x0b, 0x4b, 0xd8, 0x97, 0x5e, 0xd1, 0xab, 0xb0, 0x3a, 0xc2, 0xa7, 0xeb, 0xf1 };

        /// <summary>
        /// KeySize => 128 or 256. Default 256. Changing this value require you to also override fixSalt, SaltLength to match the keysize
        /// </summary>
        public int KeySize { get; set; } = 256;

        /// <summary>
        /// this is used for the random salt generator default = 32 bytes. if you change this value it should match te keysize
        /// </summary>
        public int SaltLength { get; set; } = 32;

        /// <summary>
        /// This is used for the random IV generator default 16 bytes
        /// </summary>
        public int IVLength { get; set; } = 16;
    }
}
