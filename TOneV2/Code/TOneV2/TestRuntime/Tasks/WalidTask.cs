using System;
using System.Collections.Generic;
using TOne.WhS.DBSync.Business;
using TOne.WhS.DBSync.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TestRuntime.Tasks
{
    public class WalidTask : ITask
    {
        public void Execute()
        {
            //var runtimeServices = new List<RuntimeService>();
            //SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(schedulerService);
            //RuntimeHost host = new RuntimeHost(runtimeServices);
            //host.Start();

            List<DBTableName> migrationTables = new List<DBTableName>();
            string tableName;

            foreach (DBTableName table in Enum.GetValues(typeof(DBTableName)))
            {
                tableName = Vanrise.Common.Utilities.GetEnumDescription(table);

                switch (table)
                {
                    case DBTableName.CarrierAccount:
                        migrationTables.Add(table);
                        break;


                    case DBTableName.CarrierProfile:
                        migrationTables.Add(table);
                        break;


                    case DBTableName.Currency:
                        migrationTables.Add(table);
                        break;

                    case DBTableName.CurrencyExchangeRate:
                        migrationTables.Add(table);
                        break;

                    case DBTableName.Switch:
                        migrationTables.Add(table);
                        break;
                    case DBTableName.Country:
                        migrationTables.Add(table);
                        break;

                    case DBTableName.CodeGroup:
                        migrationTables.Add(table);
                        break;

                    case DBTableName.SupplierCode:
                        migrationTables.Add(table);
                        break;

                    case DBTableName.SupplierPriceList:
                        migrationTables.Add(table);
                        break;
                    case DBTableName.SupplierRate:
                        migrationTables.Add(table);
                        break;

                    case DBTableName.SupplierZone:
                        migrationTables.Add(table);
                        break;

                    case DBTableName.SaleCode:
                        migrationTables.Add(table);
                        break;

                    case DBTableName.SalePriceList:
                        migrationTables.Add(table);
                        break;

                    case DBTableName.SaleRate:
                        migrationTables.Add(table);
                        break;

                    case DBTableName.SaleZone:
                        migrationTables.Add(table);
                        break;
                    case DBTableName.File:
                        migrationTables.Add(table);
                        break;
                    case DBTableName.CustomerCountry:
                        migrationTables.Add(table);
                        break;
                    case DBTableName.CustomerSellingProduct:
                        migrationTables.Add(table);
                        break;
                    case DBTableName.SwitchConnectivity:
                        migrationTables.Add(table);
                        break;
                    case DBTableName.ZoneServiceConfig:
                        migrationTables.Add(table);
                        break;
                    case DBTableName.SupplierZoneService:
                        migrationTables.Add(table);
                        break;
                    case DBTableName.SaleEntityService:
                        migrationTables.Add(table);
                        break;
                    case DBTableName.Rule:
                        migrationTables.Add(table);
                        break;


                }
            }



            DBSyncTaskActionArgument taskActionArgument = new DBSyncTaskActionArgument();
            taskActionArgument.ConnectionString = "Server=192.168.110.185;Database=mvtspro;User ID=development;Password=dev!123;";
            taskActionArgument.DefaultSellingNumberPlanId = 1;
            taskActionArgument.SellingProductId = 1;
            taskActionArgument.OffPeakRateTypeId = 1;
            taskActionArgument.WeekendRateTypeId = 1;
            taskActionArgument.UseTempTables = true;
            taskActionArgument.MigrationRequestedTables = migrationTables;
            DBSyncTaskAction dbSyncTaskAction = new DBSyncTaskAction();
            dbSyncTaskAction.Execute(new SchedulerTask(), taskActionArgument, null);
        }
    }
}




