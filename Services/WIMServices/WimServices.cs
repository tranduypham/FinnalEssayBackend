using System;
using System.Collections.Generic;
using System.Linq;
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
                        return new BankingInfoDto() {
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
                        return new BankingInfoDto() {
                                BankingInfo = bankInfo,
                                Signature = sign
                            };
                    }

                default:
                    return null;
            }
        }
    }
}