using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Backend.KeyPair.Client;
using Backend.KeyPair.ClientBank;
using Backend.KeyPair.Merchant;
using Backend.KeyPair.MerchantBank;
using Backend.Helper;

namespace Backend.Helper
{
    public class KeyProcess
    {
        public static string readKey(string keyName)
        {
            var lowerCase = keyName.ToLower();
            switch (lowerCase)
            {
                case "client":
                    {
                        Console.WriteLine(keyName);
                        return ClientKeyPair.PrivatePem();
                    }
                case "merchant":
                    {
                        Console.WriteLine(keyName);
                        return MerchantKeyPair.PrivatePem();
                    }
                case "client_bank":
                    {
                        Console.WriteLine(keyName);
                        return ClientBankKeyPair.PrivatePem();
                    }
                case "merchant_bank":
                    {
                        Console.WriteLine(keyName);
                        return MerchantBankKeyPair.PrivatePem();
                    }
                default:
                    {
                        return null;
                    }
            }
        }


        public static byte[] PrivareEncryption_RSA(RSA rsa, byte[] data)
        {
            // Add 4 byte padding to the data, and convert to BigInteger struct
            BigInteger numData = GetBig(AddPadding(data));

            RSAParameters rsaParams = rsa.ExportParameters(true);
            // Console.WriteLine(rsaParams);
            BigInteger D = GetBig(rsaParams.D);
            BigInteger Modulus = GetBig(rsaParams.Modulus);
            BigInteger encData = BigInteger.ModPow(numData, D, Modulus);

            return encData.ToByteArray();
        }

        public static byte[] PublicDecryption_RSA(RSA rsa, byte[] cipherData)
        {
            if (cipherData == null)
                throw new ArgumentNullException("cipherData");

            BigInteger numEncData = new BigInteger(cipherData);

            RSAParameters rsaParams = rsa.ExportParameters(false);
            BigInteger Exponent = GetBig(rsaParams.Exponent);
            BigInteger Modulus = GetBig(rsaParams.Modulus);

            BigInteger decData = BigInteger.ModPow(numEncData, Exponent, Modulus);

            byte[] data = decData.ToByteArray();
            byte[] result = new byte[data.Length - 1];
            Array.Copy(data, result, result.Length);
            result = RemovePadding(result);

            Array.Reverse(result);
            return result;
        }

        static BigInteger GetBig(byte[] data)
        {
            byte[] inArr = (byte[])data.Clone();
            Array.Reverse(inArr);  // Reverse the byte order
            byte[] final = new byte[inArr.Length + 1];  // Add an empty byte at the end, to simulate unsigned BigInteger (no negatives!)
            Array.Copy(inArr, final, inArr.Length);

            return new BigInteger(final);
        }

        // Add 4 byte random padding, first bit *Always On*
        static byte[] AddPadding(byte[] data)
        {
            Random rnd = new Random();
            byte[] paddings = new byte[4];
            rnd.NextBytes(paddings);
            paddings[0] = (byte)(paddings[0] | 128);

            byte[] results = new byte[data.Length + 4];

            Array.Copy(paddings, results, 4);
            Array.Copy(data, 0, results, 4, data.Length);
            return results;
        }

        static byte[] RemovePadding(byte[] data)
        {
            byte[] results = new byte[data.Length - 4];
            Array.Copy(data, results, results.Length);
            return results;
        }

    }
}