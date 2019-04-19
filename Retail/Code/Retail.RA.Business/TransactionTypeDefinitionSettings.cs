using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Retail.RA.Business
{
    public class TransactionTypeLKUPBEDefinitionSettings : LKUPBEDefinitionExtendedSettings
    {
        Guid prepaidTransactionTypeBEDefinitionID = new Guid("4444F969-32BE-4674-9EAB-764959BF037A");
        Guid postpaidTransactionTypeBEDefinitionID = new Guid("66A579FE-14E1-494F-BB02-AAD94BE9D5FE");

        public override Guid ConfigId { get { return new Guid("BA619742-DF5C-4448-A607-E38AD7B43D86"); } }
        public override Dictionary<string, LKUPBusinessEntityItem> GetAllLKUPBusinessEntityItems(ILKUPBusinessEntityExtendedSettingsContext context)
        {
            Dictionary<string, LKUPBusinessEntityItem> lkUPBusinessEntityItems = new Dictionary<string, LKUPBusinessEntityItem>();
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();

            var prepaidTransactionTypes = genericBusinessEntityManager.GetAllGenericBusinessEntities(prepaidTransactionTypeBEDefinitionID);
            var postpaidTransactionTypes = genericBusinessEntityManager.GetAllGenericBusinessEntities(postpaidTransactionTypeBEDefinitionID);

            if(prepaidTransactionTypes!=null && prepaidTransactionTypes.Count > 0)
            {
                foreach(var transactionType in prepaidTransactionTypes)
                {
                    var idFieldAsString = transactionType.FieldValues.GetRecord("ID").ToString();
                    lkUPBusinessEntityItems.Add(idFieldAsString, new LKUPBusinessEntityItem()
                    {
                        BusinessEntityDefinitionId = prepaidTransactionTypeBEDefinitionID,
                        LKUPBusinessEntityItemId = idFieldAsString,
                        Name = transactionType.FieldValues.GetRecord("Name").ToString()
                    });
                }
            }

            if(postpaidTransactionTypes!=null && postpaidTransactionTypes.Count > 0)
            {
                foreach(var transactionType in postpaidTransactionTypes)
                {
                    var idFieldAsString = transactionType.FieldValues.GetRecord("ID").ToString();
                    lkUPBusinessEntityItems.Add(idFieldAsString, new LKUPBusinessEntityItem()
                    {
                        BusinessEntityDefinitionId = postpaidTransactionTypeBEDefinitionID,
                        LKUPBusinessEntityItemId = idFieldAsString,
                        Name = transactionType.FieldValues.GetRecord("Name").ToString()
                    });
                }
            }
            return lkUPBusinessEntityItems;
        }

        public override bool IsCacheExpired(ref DateTime? lastCheckTime)
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.IsCacheExpired(prepaidTransactionTypeBEDefinitionID, ref lastCheckTime) || genericBusinessEntityManager.IsCacheExpired(postpaidTransactionTypeBEDefinitionID, ref lastCheckTime);
        }
    }
}
