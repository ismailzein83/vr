using BPMExtended.Main.Data;
using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace BPMExtended.Main.Business
{
    public class CustomerRequestTypeManager
    {
        static ICustomerRequestTypeDataManager s_dataManager = BPMExtendedDataManagerFactory.GetDataManager<ICustomerRequestTypeDataManager>();
        public List<CustomerRequestTypeInfo> GetCustomerRequestTypeInfos(CustomerObjectType customerObjectType, Guid accountOrContactId)
        {
            return GetCustomerRequestTypes().Values.MapRecords(itm => new CustomerRequestTypeInfo
            {
                CustomerRequestTypeId = itm.CustomerRequestTypeId,
                Name = itm.Name,
                PageURL = itm.Settings != null ? itm.Settings.PageURL : null
            }).ToList();
        }

        #region Public Methods

        public Dictionary<Guid, CustomerRequestType> GetCustomerRequestTypes()
        {
            //return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCustomerRequestTypes",
            //   () =>
            //   {                   
                   List<CustomerRequestType> customerRequestTypes = s_dataManager.GetCustomerRequestTypes();
                   return customerRequestTypes.ToDictionary(cn => cn.CustomerRequestTypeId, cn => cn);
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
