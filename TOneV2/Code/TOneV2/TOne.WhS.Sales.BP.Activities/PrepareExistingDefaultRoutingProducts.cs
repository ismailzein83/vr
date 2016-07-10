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
        #region Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<DefaultRoutingProduct>> ExistingSaleEntityDefaultRoutingProducts { get; set; }

        [RequiredArgument]
        public OutArgument <IEnumerable<ExistingDefaultRoutingProduct>> ExistingDefaultRoutingProducts { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<DefaultRoutingProduct> existingSaleEntities = ExistingSaleEntityDefaultRoutingProducts.Get(context);

            if (existingSaleEntities == null || existingSaleEntities.Count() == 0)
                return;

            var existingEntities = new List<ExistingDefaultRoutingProduct>();

            foreach (DefaultRoutingProduct existingSaleEntity in existingSaleEntities)
            {
                existingEntities.Add(new ExistingDefaultRoutingProduct()
                {
                    DefaultRoutingProductEntity = existingSaleEntity
                });
            }

            ExistingDefaultRoutingProducts.Set(context, existingEntities);
        }
    }
}
