using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameWork.Data.Encryption.Exceptions
{
    internal class EncryptionOptionsException : Exception
    {
        public EncryptionOptionsException(string failedValidationsMessage) : base(failedValidationsMessage) { }
    }
}
