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
    public class PrepareExistingDefaultServices : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<SaleEntityDefaultService>> ExistingSaleEntityDefaultServices { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingDefaultService>> ExistingDefaultServices { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleEntityDefaultService> existingSaleEntities = ExistingSaleEntityDefaultServices.Get(context);
            var existingEntities = new List<ExistingDefaultService>();

            if (existingSaleEntities != null)
            {
                foreach (SaleEntityDefaultService existingSaleEntity in existingSaleEntities)
                {
                    existingEntities.Add(new ExistingDefaultService()
                    {
                        SaleEntityDefaultServiceEntity = existingSaleEntity
                    });
                }
            }

            ExistingDefaultServices.Set(context, existingEntities);
        }
    }
}
