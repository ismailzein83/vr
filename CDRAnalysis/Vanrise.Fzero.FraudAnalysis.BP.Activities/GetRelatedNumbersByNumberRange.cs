using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class GetRelatedNumbersByNumberRange : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<AccountRelatedNumberDictionary> AccountRelatedNumbersDictionary { get; set; }

        public InArgument<string> NumberPrefix { get; set; }

        #endregion


        protected override void Execute(CodeActivityContext context)
        {
            IRelatedNumberDataManager dataManager = FraudDataManagerFactory.GetDataManager<IRelatedNumberDataManager>();
            AccountRelatedNumberDictionary accountRelatedNumberDictionary = new AccountRelatedNumberDictionary();

            dataManager.LoadRelatedNumberByNumberPrefix(NumberPrefix.Get(context), ((relatedNumber) =>
                {
                    if (relatedNumber.RelatedAccountNumber != null)
                    {
                        HashSet<String> relatedAccountNumbers = accountRelatedNumberDictionary.GetOrCreateItem(relatedNumber.AccountNumber);
                        relatedAccountNumbers.Add(relatedNumber.RelatedAccountNumber);
                    }
                }));


            context.SetValue(AccountRelatedNumbersDictionary, accountRelatedNumberDictionary);

        }

    }
}
