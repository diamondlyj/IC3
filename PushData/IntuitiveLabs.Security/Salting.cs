using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IntuitiveLabs.Security
{
    public class Salt
    {
        public static bool AreIdentical(byte[] Array1, byte[] Array2)
        {
            if (Array1.Length == Array2.Length)
            {
                for (int i = 0; i < Array2.Length; i++)
                {
                    if (Array1[i] != Array2[i])
                        return false;
                }
            }
            else
                return false;

            return true;
        }

        public static byte[] Create(int Size)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[Size];
            provider.GetBytes(buffer);

            return buffer;
        }

        public static byte[] CreateHash(byte[] Cipher, byte[] Salt)
        {
            SHA1Managed sha = new SHA1Managed();

            byte[] buffer = new byte[Cipher.Length + Salt.Length];

            for (int i = 0; i < Cipher.Length; i++)
                buffer[i] = Cipher[i];

            for (int i = 0; i < Salt.Length; i++)
                buffer[Cipher.Length + i] = Salt[i];

            return sha.ComputeHash(buffer);
        }
    }
}
