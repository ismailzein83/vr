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

    public class PrepareDefaultServicePreviewInput
    {
        public DefaultServiceToAdd DefaultServiceToAdd { get; set; }
        public DefaultServiceToClose DefaultServiceToClose { get; set; }
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public DateTime MinimumDate { get; set; }
    }

    public class PrepareDefaultServicePreviewOutput
    {
        public DefaultServicePreview DefaultServicePreview { get; set; }
    }

    #endregion

    public class PrepareDefaultServicePreview : BaseAsyncActivity<PrepareDefaultServicePreviewInput, PrepareDefaultServicePreviewOutput>
    {
        #region Input Arguments

        public InArgument<DefaultServiceToAdd> DefaultServiceToAdd { get; set; }
        public InArgument<DefaultServiceToClose> DefaultServiceToClose { get; set; }
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }
        public InArgument<int> OwnerId { get; set; }
        public InArgument<DateTime> MinimumDate { get; set; }

        #endregion

        #region Output Arguments

        public OutArgument<DefaultServicePreview> DefaultServicePreview { get; set; }

        #endregion

        protected override PrepareDefaultServicePreviewInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareDefaultServicePreviewInput()
            {
                DefaultServiceToAdd = this.DefaultServiceToAdd.Get(context),
                DefaultServiceToClose = this.DefaultServiceToClose.Get(context),
                OwnerType = this.OwnerType.Get(context),
                OwnerId = this.OwnerId.Get(context),
                MinimumDate = this.MinimumDate.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.DefaultServicePreview.Get(context) == null)
                this.DefaultServicePreview.Set(context, new DefaultServicePreview());
            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareDefaultServicePreviewOutput DoWorkWithResult(PrepareDefaultServicePreviewInput inputArgument, AsyncActivityHandle handle)
        {
            DefaultServiceToAdd defaultServiceToAdd = inputArgument.DefaultServiceToAdd;
            DefaultServiceToClose defaultServiceToClose = inputArgument.DefaultServiceToClose;

            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            int ownerId = inputArgument.OwnerId;
            DateTime minimumDate = inputArgument.MinimumDate;

            DefaultServicePreview preview = new DefaultServicePreview();

            SaleEntityService currentService;
            var serviceLocator = new SaleEntityServiceLocator(new SaleEntityServiceReadWithCache(minimumDate));
            SaleEntityServiceSource targetSource;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                currentService = serviceLocator.GetSellingProductDefaultService(ownerId);
                targetSource = SaleEntityServiceSource.ProductDefault;
            }
            else
            {
                var customerSellingProductManager = new CustomerSellingProductManager();
                int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(ownerId, minimumDate, false);
                if (!sellingProductId.HasValue)
                    throw new NullReferenceException("sellingProductId");

                currentService = serviceLocator.GetCustomerDefaultService(ownerId, sellingProductId.Value);
                targetSource = SaleEntityServiceSource.CustomerDefault;
            }

            if (currentService != null)
            {
                preview.CurrentServices = currentService.Services;
                preview.IsCurrentServiceInherited = currentService.Source != targetSource;
            }

            if (defaultServiceToAdd != null)
            {
                preview.NewServices = defaultServiceToAdd.NewDefaultService.Services;
                preview.EffectiveOn = defaultServiceToAdd.BED;
                preview.EffectiveUntil = defaultServiceToAdd.EED;
            }
            else if (defaultServiceToClose != null)
            {
                preview.EffectiveOn = defaultServiceToClose.CloseEffectiveDate;
            }
            else
            {
                preview = null;
            }

            return new PrepareDefaultServicePreviewOutput()
            {
                DefaultServicePreview = preview
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareDefaultServicePreviewOutput result)
        {
            this.DefaultServicePreview.Set(context, result.DefaultServicePreview);
        }
    }
}
