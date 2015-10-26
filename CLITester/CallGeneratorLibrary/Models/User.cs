using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallGeneratorLibrary.Repositories;
using System.Diagnostics;

namespace CallGeneratorLibrary
{
    public partial class User
    {
        string balancehistory = "";
        public string BalanceHistory
        {
            get
            {
                try
                {
                    if (balancehistory == "")
                    {
                        List<UserBalance> lstOp = new List<UserBalance>();
                        lstOp = UserBalanceRepository.GetUserBalances(this.Id);
                        foreach (UserBalance op in lstOp)
                            balancehistory += op.OldBalance + "$" + op.CountCalls + "$" + op.CreationDate + "!";
                    }

                    return balancehistory;
                }
                catch (System.Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
