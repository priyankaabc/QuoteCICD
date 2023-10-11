using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Helper
{
    public class AESEncryptionDecryptionHelper
    {
        private static string secretKey = ConfigurationManager.AppSettings["SecretKey"].ToString();
        public static string Encrypt(string plaintext)
        {
            using (Aes aesAlg = Aes.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
                Array.Resize(ref keyBytes, 32);
                aesAlg.Key = keyBytes;
                
                aesAlg.GenerateIV();

                byte[] iv = aesAlg.IV;

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
                        csEncrypt.Write(plaintextBytes, 0, plaintextBytes.Length);
                    }

                    byte[] encryptedBytes = msEncrypt.ToArray();

                    // Combine IV and ciphertext with a delimiter
                    byte[] combined = new byte[iv.Length + encryptedBytes.Length];
                    Array.Copy(iv, 0, combined, 0, iv.Length);
                    Array.Copy(encryptedBytes, 0, combined, iv.Length, encryptedBytes.Length);

                    return Convert.ToBase64String(combined);
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            try
            {
                byte[] combined = Convert.FromBase64String(encryptedText);

                using (Aes aesAlg = Aes.Create())
                {
                    byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
                    Array.Resize(ref keyBytes, 32);
                    aesAlg.Key = keyBytes;
                    //aesAlg.Key = Encoding.UTF8.GetBytes(secretKey);

                    // Extract IV
                    byte[] iv = new byte[aesAlg.BlockSize / 8];
                    byte[] ciphertext = new byte[combined.Length - iv.Length];
                    Array.Copy(combined, 0, iv, 0, iv.Length);
                    Array.Copy(combined, iv.Length, ciphertext, 0, ciphertext.Length);

                    aesAlg.IV = iv;

                    using (MemoryStream msDecrypt = new MemoryStream(ciphertext))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                string decryptedText = srDecrypt.ReadToEnd();
                                return decryptedText;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle decryption error (e.g., log the error or return a specific value)
                Console.WriteLine("Decryption error: " + ex.Message);
                return null;
            }
        }


        public static byte[] GenerateIV()
        {
            byte[] ivBytes;
            using (var rng = new RNGCryptoServiceProvider())
            {
                ivBytes = new byte[16]; // 128 bits IV for AES
                rng.GetBytes(ivBytes);
                return ivBytes;
            }
        }
    }
}
