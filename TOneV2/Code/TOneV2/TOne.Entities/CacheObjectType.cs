using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.Entities
{
    public enum CacheObjectType { SupplierCodes, Pricing, TempObjects, SharedMemoryQueue, Zone, CarrierAccount, FlaggedService,Switch,CodeGroup, CarrierGroup }
    //public class CacheObjectTypes : ICacheObjectType
    //{
    //    public CacheObjectTypes(CacheObjectType objectType)
    //    {
    //        _typeUniqueValue = ((int)objectType).ToString();
    //    }

    //    #region ICacheObjectType Members

    //    string _typeUniqueValue;
    //    public string TypeUniqueValue
    //    {
    //        get { return _typeUniqueValue; }
    //    }

    //    #endregion

    //    static CacheObjectTypes _supplierCodes = new CacheObjectTypes(CacheObjectType.SupplierCodes);
    //    public static CacheObjectTypes SupplierCodes
    //    {
    //        get
    //        {
    //            return _supplierCodes;
    //        }
    //    }

    //    static CacheObjectTypes _pricing = new CacheObjectTypes(CacheObjectType.Pricing);
    //    public static CacheObjectTypes Pricing
    //    {
    //        get
    //        {
    //            return _pricing;
    //        }
    //    }

    //    static CacheObjectTypes _tempObjects = new CacheObjectTypes(CacheObjectType.TempObjects);
    //    public static CacheObjectTypes TempObjects
    //    {
    //        get
    //        {
    //            return _tempObjects;
    //        }
    //    }

    //}
}
