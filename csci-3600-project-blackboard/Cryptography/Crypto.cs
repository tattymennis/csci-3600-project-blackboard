using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text; // encoding conversions
using System.Security.Cryptography; // C# cryptography libraries
// Password-Based Key Derivation Function used to generate 128, 
// 192, 256 byte keys from arbitrarily long passwords

namespace csci_3600_project_the_struggle.Cryptography
{
    // Crypt() will either encrypt or decrypt
    public enum CryptType { Encrypt, Decrypt };
    public enum CryptAlgo { DES, TDES, AES128, AES192, AES256, BLOWFISH, TWOFISH }
    public class Crypto
    {
        public byte[] Salt { get; set; }
        public byte[] IV { get; set; }
        public object Crypt(CryptType whichCrypt, CryptAlgo whichAlgo, string input, string password)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            SymmetricAlgorithm cipher;
            try
            {
                switch (whichAlgo)
                {
                    case CryptAlgo.DES:
                        cipher = new DESCryptoServiceProvider();
                        break;
                    case CryptAlgo.TDES:
                        cipher = new TripleDESCryptoServiceProvider();
                        break;
                    case CryptAlgo.AES128:
                        cipher = new AesManaged();
                        cipher.KeySize = 128; // 16 bit key
                        if (IV == null)
                        {
                            cipher.GenerateIV();
                            IV = cipher.IV;
                        }
                        else
                        {
                            cipher.IV = IV;
                        }                     
                        break;
                    case CryptAlgo.AES192:
                        cipher = new AesManaged();
                        cipher.KeySize = 192;
                        if (IV == null)
                        {
                            cipher.GenerateIV();
                            IV = cipher.IV;
                        }
                        else
                        {
                            cipher.IV = IV;
                        }
                        break;
                    case CryptAlgo.AES256:
                        cipher = new AesManaged();
                        cipher.KeySize = 256; // 32 bit key
                        if (IV == null)
                        {
                            cipher.GenerateIV();
                            IV = cipher.IV;
                        }
                        else
                        {
                            cipher.IV = IV;
                        }
                        break;
                    default:
                        return false; // switch shouldn't return
                }

                //cipher.Key = UTF8Encoding.UTF8.GetBytes(key);
                //cipher.Key = Encoding.Unicode.GetBytes(password);
                if (Salt == null)
                {
                    Salt = deriveSalt(cipher.KeySize, password);
                }
                
                cipher.Key = deriveKeyFromPass(cipher.KeySize, password, Salt);
                cipher.Padding = PaddingMode.PKCS7; // change to random data or ISO standard
                cipher.Mode = CipherMode.CBC; // More research into proper modes needed

                byte[] _input; // encrypt needs a string, decrypt needs a byte[]
                byte[] _output; // will contain either ciphertxt or plaintxt
                ICryptoTransform transformer = null; // object that does actual crypto computation

                if (whichCrypt == CryptType.Encrypt)
                    transformer = cipher.CreateEncryptor(cipher.Key, cipher.IV);

                else if (whichCrypt == CryptType.Decrypt)
                    transformer = cipher.CreateDecryptor(cipher.Key, cipher.IV);
                
                if (input is string)
                {
                    //_input = UTF8Encoding.UTF8.GetBytes(input as string);

                    if (whichCrypt == CryptType.Encrypt)
                    {
                        //_input = UnicodeEncoding.Unicode.GetBytes(input as string);
                        _input = Encoding.UTF8.GetBytes(input as string);
                        //_input = Convert.FromBase64String(input as string);
                        _output = transformer.TransformFinalBlock(_input, 0, _input.Length);
                        transformer.Dispose();
                        cipher.Clear(); // kill resource
                        //return Convert.ToBase64String(_output);
                        return Convert.ToBase64String(_output, 0, _output.Length);
                    }

                    else if (whichCrypt == CryptType.Decrypt)
                    {
                        _input = Convert.FromBase64String(input as string);
                        _output = transformer.TransformFinalBlock(_input, 0, _input.Length);
                        //return Convert.ToBase64String(_input, 0, _input.Length);
                        //return Convert.ToBase64String(_output);
                        //return Encoding.UTF8.GetString(_output);
                        transformer.Dispose();
                        cipher.Clear(); // kill resource
                        return Encoding.UTF8.GetString(_output);
                    }
                }

                //else if (input is byte[])
                //{
                //    _output = transformer.TransformFinalBlock(input as byte[], 0, (input as byte[]).Length);
                //    return _output;
                //}
                
                return false; // failed
            }
            catch (Exception ex)
            {
                //return ex.Message;
                return "Invalid key! " + ex.Message;
            }

        }

        // necessary to store salt alongside ciphertext
        public byte[] deriveSalt(int keySize, string key)
        {
            byte[] salt;
            try
            {
                if (keySize == 128 || keySize == 192 || keySize == 256)
                {
                    salt = new byte[8];
                    using (RNGCryptoServiceProvider Rando = new RNGCryptoServiceProvider())
                    {
                        Rando.GetBytes(salt); // populate salt with secure bits
                        //Rando.Dispose();
                    }
                    return salt;
                }
                else
                    throw new Exception();               
            }

            catch (Exception ex)
            {
                return null;
            }
        }

        // generate keys of appropriate bit-size for various ciphers
        public byte[] deriveKeyFromPass(int keySize, string key, byte[] salt)
        {
            byte[] newKey; // number of bytes required for keySize
            try
            {
                if (keySize == 128 || keySize == 192 || keySize == 256)
                {
                    using (Rfc2898DeriveBytes ByteDeriver = new Rfc2898DeriveBytes(key, salt, 1000))
                    {
                        newKey = ByteDeriver.GetBytes(keySize / 8);
                    }
                        
                    return newKey;
                }

                else
                    throw new Exception();
            }

            catch (Exception ex)
            {
                return null;
            }
        }
    }
}