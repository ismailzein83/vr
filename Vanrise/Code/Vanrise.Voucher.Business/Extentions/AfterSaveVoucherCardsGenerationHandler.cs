using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Common;
namespace Vanrise.Voucher.Business
{
    public class AfterSaveVoucherCardsGenerationHandler : GenericBEOnAfterSaveHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("C7A64432-7DED-4333-9B65-D71AC055ABAA"); }
        }

        public override void Execute(IGenericBEOnAfterSaveHandlerContext context)
        {
            VoucherCardsManager voucherCardsManager = new VoucherCardsManager();

            Object generationVoucherIdObject;
            context.NewEntity.FieldValues.TryGetValue("ID", out generationVoucherIdObject);
            long generationVoucherId = (long)generationVoucherIdObject;

             Object voucherTypeIdObject;
             context.NewEntity.FieldValues.TryGetValue("VoucherTypeId",out voucherTypeIdObject);
             long voucherTypeId = (long)voucherTypeIdObject;

             Object expiryDateObject;
             context.NewEntity.FieldValues.TryGetValue("ExpiryDate",out expiryDateObject);
             DateTime expiryDate = (DateTime)expiryDateObject;

             Object numberOfCardsObject;
             context.NewEntity.FieldValues.TryGetValue("NumberOfCards",out numberOfCardsObject);
             int numberOfCards = (int)numberOfCardsObject;

             voucherCardsManager.GenerateVoucherCards(generationVoucherId,expiryDate, voucherTypeId, numberOfCards);
        }
    }
}
