using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace TOne.WhS.Jazz.Business
{
    public class WhsJazzTaxCodeManager
    {
        //public static Guid _definitionId = new Guid("EA7443B5-EEC8-4A51-AA26-3A15FE9B2ABE");
        //GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        //public IEnumerable<WhSJazzTax> GetAllTaxes()
        //{
        //   return GetCachedTaxes().Values;
        //}


        //private Dictionary<Guid, WhSJazzTax> GetCachedTaxes()
        //{
        //    GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
        //    return genericBusinessEntityManager.GetCachedOrCreate("GetCachedTaxes", _definitionId, () =>
        //    {
        //        List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
        //        Dictionary<Guid, WhSJazzTax> result = new Dictionary<Guid, WhSJazzTax>();

        //        if (genericBusinessEntities != null)
        //        {
        //            foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
        //            {
        //                if (genericBusinessEntity.FieldValues == null)
        //                    continue;

        //                WhSJazzTax tax = new WhSJazzTax()
        //                {
        //                    ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
        //                    SwitchId = (int)genericBusinessEntity.FieldValues.GetRecord("SwitchId"),
        //                    Type = (TaxTypeEnum)genericBusinessEntity.FieldValues.GetRecord("Type"),
        //                    CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
        //                    CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
        //                    LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
        //                    LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

        //                };
        //                result.Add(tax.ID, tax);
        //            }
        //        }

        //        return result;
        //    });
        //}

    }

    public class WhSJazzTax
    {
        public Guid ID { get; set; }
        public int SwitchId { get; set; }
        public TaxTypeEnum Type { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public int CreatedBy { get; set; }
    }
    public enum TaxTypeEnum {In=1,Out=2 }
}
