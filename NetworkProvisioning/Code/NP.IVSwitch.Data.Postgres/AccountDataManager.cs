using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class AccountDataManager : BasePostgresDataManager, IAccountDataManager
    {
        public AccountDataManager()
            : base(GetConnectionStringName("NetworkProvisioningDBConnStringKey", "NetworkProvisioningDBConnString"))
        {

        }

        private Account AccountMapper(IDataReader reader)
        {

            Account account = new Account();

            account.AccountId = (int)reader["account_id"];
            account.TypeId = (AccountType)(Int16)reader["type_id"];
            account.FirstName = reader["first_name"] as string;
            account.LastName = reader["last_name"] as string;
            account.CompanyName = reader["company_name"] as string;
            account.ContactDisplay = reader["contact_display"] as string;
            account.Email = reader["email"] as string;
            account.WebSite = reader["web_site"] as string;
            account.BillingCycle =  (Int16)reader["cycle_id"];
            account.TaxGroupId =  (Int16)reader["tax_group_id"];
            account.PaymentTerms =  (Int16)reader["pay_terms"];
            account.CurrentState = (State)(Int16)reader["state_id"];
            account.CreditLimit = (decimal)reader["credit_limit"];
            account.CreditThreshold = (decimal)reader["threshold_credit"];
            account.CurrentBalance = (decimal)reader["balance"];
            account.LogAlias =  reader["log_alias"] as string;
            account.Address = reader["address"] as string;
            account.PeerVendorId = (int)reader["peer_account_id"];
            account.ChannelsLimit = (int)reader["channels_limit"];



            return account;
        }


        public List<Account> GetAccounts()
        {
            String cmdText = @"SELECT  account_id,type_id,first_name,last_name,company_name, contact_display,email,web_site,cycle_id,tax_group_id,pay_terms,state_id,credit_limit,
                                       threshold_credit,balance,log_alias,address,peer_account_id,channels_limit            
                                       FROM accounts;";
            return GetItemsText(cmdText, AccountMapper, (cmd) =>
            {
            });
        }

        public bool Update(Account account)
        {

            int currentState, typeId;

            MapEnum(account, out currentState, out  typeId);

            String cmdText = @"UPDATE accounts
	                             SET type_id = @type_id, first_name = @first_name, last_name = @last_name, company_name = @company_name,
	                               contact_display = @contact_display, email = @email, web_site = @web_site, cycle_id = @cycle_id, tax_group_id = @tax_group_id,
	                               pay_terms = @pay_terms, state_id = @state_id, credit_limit = @credit_limit, threshold_credit = @threshold_credit, balance = @balance,
	                               log_alias = @log_alias, address = @address, peer_account_id = @peer_account_id, channels_limit = @channels_limit
                                   WHERE  account_id = @account_id and  NOT EXISTS(SELECT 1 FROM  accounts WHERE account_id != @account_id and type_id = @type_id and email = @email);";

            int recordsEffected = ExecuteNonQueryText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@account_id", account.AccountId);
                cmd.Parameters.AddWithValue("@type_id",  typeId);
                cmd.Parameters.AddWithValue("@first_name", account.FirstName);
                cmd.Parameters.AddWithValue("@last_name", account.LastName);
                cmd.Parameters.AddWithValue("@company_name", account.CompanyName);
                cmd.Parameters.AddWithValue("@contact_display", account.ContactDisplay);
                cmd.Parameters.AddWithValue("@email", account.Email);
                cmd.Parameters.AddWithValue("@web_site", account.WebSite);
                cmd.Parameters.AddWithValue("@cycle_id", account.BillingCycle);
                cmd.Parameters.AddWithValue("@tax_group_id", account.TaxGroupId);
                cmd.Parameters.AddWithValue("@pay_terms", account.PaymentTerms);
                cmd.Parameters.AddWithValue("@state_id",  currentState);
                cmd.Parameters.AddWithValue("@credit_limit", account.CreditLimit);
                cmd.Parameters.AddWithValue("@threshold_credit", account.CreditThreshold);
                cmd.Parameters.AddWithValue("@balance", account.CurrentBalance);
                cmd.Parameters.AddWithValue("@log_alias", account.LogAlias);
                cmd.Parameters.AddWithValue("@address", account.Address);
                cmd.Parameters.AddWithValue("@peer_account_id", account.PeerVendorId);
                cmd.Parameters.AddWithValue("@channels_limit", account.ChannelsLimit);

            }
           );
            return (recordsEffected > 0);
        }

        public bool Insert(Account account, out int insertedId)
        {
            object accountId;
            int currentState, typeId;

            MapEnum(account, out currentState, out  typeId);


            String cmdText = @"INSERT INTO accounts(type_id,first_name,last_name,company_name,contact_display,email,web_site,cycle_id,tax_group_id,
	                               pay_terms,state_id,credit_limit,threshold_credit,balance,log_alias,address,peer_account_id,channels_limit)
	                             SELECT @type_id,@first_name,@last_name, @company_name,@contact_display, @email,@web_site,@cycle_id,@tax_group_id,
	                                    @pay_terms, @state_id,@credit_limit,@threshold_credit,@balance,@log_alias,@address,@peer_account_id,@channels_limit
	                             WHERE (NOT EXISTS(SELECT 1 FROM accounts WHERE  type_id = @type_id and email = @email))
	                             returning  account_id;";

            accountId = ExecuteScalarText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@type_id", typeId);
                cmd.Parameters.AddWithValue("@first_name", account.FirstName);
                cmd.Parameters.AddWithValue("@last_name", account.LastName);
                cmd.Parameters.AddWithValue("@company_name", account.CompanyName);
                cmd.Parameters.AddWithValue("@contact_display", account.ContactDisplay);
                cmd.Parameters.AddWithValue("@email", account.Email);
                cmd.Parameters.AddWithValue("@web_site", account.WebSite);
                cmd.Parameters.AddWithValue("@cycle_id", account.BillingCycle);
                cmd.Parameters.AddWithValue("@tax_group_id", account.TaxGroupId);
                cmd.Parameters.AddWithValue("@pay_terms", account.PaymentTerms);
                cmd.Parameters.AddWithValue("@state_id", currentState);
                cmd.Parameters.AddWithValue("@credit_limit", account.CreditLimit);
                cmd.Parameters.AddWithValue("@threshold_credit", account.CreditThreshold);
                cmd.Parameters.AddWithValue("@balance", account.CurrentBalance);
                cmd.Parameters.AddWithValue("@log_alias", account.LogAlias);
                cmd.Parameters.AddWithValue("@address", account.Address);
                cmd.Parameters.AddWithValue("@peer_account_id", account.PeerVendorId);
                cmd.Parameters.AddWithValue("@channels_limit", account.ChannelsLimit);

            }
            );

            insertedId = -1;
            if (accountId != null)
            {
                insertedId = Convert.ToInt32(accountId);
                return true;
            }
            else
                return false;

        }

        private void MapEnum(Account account, out int currentState, out int typeId )
        {
            var currentStateValue = Enum.Parse(typeof(State), account.CurrentState.ToString());
            currentState = (int)currentStateValue;

            var typeIdValue = Enum.Parse(typeof(AccountType), account.TypeId.ToString());
            typeId = (int)typeIdValue;

        }


    }
}