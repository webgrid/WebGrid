/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/


#region Header

/*
Copyright ©  Olav Christian Botterli. 

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/

#endregion Header

namespace WebGrid.Util
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    /// <summary>
    /// WebGrid has builtin support for encryption and decryption of text strings. <see cref="WebGrid.Text"/> column uses this feature and supports SHA1 and MD5 Algorithm.
    /// </summary>
    public class Security
    {
        #region Fields

        /*
        /// <summary>
        /// Generates a licensekey based on the supplied index key.
        /// </summary>
        /// <param name="key">The index key to use for generating the licensekey.</param>
        internal static string GenerateInternalLicenseKey( Int64 key )
        {
            string code = string.Empty;
            Int64 crc = 0;
            Int64 crc2;
            Int64 diff;
            string last;
            lock(randomGeneratorLock)
            {
                for(int i = 0; i < 2; i++)
                {
                    int nextchar = randomGenerator.Next(0, validChars.Length - 1);
                    crc += nextchar * (Int64) Math.Pow(validChars.Length, 1 - (i % 2));
                    code += validChars[nextchar];
                }
                crc2 = crc % (Int64) Math.Pow(validChars.Length, 2);
                diff = (key - crc2 + (Int64) Math.Pow(validChars.Length, 2)) % (Int64) Math.Pow(validChars.Length, 2);
                last = string.Empty;
                for(int i = 0; i < 2; i++)
                {
                    int nextchar = (int) (diff % validChars.Length);
                    diff = (diff - nextchar) / validChars.Length;
                    last = validChars[nextchar] + last;
                }
            }

            code += last;
            return code;
        }
        */
        private static readonly Random randomGenerator = new Random();
        private static readonly object randomGeneratorLock = new object();
        private static readonly object validCharsLock = new object();

        private const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        #endregion Fields

        #region Enumerations

        internal enum HashAlgorithm
        {
            SHA1 = 0,
            MD5 = 1
        }

        internal enum KeySize
        {
            Size64 = 64,
            Size128 = 128,
            Size192 = 192,
            Size256 = 256
        }

        #endregion Enumerations

        #region Properties

        internal static bool IsLocal
        {
            get
            {

                return HttpContext.Current == null || HttpContext.Current.Request == null ||
                       HttpContext.Current.Request.IsLocal;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Decrypts a previously encrypted text. The key is located in web.config as "WGKey".
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <returns>The decrypted/plain text.</returns>
        public static string Decrypt(string cipherText)
        {
            return ConfigurationManager.AppSettings["WGKey"] == null ? null :
                Decrypt(cipherText, ConfigurationManager.AppSettings["WGKey"]);
        }

        /// <summary>
        /// Decrypts a previously encrypted text.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="key">The key ("password") to use.</param>
        /// <returns>The decrypted/plain text.</returns>
        public static string Decrypt(string cipherText, string key)
        {
            return Decrypt(cipherText, key, "n@va1u", HashAlgorithm.SHA1, 3, KeySize.Size128);
        }

        /// <summary>
        /// Encrypts a string using a key. The key is located in web.config as "WGKey".
        /// </summary>
        /// <param name="plainText">The text that should be encrypted.</param>
        /// <returns>The encrypted text.</returns>
        public static string Encrypt(string plainText)
        {
            return ConfigurationManager.AppSettings["WGKey"] == null ? null :
                Encrypt(plainText, ConfigurationManager.AppSettings["WGKey"]);
        }

        /// <summary>
        /// Encrypts a string using a key.
        /// </summary>
        /// <param name="plainText">The text that should be encrypted.</param>
        /// <param name="key">The key ("password") to use.</param>
        /// <returns>The encrypted text.</returns>
        public static string Encrypt(string plainText, string key)
        {
            return Encrypt(plainText, key, "n@va1u", HashAlgorithm.SHA1, 3, KeySize.Size128);
        }

        /// <summary>
        /// Generates a license key based on the supplied index key.
        /// </summary>
        /// <param name="key">The index key to use for generating the license key.</param>
        public static string GenerateLicenseKey(Int64 key)
        {
            StringBuilder codeBuilder = new StringBuilder();
            Int64 crc = 0;
            StringBuilder lastBuilder;
            lock (randomGeneratorLock)
            {
                for (int i = 0; i < 8; i++)
                {
                    int nextchar = randomGenerator.Next(0, validChars.Length - 1);
                    crc += nextchar*(Int64) Math.Pow(validChars.Length, 7 - (i%8));
                    codeBuilder.Append(validChars[nextchar]);
                    if (i%4 == 3)
                        codeBuilder.Append("-");
                }
                long crc2 = crc%(Int64) Math.Pow(validChars.Length, 8);
                long diff = (key - crc2 + (Int64) Math.Pow(validChars.Length, 8))%(Int64) Math.Pow(validChars.Length, 8);
                lastBuilder = new StringBuilder(string.Empty);
                for (int i = 0; i < 8; i++)
                {
                    if (i == 4)
                        lastBuilder.Insert(0, "-");
                    int nextchar = (int) (diff%validChars.Length);
                    diff = (diff - nextchar)/validChars.Length;
                    lastBuilder.Insert(0, validChars[nextchar]);
                }
            }

            codeBuilder.Append(lastBuilder.ToString());
            return codeBuilder.ToString();
        }

        /// <summary>
        /// Verifies a licensekey based on the supplied index key.
        /// </summary>
        /// <param name="key">The index key to verify against.</param>
        /// <param name="code">The code supplied by the user.</param>
        public static bool VerifyLicenseKey(Int64 key, string code)
        {
            code = code.Replace("-", string.Empty).ToUpperInvariant();
            if (code.Length != 16)
                return false;

            Int64 crc = 0;
            lock (validCharsLock)
            {
                for (int i = 0; i < 16; i++)
                {
                    int pos = validChars.IndexOf(code[i]);
                    if (pos < 0)
                        return false;
                    crc += pos*(Int64) Math.Pow(validChars.Length, 7 - (i%8));
                }
                return (key == crc%(Int64) Math.Pow(validChars.Length, 8));
            }
        }

        /// <summary>
        /// Verifies a licensekey based on the supplied index key.
        /// </summary>
        /// <param name="key">The index key to verify against.</param>
        /// <param name="code">The code supplied by the user.</param>
        internal static bool VerifyInternalLicenseKey(Int64 key, string code)
        {
            code = code.Replace("-", string.Empty).ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            if (code.Length != 4)
                return false;

            Int64 crc = 0;
            lock (validCharsLock)
            {
                for (int i = 0; i < 4; i++)
                {
                    int pos = validChars.IndexOf(code[i]);
                    if (pos < 0)
                        return false;
                    crc += pos*(Int64) Math.Pow(validChars.Length, 1 - (i%2));
                }
                return (key == crc%(Int64) Math.Pow(validChars.Length, 2));
            }
        }

        internal static string deCodeKey(string key)
        {
            StringBuilder expiresdateBuilder = new StringBuilder();
            foreach (string block in key.Split('-'))
            {
                int i = 0;
                while (i <= 31)
                {
                    if (Equals(VerifyInternalLicenseKey(i, block), true))
                    {
                        if (i == 0)
                            expiresdateBuilder.Append("00");
                        else if (i < 10)
                            expiresdateBuilder.AppendFormat("0{0}", i);
                        else
                            expiresdateBuilder.Append(i.ToString());
                        break;
                    }
                    i++;
                }
            }
            return expiresdateBuilder.ToString();
        }

        /// <SUMMARY>
        /// Decrypts specified ciphertext using Rijndael symmetric key algorithm.
        /// </SUMMARY>
        /// <PARAM name="cipherText">
        /// Base64-formatted ciphertext value.
        /// </PARAM>
        /// <PARAM name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived. The
        /// derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that this
        /// passphrase is an ASCII string.
        /// </PARAM>
        /// <PARAM name="saltValue">
        /// Salt value used along with passphrase to generate password. Salt can
        /// be any string. In this example we assume that salt is an ASCII string.
        /// </PARAM>
        /// <PARAM name="hashAlgorithm">
        /// Hash algorithm used to generate password. Allowed values are: "MD5" and
        /// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
        /// </PARAM>
        /// <PARAM name="passwordIterations">
        /// Number of iterations used to generate password. One or two iterations
        /// should be enough.
        /// </PARAM>
        /// <PARAM name="initVector">
        /// Initialization vector (or IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long.
        /// </PARAM>
        /// <PARAM name="keySize">
        /// Size of encryption key in bits. Allowed values are: 128, 192, and 256.
        /// Longer keys are more secure than shorter keys.
        /// </PARAM>
        /// <RETURNS>
        /// Decrypted string value.
        /// </RETURNS>
        /// <REMARKS>
        /// Most of the logic in this function is similar to the Encrypt
        /// logic. In order for decryption to work, all parameters of this function
        /// - except cipherText value - must match the corresponding parameters of
        /// the Encrypt function which was called to generate the
        /// ciphertext.
        /// </REMARKS>
        private static string Decrypt(string cipherText,
            string passPhrase,
            string saltValue,
            HashAlgorithm hashAlgorithm,
            int passwordIterations,
            KeySize keySize)
        {
            const string initVector = "@4E1D4A5C3E2g2e7";

            string _hashAlgorithm = hashAlgorithm == HashAlgorithm.MD5 ? "MD5" : "SHA1";

            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            // First, we must create a password, from which the key will be
            // derived. This password will be generated from the specified
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                passPhrase,
                saltValueBytes,
                _hashAlgorithm,
                passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            #pragma warning disable 618,612
            byte[] keyBytes = password.GetBytes((int) keySize/8);
            #pragma warning restore 618,612

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged {Mode = CipherMode.CBC};

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.

            // Generate decryptor from the existing key bytes and initialization
            // vector. Key size will be defined based on the number of the key
            // bytes.
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                keyBytes,
                initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Define cryptographic stream (always use Read mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         decryptor,
                                                         CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            // Start decrypting.
            int decryptedByteCount = 0;
            bool status = true;
            try
            {
                decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            }
            catch
            {
                status = false;
            }

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            if (status == false)
                return null;
            // Convert decrypted data into a string.
            // Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = Encoding.UTF8.GetString(plainTextBytes,
                                                       0,
                                                       decryptedByteCount);

            // Return decrypted string.
            return plainText;
        }

        /// <SUMMARY>
        /// This class uses a symmetric key algorithm (Rijndael/AES) to encrypt and 
        /// decrypt data. As long as encryption and decryption routines use the same
        /// parameters to generate the keys, the keys are guaranteed to be the same.
        /// The class uses static functions with duplicate code to make it easier to
        /// demonstrate encryption and decryption logic. In a real-life application, 
        /// this may not be the most efficient way of handling encryption, so - as
        /// soon as you feel comfortable with it - you may want to redesign this class.
        /// </SUMMARY>
        /// <SUMMARY>
        /// Encrypts specified plaintext using Rijndael symmetric key algorithm
        /// and returns a base64-encoded result.
        /// </SUMMARY>
        /// <PARAM name="plainText">
        /// Plaintext value to be encrypted.
        /// </PARAM>
        /// <PARAM name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived. The
        /// derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that this
        /// passphrase is an ASCII string.
        /// </PARAM>
        /// <PARAM name="saltValue">
        /// Salt value used along with passphrase to generate password. Salt can
        /// be any string. In this example we assume that salt is an ASCII string.
        /// </PARAM>
        /// <PARAM name="hashAlgorithm">
        /// Hash algorithm used to generate password. Allowed values are: "MD5" and
        /// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
        /// </PARAM>
        /// <PARAM name="passwordIterations">
        /// Number of iterations used to generate password. One or two iterations
        /// should be enough.
        /// </PARAM>
        /// <PARAM name="initVector">
        /// Initialization vector (or IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be 
        /// exactly 16 ASCII characters long.
        /// </PARAM>
        /// <PARAM name="keySize">
        /// Size of encryption key in bits. Allowed values are: 128, 192, and 256. 
        /// Longer keys are more secure than shorter keys.
        /// </PARAM>
        /// <RETURNS>
        /// Encrypted value formatted as a base64-encoded string.
        /// </RETURNS>
        /// 
        private static string Encrypt(string plainText,
            string passPhrase,
            string saltValue,
            HashAlgorithm hashAlgorithm,
            int passwordIterations,
            KeySize keySize)
        {
            const string initVector = "@4E1D4A5C3E2g2e7";

            string _hashAlgorithm = hashAlgorithm == HashAlgorithm.MD5 ? "MD5" : "SHA1";

            // Convert strings into byte arrays.
            // Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our plaintext into a byte array.
            // Let us assume that plaintext contains UTF8-encoded characters.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // First, we must create a password, from which the key will be derived.
            // This password will be generated from the specified passphrase and
            // salt value. The password will be created using the specified hash
            // algorithm. Password creation can be done in several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                passPhrase,
                saltValueBytes,
                _hashAlgorithm,
                passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            #pragma warning disable 618,612
            byte[] keyBytes = password.GetBytes((int) keySize/8);
            #pragma warning restore 618,612

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged {Mode = CipherMode.CBC};

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.

            // Generate encryptor from the existing key bytes and initialization
            // vector. Key size will be defined based on the number of the key
            // bytes.
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
                keyBytes,
                initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream();

            // Define cryptographic stream (always use Write mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         encryptor,
                                                         CryptoStreamMode.Write);
            // Start encrypting.
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            string cipherText = Convert.ToBase64String(cipherTextBytes);

            // Return encrypted string.
            return cipherText;
        }

        #endregion Methods
    }
}