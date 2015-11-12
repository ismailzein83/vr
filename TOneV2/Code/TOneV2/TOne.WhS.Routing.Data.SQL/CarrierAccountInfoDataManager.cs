﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CarrierAccountInfoDataManager : RoutingDataManager, ICarrierAccountInfoDataManager
    {
        public IEnumerable<RoutingCustomerInfo> GetRoutingtCustomerInfo()
        {
            return GetItemsText(query_GetRoutingCustomerInfo, RoutingCustomerInfoMapper, null);
        }
        public IEnumerable<RoutingSupplierInfo> GetRoutingtSupplierInfo()
        {
            return GetItemsText(query_GetRoutingSupplierInfo, RoutingSupplierInfoMapper, null);
        }
        public void SaveRoutingCustomerInfo(IEnumerable<RoutingCustomerInfo> customerInfos)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in customerInfos)
                {
                    wr.WriteLine(String.Format("{0}", c.CustomerId));
                }
                wr.Close();
            }

            ApplyRoutingCustomerInfoToDB(new BulkInsertInfo
            {
                TableName = "RoutingCustomerInfo",
                DataFilePath = filePath,
                TabLock = true,
                FieldSeparator = '^'
            });
        }
        public void SaveRoutingSupplierInfo(IEnumerable<RoutingSupplierInfo> supplierInfos)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var s in supplierInfos)
                {
                    wr.WriteLine(String.Format("{0}", s.SupplierId));
                }
                wr.Close();
            }

            ApplyRoutingSupplierInfoToDB(new BulkInsertInfo
            {
                TableName = "RoutingSupplierInfo",
                DataFilePath = filePath,
                TabLock = true,
                FieldSeparator = '^'
            });
        }
        void ApplyRoutingCustomerInfoToDB(object preparedCustomerInfos)
        {
            InsertBulkToTable(preparedCustomerInfos as BulkInsertInfo);
        }
        void ApplyRoutingSupplierInfoToDB(object preparedSupplierInfos)
        {
            InsertBulkToTable(preparedSupplierInfos as BulkInsertInfo);
        }
        RoutingCustomerInfo RoutingCustomerInfoMapper(IDataReader reader)
        {
            return new RoutingCustomerInfo()
            {
                CustomerId = (int)reader["CustomerId"]
            };
        }
        RoutingSupplierInfo RoutingSupplierInfoMapper(IDataReader reader)
        {
            return new RoutingSupplierInfo()
            {
                SupplierId = (int)reader["SupplierId"]
            };
        }

        #region Queries

        const string query_GetRoutingCustomerInfo = @"                                                       
                                            SELECT  c.CustomerID                                        
                                            FROM  [RoutingCustomerInfo] c";
        const string query_GetRoutingSupplierInfo = @"                                                       
                                            SELECT  s.SupplierId                                        
                                            FROM  [RoutingSupplierInfo] s
                                        ";
        #endregion

    }
}
