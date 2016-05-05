using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{

    public class AccountStatusManager
    {
        public AccountStatus GetAccountStatus(string accountNumber)
        {
            IAccountStatusDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountStatusDataManager>();
            return dataManager.GetAccountStatus(accountNumber);
        }

        public string AddAccountStatuses(int fileId, DateTime validTill, string reason)
        {
            DataTable accountStatusDataTable = new DataTable();
            VRFileManager fileManager = new VRFileManager();
            byte[] bytes = fileManager.GetFile(fileId).Content;
            var fileStream = new System.IO.MemoryStream(bytes);
            ExportTableOptions options = new ExportTableOptions();
            options.CheckMixedValueType = true;
            Workbook woorkbook = new Workbook(fileStream);
            Common.Utilities.ActivateAspose(); 
            woorkbook.CalculateFormula();
            if (woorkbook.Worksheets[0].Cells.MaxDataRow > -1 && woorkbook.Worksheets[0].Cells.MaxDataColumn > -1)
                accountStatusDataTable = woorkbook.Worksheets[0].Cells.ExportDataTableAsString(0, 0, woorkbook.Worksheets[0].Cells.MaxDataRow + 1, woorkbook.Worksheets[0].Cells.MaxDataColumn + 1);

            IAccountStatusDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountStatusDataManager>();
            int userId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();

            bool applied = dataManager.ApplyAccountStatuses(accountStatusDataTable, validTill, reason, userId);
            if (applied)
                return "Uploaded Successfully";
            else
                return "Error Occured";
        }

        public Vanrise.Entities.InsertOperationOutput<AccountStatusDetail> AddAccountStatus(AccountStatus accountStatus)
        {
            accountStatus.Source = AccountStatusSource.ManualUpload;
            accountStatus.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            Vanrise.Entities.InsertOperationOutput<AccountStatusDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountStatusDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IAccountStatusDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountStatusDataManager>();
            bool insertActionSucc = dataManager.Insert(accountStatus);
            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = AccountStatusDetailMapper(accountStatus);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<AccountStatusDetail> UpdateAccountStatus(AccountStatus accountStatus)
        {
            accountStatus.Source = AccountStatusSource.ManualUpload;
            accountStatus.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();

            IAccountStatusDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountStatusDataManager>();

            bool updateActionSucc = dataManager.Update(accountStatus);
            Vanrise.Entities.UpdateOperationOutput<AccountStatusDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountStatusDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AccountStatusDetailMapper(accountStatus);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }


        public DeleteOperationOutput<object> DeleteAccountStatus(string number)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.InUse;

            IAccountStatusDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountStatusDataManager>();

            bool deleted = dataManager.Delete(number);

            if (deleted)
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }


        public List<string> GetAccountNumbersByNumberPrefixAndStatuses(List<CaseStatus> caseStatuses, List<string> numberPrefixes)
        {
            IAccountStatusDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountStatusDataManager>();
            return dataManager.GetAccountNumbersByNumberPrefixAndStatuses(caseStatuses, numberPrefixes);
        }

        public bool InsertOrUpdateAccountStatus(string accountNumber, CaseStatus caseStatus, DateTime? validTill, string reason, int userId)
        {
            IAccountStatusDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountStatusDataManager>();
            return dataManager.InsertOrUpdateAccountStatus(accountNumber, caseStatus, validTill, reason, userId);
        }

        public Vanrise.Entities.IDataRetrievalResult<AccountStatusDetail> GetAccountStatusesData(Vanrise.Entities.DataRetrievalInput<AccountStatusQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new AccountStatusRequestHandler());
        }

        public AccountStatusDetail AccountStatusDetailMapper(AccountStatus accountStatus)
        {
            Vanrise.Security.Business.UserManager userManager = new Security.Business.UserManager();
            AccountStatusDetail accountStatusDetail = new AccountStatusDetail
            {
                Entity = accountStatus,
                StatusName = Utilities.GetEnumDescription(accountStatus.Status),
                SourceName = Utilities.GetEnumDescription(accountStatus.Source),
                UserName = userManager.GetUserName(accountStatus.UserId)
            };

            return accountStatusDetail;
        }

        #region Private Classes

        private class AccountStatusRequestHandler : BigDataRequestHandler<AccountStatusQuery, AccountStatus, AccountStatusDetail>
        {
            public override AccountStatusDetail EntityDetailMapper(AccountStatus entity)
            {
                AccountStatusManager manager = new AccountStatusManager();
                return manager.AccountStatusDetailMapper(entity);
            }

            public override IEnumerable<AccountStatus> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<AccountStatusQuery> input)
            {
                IAccountStatusDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountStatusDataManager>();
                return dataManager.GetAccountStatusesData(input);
            }
        }

        #endregion

    }


}
