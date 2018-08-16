using BPMExtended.Main.Data;
using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class CustomerRequestTypeManager
    {
        //static ICustomerRequestTypeDataManager s_dataManager = new BPMExtended.Main.Data.SQL.CustomerRequestTypeDataManager();// BPMExtendedDataManagerFactory.GetDataManager<ICustomerRequestTypeDataManager>();
        //public List<CustomerRequestTypeInfo> GetCustomerRequestTypeInfos(BPMCustomerType customerType, Guid accountOrContactId)
        //{
        //    return GetCustomerRequestTypes().Values.MapRecords(itm => new CustomerRequestTypeInfo
        //    {
        //        CustomerRequestTypeId = itm.CustomerRequestTypeId,
        //        Name = itm.Name,
        //        PageURL = itm.Settings != null ? itm.Settings.PageURL : null
        //    }).ToList();
        //}

        #region Public Methods

        public Dictionary<Guid, CustomerRequestType> GetCustomerRequestTypes()
        {
            List<CustomerRequestType> customerRequestTypes = new List<CustomerRequestType>();

            customerRequestTypes.Add(new CustomerRequestType
            {
                CustomerRequestTypeId = new Guid("31E1AFF4-D7F2-4C30-BDF1-BC7D965E8B20"),
                Name = "Line Subscription",
                Settings = new CustomerRequestTypeSettings { PageURL = "/CustomerOrders/LineSubscription" }
            });
            customerRequestTypes.Add(new CustomerRequestType
            {
                CustomerRequestTypeId = new Guid("31E1AFF4-D7F2-4C30-BDF1-BC7D965E8B20"),
                Name = "Telephony Line Subscription",
                Settings = new CustomerRequestTypeSettings { PageURL = "/CustomerOrders/LineSubscription" }
            });
            customerRequestTypes.Add(new CustomerRequestType
            {
                CustomerRequestTypeId = new Guid("461B7474-9B19-4B90-AEAB-63BA37245E53"),
                Name = "Line Subscription Termination",
                Settings = new CustomerRequestTypeSettings { PageURL = "/CustomerOrders/LineSubscriptionTermination" }
            });
            customerRequestTypes.Add(new CustomerRequestType
            {
                CustomerRequestTypeId = new Guid("900B0866-A871-4974-ACA9-1D3FB16DCB45"),
                Name = "Move Line",
                Settings = new CustomerRequestTypeSettings { PageURL = "/CustomerOrders/MoveLine" }
            });
            return customerRequestTypes.ToDictionary(cn => cn.CustomerRequestTypeId, cn => cn); ;
            //return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCustomerRequestTypes",
            //   () =>
            //   {                   
            //List<CustomerRequestType> customerRequestTypes = s_dataManager.GetCustomerRequestTypes();
            //return customerRequestTypes.ToDictionary(cn => cn.CustomerRequestTypeId, cn => cn);
            //});
        }

        #endregion

        #region Private Classes

        //private class CacheManager : Vanrise.Caching.BaseCacheManager
        //{
        //    ICustomerRequestTypeDataManager _dataManager = BPMExtendedDataManagerFactory.GetDataManager<ICustomerRequestTypeDataManager>();
        //    object _updateHandle;

        //    protected override bool ShouldSetCacheExpired(object parameter)
        //    {
        //        return _dataManager.AreCustomerRequestTypesUpdated(ref _updateHandle);
        //    }
        //}

        #endregion
    }
}
