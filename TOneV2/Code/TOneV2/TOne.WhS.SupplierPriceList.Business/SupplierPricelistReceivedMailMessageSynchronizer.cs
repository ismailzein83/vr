using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.BP.Arguments;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierPricelistReceivedMailMessageSynchronizer : TargetBESynchronizer
    {
        public override string Name
        {
            get
            {
                return "Supplier Pricelist Received Mail Message Synchronizer";
            }
        }

        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            foreach (var targetBE in context.TargetBE)
            {
                /*var objectsTargetBE =targetBE as VRObjectsTargetBE;
                objectsTargetBE.ThrowIfNull("objectsTargetBE is null");
                foreach(var targetObject in objectsTargetBE.TargetObjects.Values)
                {
                    VRReceivedMailMessage receivedMailMessage = targetObject as VRReceivedMailMessage;
                    receivedMailMessage.ThrowIfNull("receivedMailMessage is null");
                    receivedMailMessage.Attachments.ThrowIfNull("receivedMailMessage.Attachments is null");
                    VRFile file = receivedMailMessage.Attachments.First();
                    if (file == null)
                        throw new VRBusinessException("receivedMailMessage.Attachments is empty");
                    var carrierAccountManager = new CarrierAccountManager();
                    Dictionary<string, CarrierAccount> supplierAccounts = carrierAccountManager.GetCachedSupplierAccountsByAutoImportEmail();
                    var supplierAccount = supplierAccounts.GetRecord(receivedMailMessage.Header.From);
                    var supplierPriceListTemplateManager = new SupplierPriceListTemplateManager();
                    var templateId = supplierPriceListTemplateManager.GetSupplierPriceListTemplateBySupplierId(supplierAccount.CarrierAccountId).SupplierPriceListTemplateId;
                    SupplierPriceListType pricelistType;
                    if (receivedMailMessage.Header.Subject.ToLower().Contains("full"))
                        pricelistType = SupplierPriceListType.Full;
                    else if (receivedMailMessage.Header.Subject.ToLower().Contains("country"))
                        pricelistType = SupplierPriceListType.Country;
                    else pricelistType = SupplierPriceListType.RateChange;

                    var supplierPriceListProcessInput = new SupplierPriceListProcessInput
                    {
                        SupplierAccountId = supplierAccount.CarrierAccountId,
                        FileId = file.FileId,
                        SupplierPriceListTemplateId = templateId,
                        CurrencyId = carrierAccountManager.GetCarrierAccountCurrencyId(supplierAccount.CarrierAccountId),
                        PriceListDate = DateTime.Today,
                        SupplierPriceListType = pricelistType,
                    };
                }*/
            }
        }

        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            return true;
        }

        public override void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context)
        {

        }
    }
}
