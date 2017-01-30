using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountActionManager
    {
        #region Public Methods

        public bool TryConvertActionToBPArg(Guid accountBEDefinitionId, long accountId, AccountActionBackendExecutor actionExecutor, out BaseProcessInputArgument bpInputArg)
        {
            var context = new AccountActionBackendExecutorConvertToBPArgContext(accountBEDefinitionId, accountId);
            return TryConvertActionToBPArg(context, actionExecutor, out bpInputArg);
        }

        public bool TryConvertActionToBPArg(Guid accountBEDefinitionId, Account account, AccountActionBackendExecutor actionExecutor, out BaseProcessInputArgument bpInputArg)
        {
            var context = new AccountActionBackendExecutorConvertToBPArgContext(accountBEDefinitionId, account);
            return TryConvertActionToBPArg(context, actionExecutor, out bpInputArg);
        }

        public void Execute(Guid accountBEDefinitionId, long accountId, AccountActionBackendExecutor actionExecutor)
        {
            var context = new AccountActionBackendExecutorExecuteContext(accountBEDefinitionId, accountId);
            Execute(context, actionExecutor);
        }

        public void Execute(Guid accountBEDefinitionId, Account account, AccountActionBackendExecutor actionExecutor)
        {
            var context = new AccountActionBackendExecutorExecuteContext(accountBEDefinitionId, account);
            Execute(context, actionExecutor);
        }

        #endregion

        #region Private Methods

        private bool TryConvertActionToBPArg(AccountActionBackendExecutorConvertToBPArgContext context, AccountActionBackendExecutor actionExecutor, out BaseProcessInputArgument bpInputArg)
        {
            if(actionExecutor == null)
                throw new ArgumentNullException("actionExecutor");
            if(actionExecutor.TryConvertToBPArg(context))
            {
                bpInputArg = context.BPInputArgument;
                return true;
            }
            else
            {
                bpInputArg = null;
                return false;
            }
        }

        private void Execute(AccountActionBackendExecutorExecuteContext context, AccountActionBackendExecutor actionExecutor)
        {
            if (actionExecutor == null)
                throw new ArgumentNullException("actionExecutor");
            actionExecutor.Execute(context);
        }

        #endregion

        #region Private Classes

        private class AccountActionBackendExecutorConvertToBPArgContext : IAccountActionBackendExecutorConvertToBPArgContext
        {
            Guid _accountDefinitionId; 
            long _accountId;
            static AccountBEManager s_accountBEManager = new AccountBEManager();
            public AccountActionBackendExecutorConvertToBPArgContext (Guid accountDefinitionId, long accountId)
            {
                _accountDefinitionId = accountDefinitionId;
                _accountId = accountId;
            }

            public AccountActionBackendExecutorConvertToBPArgContext(Guid accountDefinitionId, Account account)
            {
                _accountDefinitionId = accountDefinitionId;
                if (account == null)
                    throw new ArgumentNullException("account");
                _account = account;
                _accountId = account.AccountId;
            }

            public Guid AccountBEDefinitionId
            {
                get { return _accountDefinitionId; }
            }

            public long AccountId
            {
                get { return _accountId; }
            }

            Account _account;
            public Account Account
            {
                get
                {
                    if (_account == null)
                    {
                        _account = s_accountBEManager.GetAccount(_accountDefinitionId, _accountId);
                    }
                    return _account;
                }
            }

            public BaseProcessInputArgument BPInputArgument
            {
                set;
                get;
            }
        }

        private class AccountActionBackendExecutorExecuteContext : IAccountActionBackendExecutorExecuteContext
        {
            Guid _accountDefinitionId;
            long _accountId;
            static AccountBEManager s_accountBEManager = new AccountBEManager();
            public AccountActionBackendExecutorExecuteContext(Guid accountDefinitionId, long accountId)
            {
                _accountDefinitionId = accountDefinitionId;
                _accountId = accountId;
            }

            public AccountActionBackendExecutorExecuteContext(Guid accountDefinitionId, Account account)
            {
                _accountDefinitionId = accountDefinitionId;
                if (account == null)
                    throw new ArgumentNullException("account");
                _account = account;
                _accountId = account.AccountId;
            }

            public Guid AccountBEDefinitionId
            {
                get { return _accountDefinitionId; }
            }

            public long AccountId
            {
                get { return _accountId; }
            }

            Account _account;
            public Account Account
            {
                get
                {
                    if (_account == null)
                    {
                        _account = s_accountBEManager.GetAccount(_accountDefinitionId, _accountId);
                    }
                    return _account;
                }
            }
        }

        #endregion
    }


}
