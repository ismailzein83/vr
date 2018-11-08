using System;
using System.Collections.Generic;
using Vanrise.Data.SQL;

namespace Vanrise.Common.Data.SQL
{
    public class CurrencyExchangeRateWithEEDDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ICurrencyExchangeRateWithEEDDataManager
    {
        readonly string[] columns = { "CurrencyID", "Rate", "BED", "EED" };
        public void ApplyExchangeRateWithEESInDB(List<Vanrise.Entities.ExchangeRateWithEED> exchangeRates)
        {
            ExecuteNonQueryText(@" 
                                 IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CurrencyExchangeRate_Old]') AND type in (N'U'))
                                 BEGIN
                                    DROP TABLE [dbo].[CurrencyExchangeRate_Old]
                                 END

                                 IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CurrencyExchangeRate_Temp]') AND type in (N'U'))
                                 BEGIN
                                    DROP TABLE [dbo].[CurrencyExchangeRate_Temp]
                                 END

                                CREATE TABLE [dbo].[CurrencyExchangeRate_Temp] (
                                CurrencyID int NOT NULL,
                                Rate Decimal(18,6) NOT NULL,
                                BED DATETIME NOT NULL,
                                EED DATETIME) "
               , null);
            var streamForBulkInsert = base.InitializeStreamForBulkInsert();
            foreach (var rate in exchangeRates)
            {
                streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}", rate.CurrencyId, Math.Round(rate.Rate, 6), GetDateTimeForBCP(rate.BED), GetDateTimeForBCP(rate.EED));
            }
            streamForBulkInsert.Close();
            var bulkInsertInfo = new StreamBulkInsertInfo
            {
                TableName = "[dbo].[CurrencyExchangeRate_Temp]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns,
            };
            InsertBulkToTable(bulkInsertInfo);

            ExecuteNonQueryText(@" BEGIN TRY
                                      BEGIN TRANSACTION 
                                        IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CurrencyExchangeRate]') AND type in (N'U'))
		                                BEGIN
		                                    EXEC sp_rename N'CurrencyExchangeRate', N'CurrencyExchangeRate_Old';
	                                    END                                     
                                        EXEC sp_rename N'CurrencyExchangeRate_Temp', N'CurrencyExchangeRate';
                                    
	                                  COMMIT ;
                                    END TRY
                                    BEGIN CATCH

                                        IF @@TRANCOUNT > 0
                                            ROLLBACK
                                    END CATCH

                                    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CurrencyExchangeRate_Old]') AND type in (N'U'))
		                            BEGIN
		                                DROP TABLE [dbo].[CurrencyExchangeRate_Old]
	                                END "
           , null);


        }

        protected override string GetConnectionString()
        {
            return !String.IsNullOrEmpty(_connectionString) ? _connectionString : Common.Utilities.GetExposedConnectionString(_connectionStringName);
        }

        string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        string _connectionStringName;
        public string ConnectionStringName
        {
            set { _connectionStringName = value; }
        }
    }
}
