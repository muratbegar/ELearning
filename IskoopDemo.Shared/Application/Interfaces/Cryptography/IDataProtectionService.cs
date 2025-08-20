using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Interfaces.Cryptography
{
    public interface IDataProtectionService
    {
        string Protect(string plainText, string purpose = null);
        string Unprotect(string protectedText, string purpose = null);
        byte[] Protect(byte[] plainData, string purpose = null);
        byte[] Unprotect(byte[] protectedData, string purpose = null);
    }
}
