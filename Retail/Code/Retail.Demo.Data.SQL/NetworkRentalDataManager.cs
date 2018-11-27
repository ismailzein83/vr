using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Retail.Demo.Data;
using Retail.Demo.Entities;
using System.Data;
using Retail.BusinessEntity.Entities;

namespace Retail.Demo.Data.SQL
{
    public class NetworkRentalDataManager : BaseSQLDataManager, IDemoDataManager
    {
        public NetworkRentalDataManager()
           : base(GetConnectionStringName("ISPDBConnStringKey", "ISPDBConnString"))
        {

        }

        public List<ActiveServices> GetActiveServices(long accountId, DateTime fromDate, DateTime toDate)
        {
            List<ActiveServices> activeServices = new List<ActiveServices>();

            string query = string.Format(query_GetActiveServices.ToString());
            query = query.Replace("#AccountID#", accountId.ToString());
            query = query.Replace("#FromDate#", fromDate.ToString());
            query = query.Replace("#ToDate#", toDate.ToString());

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

        public List<NewOrders> GetNewOrders(long accountId, DateTime fromDate, DateTime toDate)
        {
            List<NewOrders> newOrders = new List<NewOrders>();

            string query = string.Format(query_GetNewOrders.ToString());
            query = query.Replace("#AccountID#", accountId.ToString());
            query = query.Replace("#FromDate#", fromDate.ToString());
            query = query.Replace("#ToDate#", toDate.ToString());

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
                ProductName = reader["ProductName"] as string,
                NoOfOrders = (int)reader["NoOfOrders"],
                Cycle = reader["BillingFrequency"] as string,
                TotalTarrif = (decimal)reader["MRC"]
            };
        }
        private NewOrders NewOrdersMapper(IDataReader reader)
        {

            return new NewOrders()
            {
                ProductName = reader["ProductName"] as string,
                NoOfOrders = (int)reader["NoOfOrders"],
                TotalTarrif = (decimal)reader["NRC"]
            };
        }
        #region Queries

        private StringBuilder query_GetActiveServices = new StringBuilder(@"select Pr.Name  as ProductName,count(Pr.Name) as NoOfOrders, od.BillingFrequency, Sum(od.MRC) as MRC
                                                                            FROM [NetworkRentalManager].[OrdersDefinition] as od
                                                                            Join [NetworkRentalManager].[Product] as Pr on OD.ProductId=pr.ID
                                                                            where AccountID =#AccountID# and od.BillStartDate >= '#FromDate#' and od.BillStartDate < '#ToDate#'
                                                                            Group by Pr.Name,od.BillingFrequency
                                                                            ");

        private StringBuilder query_GetNewOrders = new StringBuilder(@"select Pr.Name  as ProductName,count(Pr.Name) as NoOfOrders, Sum(od.NRC) as NRC
                                                                       FROM [NetworkRentalManager].[OrdersDefinition] as od
                                                                       Join [NetworkRentalManager].[Product] as Pr on OD.ProductId=pr.ID
                                                                       where AccountID =#AccountID# and od.BillStartDate >= '#FromDate#' and od.BillStartDate < '#ToDate#'
                                                                       Group by Pr.Name,od.BillingFrequency
                                                                       ");

        #endregion
    }
}
