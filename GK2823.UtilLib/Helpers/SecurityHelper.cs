using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GK2823.UtilLib.Helpers
{
    public class SecurityHelper
    {
        public static string CreatePwd(string cleanString)
        {
            byte[] KEY_64 = { 191, 94, 06, 11, 20, 182, 03, 26 };
            byte[] IV_64 = { 53, 87, 14, 14, 31, 142, 254, 142 };
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, provider.CreateEncryptor(KEY_64, IV_64), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(cs);
            sw.Write(cleanString);
            sw.Flush();
            cs.FlushFinalBlock();
            ms.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, int.Parse((ms.Length.ToString())));
        }

        public static string DesPwd(string encryptedString)
        {
            try
            {
                byte[] KEY_64 = { 191, 94, 06, 11, 20, 182, 03, 26 };
                byte[] IV_64 = { 53, 87, 14, 14, 31, 142, 254, 142 };
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                byte[] buffer = Convert.FromBase64String(encryptedString);
                MemoryStream ms = new MemoryStream(buffer);
                CryptoStream cs = new CryptoStream(ms, provider.CreateDecryptor(KEY_64, IV_64), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return sr.ReadToEnd();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
