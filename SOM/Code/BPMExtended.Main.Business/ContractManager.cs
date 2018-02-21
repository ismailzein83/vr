using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class ContractManager
    {
        public List<TelephonyContractInfo> GetTelephonyContractInfos(CustomerObjectType customerObjectType, Guid accountOrContactId, string telephonyNumber)
        {
            return new List<TelephonyContractInfo>
            {
                 new TelephonyContractInfo
                 {
                     TelephonyContractId = "1",
                     PhoneNumber= "4343465546",
                     Status = "Active",
                     ContractTime = DateTime.Today
                 },
                 new TelephonyContractInfo
                 {
                     TelephonyContractId = "2",
                      PhoneNumber= "34598798767",
                      Status = "Active",
                       ContractTime = DateTime.Today
                 },
                 new TelephonyContractInfo
                 {
                     TelephonyContractId = "3",
                      Status = "Inactive",
                      PhoneNumber= "439743444",
                       ContractTime = DateTime.Today
                 }
            };
        }

        public TelephonyContract GetTelephonyContract(CustomerObjectType customerObjectType, Guid accountOrContactId, string telephonyContractId)
        {
            var contractInfo = GetTelephonyContractInfos(customerObjectType, accountOrContactId, null).FirstOrDefault(itm => itm.TelephonyContractId == telephonyContractId);
            if (contractInfo != null)
                return new TelephonyContract
                {
                    TelephonyContractId = contractInfo.TelephonyContractId,
                    PhoneNumber = contractInfo.PhoneNumber,
                    Status = contractInfo.Status,
                    ContractTime = contractInfo.ContractTime
                };
            else
                return null;
        }

        public List<TelephonyContractServiceInfo> GetTelephonyContractServiceInfos(CustomerObjectType customerObjectType, Guid accountOrContactId, string telephonyContractId)
        {
            return new List<TelephonyContractServiceInfo>
            {
                new TelephonyContractServiceInfo
                {
                     TelephonyContractServiceInfoId = "1",
                     ServiceName = "Service 1",
                     Status = "Active",
                     CreatedTime = DateTime.Today
                },
                new TelephonyContractServiceInfo
                {
                     TelephonyContractServiceInfoId = "1",
                     ServiceName = "Service 2",
                     Status = "Active",
                     CreatedTime = DateTime.Today
                },
                new TelephonyContractServiceInfo
                {
                     TelephonyContractServiceInfoId = "1",
                     ServiceName = "Service 3",
                     Status = "Active",
                     CreatedTime = DateTime.Today
                },
                new TelephonyContractServiceInfo
                {
                     TelephonyContractServiceInfoId = "1",
                     ServiceName = "Service 4",
                     Status = "Active",
                     CreatedTime = DateTime.Today
                }
            };
        }

        public List<LeasedLineContractInfo> GetLeasedLineContractInfos(CustomerObjectType customerObjectType, Guid accountOrContactId, string telephonyNumber)
        {
            return new List<LeasedLineContractInfo>
            {
                 new LeasedLineContractInfo
                 {
                     LeasedLineContractId = "1",
                      PhoneNumber= "932487855",
                      Status = "Active",
                       ContractTime = DateTime.Today
                 },
                 new LeasedLineContractInfo
                 {
                     LeasedLineContractId = "2",
                      Status = "Inactive",
                      PhoneNumber= "233432545",
                       ContractTime = DateTime.Today
                 },
                 new LeasedLineContractInfo
                 {
                     LeasedLineContractId = "3",
                      PhoneNumber= "34654364",
                      Status = "Active",
                       ContractTime = DateTime.Today
                 }
            };
        }
    }
}
