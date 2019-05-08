using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class AccountManagerManager
    {
        //static Guid businessEntityDefinitionId = new Guid("0146109f-4e5d-4d66-be2f-15d689c960ee");

        //#region Public Methods

        //public IEnumerable<AccountManager> GetAllAccountManagers()
        //{
        //    return GetCachedAccountManager().Values;
        //}

        //public AccountManager GetAccountManagerById(int accountManagerId)
        //{
        //    return GetCachedAccountManager().GetRecord(accountManagerId);
        //}

        //#endregion

        //#region Private Methods
        //private Dictionary<int, AccountManager> GetCachedAccountManager()
        //{
        //    IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
        //    return genericBusinessEntityManager.GetCachedOrCreate("GetCachedAccountManager", businessEntityDefinitionId, () =>
        //    {
        //        Dictionary<int, AccountManager> result = new Dictionary<int, AccountManager>();
        //        IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId);
        //        if (genericBusinessEntities != null)
        //        {
        //            foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
        //            {
        //                AccountManager accountManager= new AccountManager()
        //                {
        //                    AccountManagerId = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
        //                    ParentId = (int)genericBusinessEntity.FieldValues.GetRecord("ParentId"),
        //                    UserId = (int)genericBusinessEntity.FieldValues.GetRecord("UserId"),
        //                    CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
        //                    LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
        //                    CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
        //                    LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy"),
        //                };
        //                result.Add(accountManager.AccountManagerId, accountManager);
        //            }
        //        }
        //        return result;
        //    });
        //}
        //#endregion


        //#region Mappers
        //#endregion
    }
}
