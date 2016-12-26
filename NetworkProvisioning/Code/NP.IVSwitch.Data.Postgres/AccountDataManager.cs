﻿using NP.IVSwitch.Entities;
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
        {

        }
        public TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync IvSwitchSync { get; set; }
        protected override string GetConnectionString()
        {
            return IvSwitchSync.MasterConnectionString;
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
            account.WebSite = GetReaderValue<string>(reader, "web_site");
            account.BillingCycle = (Int16)reader["cycle_id"];
            account.TaxGroupId = (Int16)reader["tax_group_id"];
            account.PaymentTerms = (Int16)reader["pay_terms"];
            account.CurrentState = (State)(Int16)reader["state_id"];
            account.CreditLimit = (decimal)reader["credit_limit"];
            account.CreditThreshold = (decimal)reader["threshold_credit"];
            account.CurrentBalance = (decimal)reader["balance"];
            account.LogAlias = reader["log_alias"] as string;
            account.Address = reader["address"] as string;
            account.PeerVendorId = (int)reader["peer_account_id"];
            account.ChannelsLimit = (int)reader["channels_limit"];



            return account;
        }
        public void UpdateCustomerChannelLimit(int accountId)
        {
            string[] query =
            {
                string.Format(@";with sumChannels as 
                            (
                            select sum(channels_limit) sumCh,account_id  from users
                             where account_id = {0}
                            group by account_id
                            )
                            UPDATE accounts
                            SET channels_limit = sumChannels.sumCh
                            FROM
                             sumChannels
                            WHERE
                             sumChannels.account_id = accounts.account_id;", accountId)
            };
            ExecuteNonQuery(query);
        }
        public bool Insert(Account account, out int insertedId)
        {
            String cmdText = @"INSERT INTO accounts(type_id,first_name,last_name,company_name,contact_display,email,web_site,cycle_id,tax_group_id,
	                               pay_terms,state_id,credit_limit,threshold_credit,balance,log_alias,address,peer_account_id,channels_limit)
	                             SELECT @type_id,@first_name,@last_name, @company_name,@contact_display, @email,@web_site,@cycle_id,@tax_group_id,
	                                    @pay_terms, @state_id,@credit_limit,@threshold_credit,@balance,@log_alias,@address,@peer_account_id,@channels_limit
	                             WHERE (NOT EXISTS(SELECT 1 FROM accounts WHERE  type_id = @type_id and email = @email))
	                             returning  account_id;";

            var accountId = ExecuteScalarText(cmdText, cmd =>
            {
                cmd.Parameters.AddWithValue("@type_id", (int)account.TypeId);
                cmd.Parameters.AddWithValue("@first_name", account.FirstName);
                cmd.Parameters.AddWithValue("@last_name", account.LastName);
                cmd.Parameters.AddWithValue("@company_name", account.CompanyName);
                cmd.Parameters.AddWithValue("@contact_display", account.ContactDisplay);
                cmd.Parameters.AddWithValue("@email", account.Email);
                var prmWebSite = new Npgsql.NpgsqlParameter("@web_site", DbType.String)
                {
                    Value = CheckIfNull(account.WebSite)
                };
                cmd.Parameters.Add(prmWebSite);
                cmd.Parameters.AddWithValue("@cycle_id", account.BillingCycle);
                cmd.Parameters.AddWithValue("@tax_group_id", account.TaxGroupId);
                cmd.Parameters.AddWithValue("@pay_terms", account.PaymentTerms);
                cmd.Parameters.AddWithValue("@state_id", (int)account.CurrentState);
                cmd.Parameters.AddWithValue("@credit_limit", -1);
                cmd.Parameters.AddWithValue("@threshold_credit", account.CreditThreshold);
                cmd.Parameters.AddWithValue("@balance", account.CurrentBalance);
                cmd.Parameters.AddWithValue("@log_alias", account.LogAlias);
                var prmAddress = new Npgsql.NpgsqlParameter("@address", DbType.String)
                {
                    Value = CheckIfNull(account.Address)
                };
                cmd.Parameters.Add(prmAddress);
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
            return false;
        }

        private void MapEnum(Account account, out int currentState, out int typeId)
        {
            var currentStateValue = Enum.Parse(typeof(State), account.CurrentState.ToString());
            currentState = (int)currentStateValue;

            var typeIdValue = Enum.Parse(typeof(AccountType), account.TypeId.ToString());
            typeId = (int)typeIdValue;

        }

        private Object CheckIfNull(String parameter)
        {

            return (String.IsNullOrEmpty(parameter)) ? (Object)DBNull.Value : parameter;

        }


    }
}