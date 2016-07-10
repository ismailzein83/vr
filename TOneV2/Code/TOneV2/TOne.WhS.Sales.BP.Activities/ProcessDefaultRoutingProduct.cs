using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class ProcessDefaultRoutingProduct : CodeActivity
    {
        #region Input Arguments
        
        [RequiredArgument]
        public InArgument<DefaultRoutingProductToAdd> DefaultRoutingProductToAdd { get; set; }

        [RequiredArgument]
        public InArgument<DefaultRoutingProductToClose> DefaultRoutingProductToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingDefaultRoutingProduct>> ExistingDefaultRoutingProducts { get; set; }
        
        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<NewDefaultRoutingProduct> NewDefaultRoutingProduct { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedDefaultRoutingProduct>> ChangedDefaultRoutingProducts { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            DefaultRoutingProductToAdd defaultRoutingProductToAdd = DefaultRoutingProductToAdd.Get(context);
            DefaultRoutingProductToClose defaultRoutingProductToClose = DefaultRoutingProductToClose.Get(context);
            IEnumerable<ExistingDefaultRoutingProduct> existingDefaultRoutingProducts = ExistingDefaultRoutingProducts.Get(context);

            ProcessDefaultRoutingProductContext processDefaultRoutingProductContext = new ProcessDefaultRoutingProductContext()
            {
                DefaultRoutingProductToAdd = defaultRoutingProductToAdd,
                DefaultRoutingProductToClose = defaultRoutingProductToClose,
                ExistingDefaultRoutingProducts = existingDefaultRoutingProducts
            };

            var priceListDefaultRoutingProductManager = new PriceListDefaultRoutingProductManager();
            priceListDefaultRoutingProductManager.ProcessDefaultRoutingProduct(processDefaultRoutingProductContext);

            NewDefaultRoutingProduct.Set(context, processDefaultRoutingProductContext.NewDefaultRoutingProduct);
            ChangedDefaultRoutingProducts.Set(context, processDefaultRoutingProductContext.ChangedDefaultRoutingProducts);
        }
    }
}
