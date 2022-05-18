using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Helper
{
    public class AESProcess
    {

        // Sinh ra một chuỗi byte ngẫu nhiên, số lượng byte ngẫu nhiên được chỉ định
        private static byte[] GenRandByte(int length)
        {
            RandomNumberGenerator Random = RandomNumberGenerator.Create();
            var randBytes = new byte[length];
            Random.GetBytes(randBytes);
            return randBytes;
        }

        // Merge cac array lai voi nhau
        // params là chỉ rằng có thể truyền một chuỗi các phần tử cùng loại vào
        // VD params int[] chỉ có thể truyền một chuỗi các int vào, các int này sẽ đc lưu dưới dạng một list, hay array gì đó.
        // VD params int[][] chỉ có thể truyền vào một chuỗi các int [].
        // https://tomrucki.com/posts/aes-encryption-in-csharp/
        private static byte[] MergeArrays(params byte[][] arr)
        {
            var merged = new byte[arr.Sum(x => x.Length)];
            var mergedIndex = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i].CopyTo(merged, mergedIndex);
                mergedIndex += arr[i].Length;
            }
            return merged;
        }

        private static byte[] GetKey(string key, byte[] salt)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);

            using (
                var derivator = new Rfc2898DeriveBytes(
                    keyBytes,
                    salt,
                    100000,
                    HashAlgorithmName.SHA256
                )
            )
            {
                return derivator.GetBytes(256 / 8);
                // Nó sẽ gen ra một cá key 256 bit
                // Key này sẽ được dùng để mã hóa AES, thật ra Key AES đầu vào có thể rage từ 128, 256, 1024, 2048, 2^(n+1)
            }
        }

        public static byte[] EncryptAESString(string plaintText, string password, string authPassword)
        {
            var AesBlockSize = 128 / 8;

            var keySalt = GenRandByte(256 / 8);
            var key = GetKey(password, keySalt);
            var iv = GenRandByte(AesBlockSize);
            using (var aes = Aes.Create())
            {

                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] result_no_hmac;
                using (var encryptor = aes.CreateEncryptor(key, iv))
                {
                    var text = Encoding.UTF8.GetBytes(plaintText);
                    var cipher = encryptor.TransformFinalBlock(text, 0, text.Length);
                    result_no_hmac = MergeArrays(keySalt, iv, cipher);
                }

                var authKeySalt = GenRandByte(256 / 8);
                var authKey = GetKey(authPassword, authKeySalt);
                using (var hmac = new HMACSHA256(authKey))
                {
                    var result_with_auth_key = MergeArrays(authKeySalt, result_no_hmac);
                    var signature = hmac.ComputeHash(result_with_auth_key, 0, result_with_auth_key.Length);
                    var result = MergeArrays(result_with_auth_key, signature);
                    return result;
                }
            }
        }

        public static string DecryptAESString(byte[] encrypted, string password, string authPassword)
        {
            var AesBlockSize = 128 / 8;
            var SignatureSize = 256 / 8;
            var SaltSize = 256 / 8;
            var MinimumEncryptLength =
            SaltSize * 2 + // KeySalt + AuthKeySalt
            AesBlockSize + // IV
            AesBlockSize + // MinimumLenght of cipher text (at least 1 block)
            SignatureSize; // signature size
            if (encrypted is null || encrypted.Length < MinimumEncryptLength)
            {
                throw new ArgumentException("Invalid lenght of encrypted data");
            }
            var authKeySalt = encrypted.Take(SaltSize).ToArray();
            var authKey = GetKey(authPassword, authKeySalt);

            var keySalt = encrypted.Skip(SaltSize).Take(SaltSize).ToArray();
            var key = GetKey(password, keySalt);

            var iv = encrypted.Skip(authKeySalt.Length + keySalt.Length).Take(AesBlockSize).ToArray();

            var signature = encrypted.Skip(encrypted.Length - SignatureSize).ToArray();
            var cipherLength = encrypted.Length - SignatureSize - (SaltSize * 2) - AesBlockSize;
            var cipher = encrypted.Skip(authKeySalt.Length + keySalt.Length + iv.Length).Take(cipherLength).ToArray();

            // Check Signature
            using (var hmac = new HMACSHA256(authKey))
            {
                var payloadToSignLength = encrypted.Length - SignatureSize;
                var expectedSignature = hmac.ComputeHash(encrypted, 0, payloadToSignLength);

                var isSignatureNotValid = 0;
                for (int i = 0; i < expectedSignature.Length; i++)
                {
                    isSignatureNotValid |= signature[i] ^ expectedSignature[i];
                }
                if (isSignatureNotValid == 1)
                {
                    throw new CryptographicException("Invalid Signature");
                }
            }

            // Decryption
            using (var aes = Aes.Create())
            {

                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateDecryptor(key, iv))
                {
                    var de_cipher = encryptor.TransformFinalBlock(cipher, 0, cipher.Length);
                    var result = Encoding.UTF8.GetString(de_cipher);
                    return result;
                }
            }
        }
    }
}