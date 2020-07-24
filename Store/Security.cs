using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Store
{
    internal static class Security
    {
        public static (string, string) EncryptUserData(string login, string pass)
        {
            try
            {
                var byteConverter = new UnicodeEncoding();

                byte[] loginByte;
                byte[] passByte;
                string encryptedPassString;
                byte[] encryptedPass;
                byte[] keyForPass;
                byte[] ivForPass;

                loginByte = byteConverter.GetBytes(login);
                passByte = byteConverter.GetBytes(pass);
                using (var sha = SHA256.Create())
                {
                    keyForPass = sha.ComputeHash(loginByte);
                }

                using (var md5 = MD5.Create())
                {
                    ivForPass = md5.ComputeHash(passByte);
                }

                using (var myAes = new AesCryptoServiceProvider())
                {
                    myAes.Key = keyForPass;
                    myAes.IV = ivForPass;
                    encryptedPass = EncryptStringToBytes_Aes(pass, myAes.Key, myAes.IV);
                    encryptedPassString = Convert.ToBase64String(encryptedPass);
                }

                return (login, encryptedPassString);
            }
            catch (Exception)
            {
                return (string.Empty, string.Empty);
            }
        }

        public static bool VerifyPassword(string inLogin, string inPass, string pass)
        {
            try
            {
                var byteConverter = new UnicodeEncoding();

                byte[] encryptedPass;
                byte[] keyForPass;
                byte[] ivForPass;

                using (var sha = SHA256.Create())
                {
                    keyForPass = sha.ComputeHash(byteConverter.GetBytes(inLogin));
                }

                using (var md5 = MD5.Create())
                {
                    ivForPass = md5.ComputeHash(byteConverter.GetBytes(inPass));
                }

                using (var myAes = new AesCryptoServiceProvider())
                {
                    myAes.Key = keyForPass;
                    myAes.IV = ivForPass;

                    encryptedPass = EncryptStringToBytes_Aes(inPass, myAes.Key, myAes.IV);
                }
                return Convert.ToBase64String(encryptedPass) == pass;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetDecryptedPassword(string inLogin, string inPass, string pass)
        {
            try
            {
                var byteConverter = new UnicodeEncoding();

                byte[] keyForPass;
                byte[] ivForPass;
                string decPass;

                using (var sha = SHA256.Create())
                {
                    keyForPass = sha.ComputeHash(byteConverter.GetBytes(inLogin));
                }

                using (var md5 = MD5.Create())
                {
                    ivForPass = md5.ComputeHash(byteConverter.GetBytes(inPass));
                }

                using (var myAes = new AesCryptoServiceProvider())
                {
                    myAes.Key = keyForPass;
                    myAes.IV = ivForPass;
                    decPass = DecryptStringFromBytes_Aes(Convert.FromBase64String(pass),
                        myAes.Key, myAes.IV);
                }

                return decPass ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            using (var aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }

            return encrypted;
        }

        private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (var aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
