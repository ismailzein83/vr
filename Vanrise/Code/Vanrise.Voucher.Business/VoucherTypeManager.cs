using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Voucher.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
namespace Vanrise.Voucher.Business
{
    public class VoucherTypeManager
    {
        static Guid _definitionId = new Guid("73fa7201-b92b-4856-89f3-a38afb53323b");

        public VoucherType GetVoucherType(long voucherTypeId)
        {
            return GetCachedVoucherTypes().GetRecord(voucherTypeId);
        }
        private Dictionary<long,VoucherType> GetCachedVoucherTypes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedSecurityProviders", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<long, VoucherType> result = new Dictionary<long, VoucherType>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        VoucherType voucherType = new VoucherType()
                        {
                            VoucherTypeId = (long)genericBusinessEntity.FieldValues.GetRecord("VoucherTypeId"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Amount = (decimal)genericBusinessEntity.FieldValues.GetRecord("Amount"),
                            CurrencyId = (int)genericBusinessEntity.FieldValues.GetRecord("CurrencyId"),
                            Description = (string)genericBusinessEntity.FieldValues.GetRecord("Description"),
                            Active = (bool)genericBusinessEntity.FieldValues.GetRecord("Active") ,
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy"),

                        };
                        result.Add(voucherType.VoucherTypeId, voucherType);
                    }
                }

                return result;
            });
        }

    }
}
