using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptEncryption: IDisposable
    {
        public void Dispose()
        {
        }

        /// <summary>
        /// Encrypts a string (in memory) with your provided password and returns the result.
        /// </summary>
        /// <param name="data">String data to encrypt</param>
        /// <param name="password">Password to encrypt with.</param>
        /// <returns></returns>
        public string Encrypt(string data, string password)
        {
            return StringCipher.Encrypt(data, password);
        }

        /// <summary>
        /// Decrypts a string (in memory) with your provided password and returns the result.
        /// </summary>
        /// <param name="data">Encrypted string data to decrypt.</param>
        /// <param name="password">Password to decript with.</param>
        /// <returns></returns>
        public string Decrypt(string data, string password)
        {
            return StringCipher.Decrypt(data, password);
        }

        /// <summary>
        /// Encrypts file(s) with a password as new file(s) with an .enx extension.
        /// </summary>
        /// <param name="filemask">Filename which may contain wildcards, representing file(s) to encrypt.</param>
        /// <param name="password">Password to encrypt with</param>
        public void EncryptFiles(string filemask, string password)
        {
            string[] matchingFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), filemask);

            SymmetricAlgorithm sa = new RijndaelManaged();

            foreach (string filename in matchingFiles)
            {
                byte[] rgbIV = sa.IV;
                byte[] saltValueBytes = Encoding.ASCII.GetBytes("s0d1uMv4l" + password.Length.ToString());

                PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, saltValueBytes, "SHA1", 2);

                // Use the password to generate pseudo-random bytes for the encryption
                // key. Specify the size of the key in bytes (instead of bits).
                byte[] keyBytes = pdb.GetBytes(256 / 8);

                sa.Mode = CipherMode.CBC;

                byte[] fileBytes;
                string newFilename = filename + ".enx";

                // Encryption
                ICryptoTransform transform = sa.CreateEncryptor(keyBytes, rgbIV);
                using (Stream outputStream = new FileStream(newFilename, FileMode.Create))
                {
                    // put reversed iv array into first 16 bytes of output stream
                    byte[] revIV = rgbIV.Reverse().ToArray();
                    outputStream.Write(revIV, 0, 16);

                    // Wrap the output stream up with a CryptoStream
                    // which performs the data encryption
                    using (Stream cryptoStream = new CryptoStream(outputStream, transform, CryptoStreamMode.Write))
                    {
                        // Store data into the cryptoStream (which will encrypt it
                        // and then pass it along to our outputStream for storage)
                        using (BinaryWriter bw = new BinaryWriter(cryptoStream))
                        {

                            // Now that we have set up an output stream, let us pipe through it the unencrypted file bytes
                            using (FileStream fs = new FileStream(filename, FileMode.Open))
                            {
                                using (BinaryReader br = new BinaryReader(fs))
                                {
                                    //int chunkSize = 1024*1024; // 1m chunks

                                    fileBytes = br.ReadBytes(99999999);

                                    bw.Write(fileBytes);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Descrypts file(s) with a password as new file(s) without an .enx extension.
        /// </summary>
        /// <param name="filemask">Filename which may contain wildcards, representing file(s) to decrypt.</param>
        /// <param name="password">Password to decrypt with</param>
        public void DecryptFiles(string filemask, string password)
        {
            string[] matchingFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), filemask);

            SymmetricAlgorithm sa = new RijndaelManaged();

            foreach (string filename in matchingFiles)
            {
                string newFilename = filename.Replace(".enx", "");

                byte[] rgbIV = sa.IV;
                byte[] saltValueBytes = Encoding.ASCII.GetBytes("s0d1uMv4l" + password.Length.ToString());

                PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, saltValueBytes, "SHA1", 2);

                // Use the password to generate pseudo-random bytes for the encryption
                // key. Specify the size of the key in bytes (instead of bits).
                byte[] keyBytes = pdb.GetBytes(256 / 8);

                sa.Mode = CipherMode.CBC;

                byte[] fileBytes;

                // Decryption
                using (Stream inputStream = new FileStream(filename, FileMode.Open))
                {
                    // Before decrypting the stream get the initialization vector out of the first 16 chars.
                    byte[] iv = new byte[16];
                    inputStream.Read(iv, 0, 16);

                    byte[] riv = iv.Reverse().ToArray();
                    rgbIV = riv;

                    ICryptoTransform transform = sa.CreateDecryptor(keyBytes, rgbIV);
                    using (Stream cryptoStream = new CryptoStream(inputStream, transform, CryptoStreamMode.Read))
                    {
                        // Read data into the cryptoStream (which will fetch encrypted
                        // data from the inputStream and then decrypt it before returning
                        // it to us)
                        using (BinaryReader br = new BinaryReader(cryptoStream))
                        {
                            // Now that we have set up an decryption output stream, let us pipe through it the not-yet-decrypted input stream
                            using (FileStream fs = new FileStream(newFilename, FileMode.Create))
                            {
                                using (BinaryWriter bw = new BinaryWriter(fs))
                                {
                                    //int chunkSize = 1024 * 1024; // 1m chunks

                                    fileBytes = br.ReadBytes(99999999);

                                    bw.Write(fileBytes);
                                }
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Private helper method to hex encode bytes.
        /// </summary>
        /// <param name="bytes">Bytes to encode.</param>
        /// <returns>Hex encoded string representation of bytes.</returns>
        private string hexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Creates an md5 hash of a file stored on disk.
        /// </summary>
        /// <param name="filename">The filename to generate hash for.</param>
        /// <returns>Base64 string representing the md5 hash generated.</returns>
        public string GetBase64EncodedMD5Hash(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (MD5Cng md5 = new MD5Cng())
                {
                    return hexStringFromBytes(md5.ComputeHash(fs));
                }
            }
        }

        /// <summary>
        /// Creates a sha1 hash of a file stored on disk.
        /// </summary>
        /// <param name="filename">The filename to generate hash for.</param>
        /// <returns>Base64 string representing the sh1 hash generated.</returns>
        public string GetBase64EncodedSHA1Hash(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    return hexStringFromBytes(sha1.ComputeHash(fs));
                }
            }
        }

        /// <summary>
        /// Creates a sha256 hash of a file stored on disk.
        /// </summary>
        /// <param name="filename">The filename to generate hash for.</param>
        /// <returns>Base64 string representing the sha256 hash generated.</returns>
        public string GetBase64EncodedSHA256Hash(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (SHA256Managed sha256 = new SHA256Managed())
                {
                    return hexStringFromBytes(sha256.ComputeHash(fs));
                }
            }
        }

        /// <summary>
        /// Generates variety of hashes for file(s) represented by filemask.
        /// </summary>
        /// <param name="path">Directory to look in for files to hash.</param>
        /// <param name="searchPattern">Filename or wildcard of file(s) to hash.</param>
        /// <returns>Json encoded list of objects containing hash info.</returns>
        public string HashFiles(string path, string searchPattern)
        {
            string[] matchingFiles = Directory.GetFiles(path, searchPattern);

            List<dynamic> hashInfo = new List<dynamic>();

            foreach (string filename in matchingFiles)
            {
                var fileHashes = new
                {
                    name = filename,
                    md5 = GetBase64EncodedMD5Hash(filename),
                    sha1 = GetBase64EncodedSHA1Hash(filename),
                    sha256 = GetBase64EncodedSHA256Hash(filename)
                };

                hashInfo.Add(fileHashes);
            }

            var json = JsonConvert.SerializeObject(hashInfo.ToArray());

            return json;
        }

    }
}
