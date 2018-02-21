using SOM.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Business
{
    public class ContractManager
    {
        public List<TelephonyContractInfo> GetTelephonyContracts(string billingAccountId, string telephonyNumber)
        {
            return new List<TelephonyContractInfo>
            {
                 new TelephonyContractInfo
                 {
                     TelephonyContractId = "1",
                      PhoneNumber= "4343465546",
                       ContractTime = DateTime.Today
                 },
                 new TelephonyContractInfo
                 {
                     TelephonyContractId = "2",
                      PhoneNumber= "34598798767",
                       ContractTime = DateTime.Today
                 },
                 new TelephonyContractInfo
                 {
                     TelephonyContractId = "3",
                      PhoneNumber= "439743444",
                       ContractTime = DateTime.Today
                 }
            };
        }

        public List<LeasedLineContractInfo> GetLeasedLineContracts(string billingAccountId, string telephonyNumber)
        {
            return new List<LeasedLineContractInfo>
            {
                 new LeasedLineContractInfo
                 {
                     LeasedLineContractId = "1",
                      PhoneNumber= "932487855",
                       ContractTime = DateTime.Today
                 },
                 new LeasedLineContractInfo
                 {
                     LeasedLineContractId = "2",
                      PhoneNumber= "233432545",
                       ContractTime = DateTime.Today
                 },
                 new LeasedLineContractInfo
                 {
                     LeasedLineContractId = "3",
                      PhoneNumber= "34654364",
                       ContractTime = DateTime.Today
                 }
            };
        }

        public List<XDSLContractInfo> GetXDSLContracts(string billingAccountId, string telephonyNumber)
        {
            return new List<XDSLContractInfo>
            {
                 new XDSLContractInfo
                 {
                     XDSLContractId = "1",
                      PhoneNumber= "687684545",
                       ContractTime = DateTime.Today
                 },
                 new XDSLContractInfo
                 {
                     XDSLContractId = "2",
                      PhoneNumber= "6547657653",
                       ContractTime = DateTime.Today
                 },
                 new XDSLContractInfo
                 {
                     XDSLContractId = "3",
                      PhoneNumber= "5654765487",
                       ContractTime = DateTime.Today
                 }
            };
        }
    }
}
