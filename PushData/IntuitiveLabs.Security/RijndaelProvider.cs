using System;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Summary description for RijsdaelProvider
/// </summary>
namespace IntuitiveLabs.Security
{
    public class RijndaelProvider
    {
        private Rijndael algorithm;

        public RijndaelProvider()
        {
            this.algorithm = RijndaelManaged.Create();
            this.ShowKey();
        }

        public RijndaelProvider(byte[] Key, byte[] IV)
        {
            this.Init(Key, IV);
        }

        public RijndaelProvider(string Key, string IV, ref RSACryptoServiceProvider Provider)
        {
            this.Init
                (
                    Provider.Decrypt(Convert.FromBase64String(Key), true),
                    Provider.Decrypt(Convert.FromBase64String(IV), true)
                );
        }

        public RijndaelProvider(string Key, string IV, ref RSAProvider Provider)
        {
            this.Init
                (
                    Provider.DecryptFromString(Key),
                    Provider.DecryptFromString(IV)
                );
        }

        public void ShowKey()
        {
            for (int i = 0; i < Key.Length; i++)
            {
                Console.WriteLine(Key[i].ToString());
            } 
        }

        private void Init(byte[] Key, byte[] IV)
        {
            this.algorithm = RijndaelManaged.Create();

            this.algorithm.Key = Key;
            this.algorithm.IV = IV;

            //this.ShowKey();
        }

        public void GenerateKey()
        {
            this.algorithm.GenerateKey();
        }

        public void GenerateIV()
        {
            this.algorithm.GenerateIV();
        }

        public byte[] Key
        {
            get { return this.algorithm.Key; }
            set { this.algorithm.Key = value; }
        }


        public byte[] IV
        {
            get { return this.algorithm.IV; }
            set { this.algorithm.IV = IV; }
        }

        public string Decrypt(string Text)
        {
            byte[] data = Convert.FromBase64String(Text);

            return this.Decrypt(data);
        }

        public string Decrypt(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(stream, this.algorithm.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                }

                return Encoding.Default.GetString(stream.ToArray());
            }
        }

        public byte[] Encrypt(string Text)
        {
            byte[] data = System.Text.Encoding.Default.GetBytes(Text);

            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(stream, this.algorithm.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                }

                return stream.ToArray();
            }
        }

        public string EncryptToString(string Text)
        {
            byte[] data = System.Text.Encoding.Default.GetBytes(Text);

            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(stream, this.algorithm.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                }

                return Convert.ToBase64String(stream.ToArray());
            }
        }
    }
}
