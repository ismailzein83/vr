using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Diagnostics;

namespace CallGeneratorLibrary.Repositories
{
    public class BalanceDetailRepository
    {
        public static BalanceDetail GetEffectiveBalanceDetail()
        {
            BalanceDetail balanceDetail = new BalanceDetail();
            List<BalanceDetail> lstBalanceDetails = new List<BalanceDetail>();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    lstBalanceDetails = GetBalanceDetails();
                    foreach(BalanceDetail balanceDet in lstBalanceDetails)
                    {
                        if(balanceDet.BeginEffectiveDate <= DateTime.Now && balanceDet.EndEffectiveDate >= DateTime.Now && balanceDet.Remaining >= 0)
                        {
                            balanceDetail = balanceDet;
                            break;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return balanceDetail;
        }

        public static List<BalanceDetail> GetBalanceDetails()
        {
            List<BalanceDetail> lstBalanceDetails = new List<BalanceDetail>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<BalanceDetail>(c => c.Contract);
                    context.LoadOptions = options;

                    lstBalanceDetails = context.BalanceDetails.OrderBy(l => l.Id).ToList<BalanceDetail>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return lstBalanceDetails;
        }

        public static void Save(BalanceDetail balanceDetail)
        {
            if (balanceDetail.Id == default(int))
                Insert(balanceDetail);
            else
                Update(balanceDetail);
        }

        private static void Insert(BalanceDetail balanceDetail)
        {
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.BalanceDetails.InsertOnSubmit(balanceDetail);
                    context.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private static void Update(BalanceDetail balanceDetail)
        {
            BalanceDetail _balanceDetail = new BalanceDetail();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    _balanceDetail = context.BalanceDetails.Single(ul => ul.Id == balanceDetail.Id);
                    _balanceDetail.Balance = balanceDetail.Balance;
                    _balanceDetail.Remaining = balanceDetail.Remaining;
                    _balanceDetail.BeginEffectiveDate = balanceDetail.BeginEffectiveDate;
                    _balanceDetail.EndEffectiveDate = balanceDetail.EndEffectiveDate;
                    _balanceDetail.ContractId = balanceDetail.ContractId;
                    context.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
