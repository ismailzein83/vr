using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Vanrise.Entities;

namespace Retail.EntitiesMigrator.Migrators
{
    public enum MonthlyChargeType
    {
        [Description("OTC")]
        OTC,
        [Description("Line Rent")]
        LineRent
    }
    public class MonthlyChargesMigrator
    {
        public void Execute()
        {
            List<MonthlyCharge> monthlyCharges = GetMonthlyCharges();

            Dictionary<string, Account> branches = new AccountBEManager().GetCachedAccountsBySourceId(Helper.AccountBEDefinitionId);


            Dictionary<string, Package> packages = new Dictionary<string, Package>();
            GetOrCreatePackages(monthlyCharges, packages);

            foreach (MonthlyCharge monthlyCharge in monthlyCharges)
            {
                Account branchAccount;
                if (branches.TryGetValue(Helper.GetBranchSourceId(monthlyCharge.BranchId.ToString()), out branchAccount))
                {
                    Package package = packages[Helper.GetPackageName(monthlyCharge)];
                    AccountPackageManager manager = new AccountPackageManager();
                    manager.AddAccountPackage(Helper.GetAccountPackageToAdd(monthlyCharge, branchAccount.AccountId, package.PackageId));
                }
            }
        }

        void GetOrCreatePackages(List<MonthlyCharge> monthlyCharges, Dictionary<string, Package> packages)
        {
            PackageManager packageManager = new PackageManager();
            var cachedPackages = packageManager.GetCachedPackages();
            foreach (var package in cachedPackages)
            {
                packages.Add(package.Value.Name, package.Value);
            }
            foreach (var monthlyCharge in monthlyCharges)
            {
                string name = Helper.GetPackageName(monthlyCharge);
                if (!packages.ContainsKey(name))
                {
                    Package package = Helper.CreatePackage(monthlyCharge, name);
                    InsertOperationOutput<PackageDetail> detail = packageManager.AddPackage(package);
                    package.PackageId = detail.InsertedObject.Entity.PackageId;
                    packages.Add(name, package);
                }
            }
        }



        List<MonthlyCharge> GetMonthlyCharges()
        {
            List<MonthlyCharge> result = new List<MonthlyCharge>();
            using (SqlConnection conn = new SqlConnection(Helper.RatesConnectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                SqlCommand command = conn.CreateCommand();
                command.CommandText = query_GetMonthlyCharge;
                command.CommandType = System.Data.CommandType.Text;
                List<long> addedLineRentBranchIds = new List<long>();
                List<long> addedOTCBranchIds = new List<long>();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    long branchId = (long)reader["AC_ACCOUNTNO"];
                    long LineRent = (long)reader["LINERENT"];
                    long OTC = (long)reader["OTC"];
                    if (LineRent > 0 && !addedLineRentBranchIds.Contains(branchId))
                    {
                        result.Add(GetMonthlyCharge(LineRent, reader, MonthlyChargeType.LineRent));
                        addedLineRentBranchIds.Add(branchId);
                    }
                    if (OTC > 0 && !addedOTCBranchIds.Contains(branchId))
                    {
                        result.Add(GetMonthlyCharge(OTC, reader, MonthlyChargeType.OTC));
                        addedOTCBranchIds.Add(branchId);
                    }
                }
                reader.Close();
                conn.Close();
            }

            return result;
        }

        private MonthlyCharge GetMonthlyCharge(long LineRent, SqlDataReader reader, MonthlyChargeType monthlyChargeType)
        {
            return new MonthlyCharge
            {
                ActivationDate = (DateTime)reader["SSFC_ACTIVATIONDATE"],
                BranchId = (long)reader["AC_ACCOUNTNO"],
                Price = LineRent,
                Type = monthlyChargeType
            };
        }



        const string query_GetMonthlyCharge = @"SELECT [SUBSCRIBERID]
                                                      ,[OTC]
                                                      ,[LINERENT]
                                                      ,[SSFC_ACTIVATIONDATE]
                                                      ,[AC_ACCOUNTNO]
                                                  FROM [vw_MultiNet_MonthlyCharges]";

    }

    public class MonthlyCharge
    {
        public MonthlyChargeType Type { get; set; }
        public long BranchId { get; set; }
        public DateTime ActivationDate { get; set; }
        public long Price { get; set; }
    }
}
