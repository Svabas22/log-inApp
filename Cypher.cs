using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace sifravimas2
{
    class Cypher
    {
        
        public static string Sha1(string text)
        {

            /*
                Slaptazodziui uzsifruoti naudojamas Sha1 algoritmas, pasinaudojus integruota SHA1 klase 
            */
            using (SHA1 sha1Hash = SHA1.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(text);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hash;
            }
            
        }

        public static void EncryptText(string text, string fileName)
        {
            /*
                Failui su varototoju duomenimis uzsifruoti naudojamas AES algoritmas, pasinaudojus integruota AES klase 
                Naudojamas simetrinis sifravimas
            */
            try
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    using (Aes aes = Aes.Create())
                    {
                        byte[] key = //sukuriamas AES raktas, toks pat raktas naudojamas ir atsifravimui
                        {
                              0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                              0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
                        };
                        aes.Key = key;

                        byte[] iv = aes.IV;  // Sukuriamas IV, kuris padeda tam paciam tekstui sugeneruoti vis nauja sifruota teksta
                        fileStream.Write(iv, 0, iv.Length);
                        using (CryptoStream cryptoStream = new CryptoStream(fileStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            using (StreamWriter encryptWriter = new StreamWriter(cryptoStream))
                            {
                                encryptWriter.WriteLine(text);
                            }
                        }
                    }
                }

                MessageBox.Show("User registered.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The encryption failed. {ex}");
            }
        }
        public static string[] DecryptData(string fileName)
        {
            // Failas "SifruotiDuomenys.dat" yra atidaromas ir issifruojamas
            try
            {
                var list = new List<string>();  //Sukuriamas sarasas i kuri bus dedamas issifruotas tekstas
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
                {
                    using (Aes aes = Aes.Create())
                    {
                        byte[] iv = new byte[aes.IV.Length];
                        int numBytesToRead = aes.IV.Length;
                        int numBytesRead = 0;
                        while (numBytesToRead > 0)
                        {
                            int n = fileStream.Read(iv, numBytesRead, numBytesToRead);
                            if (n == 0) break;

                            numBytesRead += n;
                            numBytesToRead -= n;
                        }

                        byte[] key =
                        {
                              0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                              0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
                        };
                        using (CryptoStream cryptoStream = new CryptoStream(fileStream,aes.CreateDecryptor(key, iv),CryptoStreamMode.Read))
                        {
                            using (StreamReader decryptReader = new StreamReader(cryptoStream))
                            {
                                string line;
                                while((line = decryptReader.ReadLine()) != null) //Issifruotas tekstas suskirstomas i atskiras linijas ir sudedamas i sarasa
                                {
                                    list.Add(line);
                                }
                                return list.ToArray();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] line = { ex.ToString() };
                return line;
            }
        }
    }
}
