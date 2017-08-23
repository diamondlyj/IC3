using System;
using System.Data;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for RijsdaelProvider
/// </summary>
public class RijsdaelProvider
{
    public Rijndael Algorithm;

	public RijsdaelProvider()
	{
        this.Algorithm = RijndaelManaged.Create();        
	}

	public RijsdaelProvider( byte[] Key, byte[] IV )
	{
        this.Init(Key, IV);
	}

    public RijsdaelProvider( string Key, string IV, ref RSACryptoServiceProvider Provider )
    {
        this.Init
            (
                Provider.Decrypt(Convert.FromBase64String(Key), true),
                Provider.Decrypt(Convert.FromBase64String(IV), true)
            );
    }

    private void Init(byte[] Key, byte[] IV)
    {
        this.Algorithm = RijndaelManaged.Create();
        this.Algorithm.Key = Key;
        this.Algorithm.IV = IV;
    }

    public void Clear()
    {
        this.Algorithm.Clear();
    }

    public string Decrypt( string Text )
    {
        byte[] data = Convert.FromBase64String( Text );

        using (MemoryStream stream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(stream, this.Algorithm.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                cryptoStream.Close();
            }

            string str = Encoding.Default.GetString(stream.ToArray());

            stream.Close();

            return str;
        }        
    }

    public string Encrypt( string Text )
    {
        byte[] data = System.Text.Encoding.Default.GetBytes(Text);

        using (MemoryStream stream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(stream, this.Algorithm.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                cryptoStream.Close();
            }

            string str = Convert.ToBase64String(stream.ToArray());

            return str;
        }
    }
}
