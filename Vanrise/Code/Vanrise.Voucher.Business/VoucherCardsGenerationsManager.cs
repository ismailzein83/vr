using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Voucher.Entities;
using Vanrise.Common;

namespace Vanrise.Voucher.Business
{
    public class VoucherCardsGenerationsManager
    {
       public static Guid _definitionId = new Guid("16fba68f-d080-4a5e-a7a8-054312ae7b6b");
        GenericBusinessEntityManager _genericBEDManager= new GenericBusinessEntityManager();

         public VoucherCardsGeneration GetVoucherCardsGeneration(long VoucherCardsGenerationId)
        {
            return GetCachedVoucherCardsGeneration().GetRecord(VoucherCardsGenerationId);
        }
        private Dictionary<long, VoucherCardsGeneration> GetCachedVoucherCardsGeneration()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedVoucherCardsGeneration", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<long, VoucherCardsGeneration> result = new Dictionary<long, VoucherCardsGeneration>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        VoucherCardsGeneration voucherCardsGeneration = new VoucherCardsGeneration()
                        {
                            VoucherTypeId = (long)genericBusinessEntity.FieldValues.GetRecord("VoucherTypeId"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            VoucherCardsGenerationId = (long)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            NumberOfCards = (int)genericBusinessEntity.FieldValues.GetRecord("NumberOfCards"),
                            ExpiryDate = (DateTime)genericBusinessEntity.FieldValues.GetRecord("ExpiryDate"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy"),
                            InactiveCards = (int?)genericBusinessEntity.FieldValues.GetRecord("InactiveCards")

                        };
                        result.Add(voucherCardsGeneration.VoucherCardsGenerationId, voucherCardsGeneration);
                    }
                }

                return result;
            });
        }
    }
}
