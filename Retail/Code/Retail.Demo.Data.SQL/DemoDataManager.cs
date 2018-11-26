using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Retail.Demo.Data;
using Retail.Demo.Entities;
using System.Data;

namespace Retail.Demo.Data.SQL
{
    public class DemoDataManager : BaseSQLDataManager, IDemoDataManager
    {
        public DemoDataManager()
           : base(GetConnectionStringName("ISPDBConnString", "ISPDBConnString"))
        {

        }
        public List<ActiveServices> GetActiveServices()
        {
            List<ActiveServices> activeServices = new List<ActiveServices>();

            string query = string.Format(query_GetActiveServices.ToString());
            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    ActiveServices activeService = ActiveServicesMapper(reader);
                    activeServices.Add(activeService);

                }
            }, null);
            return activeServices;
        }

        public List<NewOrders> GetNewOrders()
        {
            List<NewOrders> newOrders = new List<NewOrders>();

            string query = string.Format(query_GetNewOrders.ToString());
            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    NewOrders newOrder = NewOrdersMapper(reader);
                    newOrders.Add(newOrder);

                }
            }, null);
            return newOrders;
        }

        #region Mappers


        #endregion
        private ActiveServices ActiveServicesMapper(IDataReader reader)
        {

            return new ActiveServices()
            {
                ProductName = reader["Name"] as string,
                Cycle = reader["BillingFrequency"] as string,
                TotalTarrif = (decimal)reader["MRC"]
            };
        }
        private NewOrders NewOrdersMapper(IDataReader reader)
        {

            return new NewOrders()
            {
                ProductName = reader["Name"] as string,
                TotalTarrif = (decimal)reader["NRC"]
            };
        }
        #region Queries

        private StringBuilder query_GetActiveServices = new StringBuilder(@"use[Retail_Dev_ISP];
                                                                            select Pr.Name  as ProductName, od.BillingFrequency, Sum(od.MRC) as MRC
                                                                            FROM[Retail_Dev_ISP].[NetworkRentalManager].[OrdersDefinition] as od
                                                                            Join[Retail_Dev_ISP].[NetworkRentalManager].[Product] as Pr on OD.ProductId=pr.ID
                                                                            Group by Pr.Name,od.BillingFrequency
                                                                            ");

        private StringBuilder query_GetNewOrders = new StringBuilder(@"use[Retail_Dev_ISP];
                                                                       select Pr.Name  as ProductName, Sum(od.MRC) as NRC
                                                                       FROM[Retail_Dev_ISP].[NetworkRentalManager].[OrdersDefinition] as od
                                                                       JOin[Retail_Dev_ISP].[NetworkRentalManager].[Product] as Pr on OD.ProductId=pr.ID
                                                                       Group by Pr.Name,od.BillingFrequency
                                                                       ");

        #endregion
    }
}
