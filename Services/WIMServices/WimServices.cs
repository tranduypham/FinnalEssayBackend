using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Backend.DTO;
using Backend.Models;
using Backend.Repository;
using Backend.Services.Encryption;

namespace Backend.Services.WIMServices
{
    public class WimServices : IWimServices
    {
        IBankingInfoRepo _bankInfoRepo;
        IEncryptionServices _enc;
        public WimServices(IBankingInfoRepo bankInfoRepo, IEncryptionServices enc)
        {
            _bankInfoRepo = bankInfoRepo;
            _enc = enc;
        }
        public BankingInfoDto? GetBankingInfo(string KeyName)
        {
            var temp = KeyName.ToLower();
            switch (temp)
            {
                case "client":
                    {
                        var bankInfo = _bankInfoRepo.Get("1255070770448");
                        if (bankInfo == null) return null;
                        var sign = _enc.SignData("client_bank", JsonSerializer.Serialize(bankInfo));
                        // return _enc.EncryptData(
                        //     "client",
                        //     JsonSerializer.Serialize(new {
                        //         BankInfo = bankInfo,
                        //         Signature = sign
                        //     }),
                        //     true
                        // );
                        return new BankingInfoDto()
                        {
                            BankingInfo = bankInfo,
                            Signature = sign
                        };
                    }

                case "merchant":
                    {
                        var bankInfo = _bankInfoRepo.Get("7467811997849");
                        if (bankInfo == null) return null;
                        var sign = _enc.SignData("merchant_bank", JsonSerializer.Serialize(bankInfo));
                        // return _enc.EncryptData(
                        //     "merchant",
                        //     JsonSerializer.Serialize(new {
                        //         BankInfo = bankInfo,
                        //         Signature = sign
                        //     }),
                        //     true
                        // );
                        return new BankingInfoDto()
                        {
                            BankingInfo = bankInfo,
                            Signature = sign
                        };
                    }

                default:
                    return null;
            }
        }

        public string RandNumString(int length)
        {
            Random random = new Random();
            string RandomString(int length)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            var byteRand = Encoding.ASCII.GetBytes(RandomString(length));
            return Convert.ToBase64String(byteRand);
        }
        public string genPreMasterSecret() {
            return this.RandNumString(20);
        }
        public string genMasterSecret(string Pre_Master_Secret, string RandString) {
            var preMasterByte = Convert.FromBase64String(Pre_Master_Secret);
            var RandNumString = Encoding.ASCII.GetBytes(RandString);
            var MasterByte = this.PRF(preMasterByte, RandNumString, 20);
            return Convert.ToBase64String(MasterByte);
        }
        public SessionKey genSessionKey (string Master_Secret_64, string RandString) {
            var MasterByte = Convert.FromBase64String(Master_Secret_64);
            var RandNumString = Encoding.ASCII.GetBytes(RandString);
            byte[] SessionByte = this.PRF(MasterByte, RandNumString, 20*4);

            var ClientWriteKey_byte = SessionByte.Take(20).ToArray();
            var ServerWriteKey_byte = SessionByte.Skip(20).Take(20).ToArray();
            var ClientWriteMacKey_byte = SessionByte.Skip(20*2).Take(20).ToArray();
            var ServerWriteMacKey_byte = SessionByte.Skip(20*3).Take(20).ToArray();
            return new SessionKey(){
                ClientWriteKey64 = Convert.ToBase64String(ClientWriteKey_byte),
                ClientWriteMacKey64 = Convert.ToBase64String(ClientWriteMacKey_byte),
                ServerWriteMacKey64 = Convert.ToBase64String(ServerWriteMacKey_byte),
                ServerWriteKey64 = Convert.ToBase64String(ServerWriteKey_byte),
            };
        }
        // Thuat toan Gia ngau nhien PRF dua tre MD5-hash XOR voi SHA1-hash
        private byte[] PRF(byte[] secret, byte[] seed, int outputLength)
        {
            List<byte> s1 = new List<byte>();
            List<byte> s2 = new List<byte>();

            int size = (int)Math.Ceiling((double)secret.Length / 2);

            // s1.AddRange(Utils.CopyArray(secret, 0, size));
            // s2.AddRange(Utils.CopyArray(secret, secret.Length - size, size));
            byte[] temp1 = new byte[size];
            byte[] temp2 = new byte[size];
            Array.Copy(secret, temp1, size);
            Array.Copy(secret, secret.Length - size, temp2, 0, size);
            s1.AddRange(temp1);
            s2.AddRange(temp2);

            var tbc = new List<byte>();
            // tbc.AddRange(label);
            tbc.AddRange(seed);

            var md5Result = MD5Hash(s1.ToArray(), tbc.ToArray(), outputLength);

            var sha1Result = SHA1Hash(s2.ToArray(), tbc.ToArray(), outputLength);


            var result = new List<byte>();
            for (int i = 0; i < outputLength; i++)
                result.Add((byte)(md5Result[i] ^ sha1Result[i]));

            return result.ToArray();
        }
        // Thuat toan Hash MD5
        private byte[] MD5Hash(byte[] secret, byte[] seed, int outputLength)
        {
            int iterations = (int)Math.Ceiling((double)outputLength / 16);

            HMACMD5 HMD5 = new HMACMD5(secret);

            var result = new List<byte>();
            byte[] A = null;
            for (int i = 0; i <= iterations; i++)
                if (A == null)
                    A = seed;
                else
                {
                    A = HMD5.ComputeHash(A);

                    var tBuff = new List<byte>();
                    tBuff.AddRange(A);
                    tBuff.AddRange(seed);

                    var tb = HMD5.ComputeHash(tBuff.ToArray());

                    result.AddRange(tb);
                }
            return result.ToArray();
        }
        // Thuat toan Hash SHA1
        private byte[] SHA1Hash(byte[] secret, byte[] seed, int outputLength)
        {
            int iterations = (int)Math.Ceiling((double)outputLength / 20);

            HMACSHA1 HSHA1 = new HMACSHA1(secret);
            var result = new List<byte>();
            byte[] A = null;

            for (int i = 0; i <= iterations; i++)
                if (A == null)
                    A = seed;
                else
                {
                    A = HSHA1.ComputeHash(A);

                    var tBuff = new List<byte>();
                    tBuff.AddRange(A);
                    tBuff.AddRange(seed);

                    var tb = HSHA1.ComputeHash(tBuff.ToArray());

                    result.AddRange(tb);
                }

            return result.ToArray();
        }

    }
}