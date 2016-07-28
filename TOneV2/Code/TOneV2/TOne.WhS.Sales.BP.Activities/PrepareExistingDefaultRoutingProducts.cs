using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class PrepareExistingDefaultRoutingProducts : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<DefaultRoutingProduct>> ExistingSaleEntityDefaultRoutingProducts { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingDefaultRoutingProduct>> ExistingDefaultRoutingProducts { get; set; }
        
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<DefaultRoutingProduct> existingSaleEntities = ExistingSaleEntityDefaultRoutingProducts.Get(context);
            var existingEntities = new List<ExistingDefaultRoutingProduct>();

            if (existingSaleEntities != null)
            {
                foreach (DefaultRoutingProduct existingSaleEntity in existingSaleEntities)
                {
                    existingEntities.Add(new ExistingDefaultRoutingProduct()
                    {
                        DefaultRoutingProductEntity = existingSaleEntity
                    });
                }    
            }

            ExistingDefaultRoutingProducts.Set(context, existingEntities);
        }
    }
}
