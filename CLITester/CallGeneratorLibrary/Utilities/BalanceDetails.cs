using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary;
using System.Diagnostics;

namespace CallGeneratorLibrary.Utilities
{
    public class BalanceDetails
    {
        public static void FillBalanceDetails()
        {
            try
            {
                //Reset all balance for users at midnight
                List<User> lstUsers = UserRepository.GetSubUsers();
                for (int i = 0; i < lstUsers.Count(); i++)
                {
                    TimeSpan span = new TimeSpan();
                    span = DateTime.Now - lstUsers[i].LastSetBalance.Value;
                    if (span.TotalSeconds >= 86400)
                    {
                        lstUsers[i].LastSetBalance = DateTime.Now;
                        lstUsers[i].RemainingBalance = lstUsers[i].Balance;
                        UserRepository.Save(lstUsers[i]);
                    }
                }
                //bool isMidnight = DateTime.Now.TimeOfDay.Ticks == 0;
                //if(isMidnight)
                //{
                //}

                //Get all contract not done and save into balance details
                List<Contract> lstContract = ContractRepository.GetContracts();
                for (int j = 0; j < lstContract.Count(); j++)
                {
                    ContractDetails contractDetails = new ContractDetails();
                    contractDetails = Utilities.DeserializeLINQfromXML<ContractDetails>(lstContract[j].Description);
                    for (int i = 0; i < contractDetails.NumberOfPeriod; i++)
                    {
                        BalanceDetail balanceDetail = new BalanceDetail();
                        balanceDetail.Balance = contractDetails.NumberOfCalls;
                        balanceDetail.Remaining = contractDetails.NumberOfCalls;

                        if (contractDetails.Period == (int)CallGeneratorLibrary.Utilities.Enums.PeriodRecharge.Daily)
                        {
                            balanceDetail.BeginEffectiveDate = DateTime.Now.AddDays(i);
                            balanceDetail.EndEffectiveDate = DateTime.Now.AddDays(i + 1);
                        }

                        if (contractDetails.Period == (int)CallGeneratorLibrary.Utilities.Enums.PeriodRecharge.Monthly)
                        {
                            balanceDetail.BeginEffectiveDate = DateTime.Now.AddMonths(i);
                            balanceDetail.EndEffectiveDate = DateTime.Now.AddMonths(i + 1);
                        }

                        if (contractDetails.Period == (int)CallGeneratorLibrary.Utilities.Enums.PeriodRecharge.yearly)
                        {
                            balanceDetail.BeginEffectiveDate = DateTime.Now.AddYears(i);
                            balanceDetail.EndEffectiveDate = DateTime.Now.AddYears(i + 1);
                        }
                        balanceDetail.ContractId = lstContract[j].Id;
                        BalanceDetailRepository.Save(balanceDetail);
                    }

                    lstContract[j].IsDone = true;
                    ContractRepository.Save(lstContract[j]);
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

        }
        public static bool CheckUserBalance(int userId)
        {
            try
            {
                BalanceDetail balanceDetail = BalanceDetailRepository.GetEffectiveBalanceDetail();
                if (balanceDetail != null)
                {
                    //Check if the super user (client) have any effective balance
                    int Requested = TestOperatorRepository.GetRequestedTestOperatorsByUser();
                    if (balanceDetail.Remaining - Requested > 0)
                    {
                        //Check if the sub user has still remaning calls per day
                        User user = UserRepository.Load(userId);
                        if (user.Role == (int)CallGeneratorLibrary.Utilities.Enums.UserRole.User)
                        {
                            if (user.RemainingBalance > 0)
                            {
                                int RequestedUser = TestOperatorRepository.GetRequestedTestOperatorsByUser(userId);
                                if (user.RemainingBalance - RequestedUser > 0)
                                {
                                    return true;
                                }
                            }
                        }
                        else
                            return true;
                    }
                }
                return false;
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
                return false;
            }
        }

        public static bool DecreaseBalance(int userId)
        {
            try
            {
                BalanceDetail balanceDetail = BalanceDetailRepository.GetEffectiveBalanceDetail();
                if (balanceDetail != null)
                {
                    //Decrease the remaining balance from balance details
                    balanceDetail.Remaining = balanceDetail.Remaining - 1;
                    BalanceDetailRepository.Save(balanceDetail);

                    //Decrease the balance for the user for the priviliges of the user test call per day
                    User user = UserRepository.Load(userId);
                    user.RemainingBalance = user.RemainingBalance - 1;
                    UserRepository.Save(user);
                    return true;
                }
                return false;
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
                return false;
            }
        }
    }
}
