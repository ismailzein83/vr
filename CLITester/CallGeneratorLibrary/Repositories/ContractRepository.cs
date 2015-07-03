using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallGeneratorLibrary.Utilities;
using System.Diagnostics;
using System.Data.Linq;

namespace CallGeneratorLibrary.Repositories
{
    public class ContractRepository
    {
        public static void Save(Contract contract)
        {
            Contract contractObj = new Contract();
            contractObj.Name = contract.Name;
            contractObj.CreationDate = contract.CreationDate;
            contractObj.ChargeType = contract.ChargeType;
            contractObj.IsDone = contract.IsDone;
            contractObj.Description = contract.Description;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.Contracts.InsertOnSubmit(contractObj);
                    context.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        public static List<Contract> GetAllContracts()
        {
            List<Contract> lstContracts = new List<Contract>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    lstContracts = context.Contracts.ToList<Contract>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return lstContracts;
        }

        public static List<Contract> GetContracts()
        {
            List<Contract> lstContracts = new List<Contract>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    lstContracts = context.Contracts.Where(l => l.IsDone != true).ToList<Contract>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return lstContracts;
        }
    }
}
