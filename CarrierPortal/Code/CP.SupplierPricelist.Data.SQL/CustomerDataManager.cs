﻿using System.Collections.Generic;
using CP.SupplierPricelist.Entities;
using Vanrise.Data.SQL;
using System.Data;
using Vanrise.Common;

namespace CP.SupplierPricelist.Data.SQL
{
    public class CustomerDataManager : BaseSQLDataManager, ICustomerDataManager
    {
        public List<Customer> GetCustomers(ref byte[] maxTimeStamp, int nbOfRows)
        {
            List<Customer> customers = new List<Customer>();
            byte[] timestamp = null;

            ExecuteReaderSP("CP_SupPriceList.sp_PriceList_GetUpdated", (reader) =>
            {
                while (reader.Read())
                    customers.Add(CustomerMapper(reader));
                if (reader.NextResult())
                    while (reader.Read())
                        timestamp = GetReaderValue<byte[]>(reader, "MaxTimestamp");
            },
               maxTimeStamp, nbOfRows);
            maxTimeStamp = timestamp;
            return customers;
        }
        Customer CustomerMapper(IDataReader reader)
        {
            Customer customer = new Customer
            {
                CustomerId = (int)reader["ID"],
                Name = reader["Name"] as string,
            };
            string settings = reader["Settings"] as string;
            if (settings != null)
                customer.Settings = Serializer.Deserialize<CustomerSettings>(settings);
            return customer;
        }
    }
}
