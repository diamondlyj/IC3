using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace IntuitiveLabs.Security
{
    public class RSAProvider
    {
        private RSACryptoServiceProvider provider;
        
        private enum IdentifierType
        {
            RSAKey, ContainerName
        }

        private RSAProvider( string Identifier, IdentifierType Type )
        {
            if (Type == IdentifierType.ContainerName)
                LoadFromContainer(Identifier);
            else
                LoadFromKey(Identifier);
        }

        private void LoadFromContainer(string ContainerName)
        {
            CspParameters cspParams = new CspParameters();
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.KeyContainerName = ContainerName;

            this.provider = new RSACryptoServiceProvider(cspParams);
            this.provider.PersistKeyInCsp = true;
        }

        public void LoadFromKey(string Key)
        {
            CspParameters cspParams = new CspParameters();
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;

            this.provider = new RSACryptoServiceProvider(cspParams);
            this.provider.FromXmlString(Key);
        }

        public string PublicKey
        {
            get { return this.provider.ToXmlString(false); }
        }

        public byte[] Decrypt( byte[] cipher )
        {
            return this.provider.Decrypt( cipher, true);
        }

        public byte[] DecryptFromString(string text)
        {
            return this.Decrypt(Convert.FromBase64String(text));
        }

        public string DecryptToString( string text)
        {
            byte[] decipher = this.Decrypt(Convert.FromBase64String(text));            
            return Encoding.Default.GetString(decipher);
        }

        public byte[] Encrypt(byte[] cipher)
        {
            return this.provider.Encrypt(cipher, true);
        }

        public byte[] EncryptFromString(string text)
        {
            return this.Encrypt(Encoding.Default.GetBytes(text));
        }

        public string EncryptToString( string text)
        {
            return Convert.ToBase64String(this.EncryptFromString(text));
        }

        public static RSAProvider CreateFromName( string Name )
        {
            return new RSAProvider(Name, IdentifierType.ContainerName);
        }

        public static RSAProvider CreateFromKey(string Key)
        {
            return new RSAProvider(Key, IdentifierType.RSAKey);
        }
    }
}
