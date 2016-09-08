using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    #region Classes

    public class PrepareSaleZoneServicePreviewsInput
    {
        public IEnumerable<SaleZoneServiceToAdd> SaleZoneServicesToAdd { get; set; }

        public IEnumerable<SaleZoneServiceToClose> SaleZoneServicesToClose { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public DateTime MinimumDate { get; set; }
    }

    public class PrepareSaleZoneServicePreviewsOutput
    {
        public IEnumerable<SaleZoneServicePreview> SaleZoneServicePreviews { get; set; }
    }
    
    #endregion

    public class PrepareSaleZoneServicePreviews : BaseAsyncActivity<PrepareSaleZoneServicePreviewsInput, PrepareSaleZoneServicePreviewsOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneServiceToAdd>> SaleZoneServicesToAdd { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneServiceToClose>> SaleZoneServicesToClose { get; set; }

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleZoneServicePreview>> SaleZoneServicePreviews { get; set; }

        #endregion

        protected override PrepareSaleZoneServicePreviewsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareSaleZoneServicePreviewsInput()
            {
                SaleZoneServicesToAdd = this.SaleZoneServicesToAdd.Get(context),
                SaleZoneServicesToClose = this.SaleZoneServicesToClose.Get(context),
                OwnerType = this.OwnerType.Get(context),
                OwnerId = this.OwnerId.Get(context),
                MinimumDate = this.MinimumDate.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.SaleZoneServicePreviews.Get(context) == null)
                this.SaleZoneServicePreviews.Set(context, new List<SaleZoneServicePreview>());
            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareSaleZoneServicePreviewsOutput DoWorkWithResult(PrepareSaleZoneServicePreviewsInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<SaleZoneServiceToAdd> saleZoneServicesToAdd = inputArgument.SaleZoneServicesToAdd;
            IEnumerable<SaleZoneServiceToClose> saleZoneServicesToClose = inputArgument.SaleZoneServicesToClose;

            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            int ownerId = inputArgument.OwnerId;
            DateTime minimumDate = inputArgument.MinimumDate;

            var saleZoneServicePreviews = new List<SaleZoneServicePreview>();
            var serviceLocator = new SaleEntityServiceLocator(new SaleEntityServiceReadWithCache(minimumDate));

            SaleEntityService currentService;
            SaleEntityServiceSource targetSource;
            SaleZoneServicePreview preview;

            foreach (SaleZoneServiceToAdd saleZoneServiceToAdd in saleZoneServicesToAdd)
            {
                preview = new SaleZoneServicePreview()
                {
                    ZoneName = saleZoneServiceToAdd.ZoneName,
                    NewServices = saleZoneServiceToAdd.Services,
                    EffectiveOn = saleZoneServiceToAdd.BED,
                    EffectiveUntil = saleZoneServiceToAdd.EED
                };

                if (ownerType == SalePriceListOwnerType.SellingProduct)
                {
                    currentService = serviceLocator.GetSellingProductZoneService(ownerId, saleZoneServiceToAdd.ZoneId);
                    targetSource = SaleEntityServiceSource.ProductZone;
                }
                else
                {
                    int sellingProductId = GetSellingProductId(ownerId, saleZoneServiceToAdd.BED, false);
                    currentService = serviceLocator.GetCustomerZoneService(ownerId, sellingProductId, saleZoneServiceToAdd.ZoneId);
                    targetSource = SaleEntityServiceSource.CustomerZone;
                }

                if (currentService != null)
                {
                    preview.CurrentServices = currentService.Services;
                    preview.IsCurrentServiceInherited = currentService.Source != targetSource;
                }

                saleZoneServicePreviews.Add(preview);
            }

            if (saleZoneServicesToClose != null)
            {
                foreach (SaleZoneServiceToClose saleZoneServiceToClose in saleZoneServicesToClose)
                {
                    preview = new SaleZoneServicePreview()
                    {
                        ZoneName = saleZoneServiceToClose.ZoneName,
                        EffectiveOn = saleZoneServiceToClose.CloseEffectiveDate
                    };

                    if (ownerType == SalePriceListOwnerType.SellingProduct)
                    {
                        currentService = serviceLocator.GetSellingProductZoneService(ownerId, saleZoneServiceToClose.ZoneId);
                        targetSource = SaleEntityServiceSource.ProductZone;
                    }
                    else
                    {
                        int sellingProductId = GetSellingProductId(ownerId, saleZoneServiceToClose.CloseEffectiveDate, false);
                        currentService = serviceLocator.GetCustomerZoneService(ownerId, sellingProductId, saleZoneServiceToClose.ZoneId);
                        targetSource = SaleEntityServiceSource.CustomerZone;
                    }

                    if (currentService != null)
                    {
                        preview.CurrentServices = currentService.Services;
                        preview.IsCurrentServiceInherited = currentService.Source != targetSource;
                    }

                    saleZoneServicePreviews.Add(preview);
                }
            }

            return new PrepareSaleZoneServicePreviewsOutput()
            {
                SaleZoneServicePreviews = saleZoneServicePreviews
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareSaleZoneServicePreviewsOutput result)
        {
            this.SaleZoneServicePreviews.Set(context, result.SaleZoneServicePreviews);
        }

        #region Private Methods

        private int GetSellingProductId(int customerId, DateTime effectiveOn, bool isEffectiveInFuture)
        {
            var customerSellingProductManager = new CustomerSellingProductManager();
            int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customerId, effectiveOn, isEffectiveInFuture);
            if (!sellingProductId.HasValue)
                throw new NullReferenceException("sellingProductId");
            return sellingProductId.Value;
        }

        #endregion
    }
}
