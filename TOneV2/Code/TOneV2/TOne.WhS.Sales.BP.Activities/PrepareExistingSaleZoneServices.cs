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
    public class PrepareExistingSaleZoneServices : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<SaleEntityZoneService>> ExistingSaleEntityZoneServices { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingSaleZoneService>> ExistingSaleZoneServices { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleEntityZoneService> existingSaleEntities = ExistingSaleEntityZoneServices.Get(context);
            var existingEntities = new List<ExistingSaleZoneService>();

            if (existingSaleEntities != null)
            {
                foreach (SaleEntityZoneService existingSaleEntity in existingSaleEntities)
                {
                    existingEntities.Add(new ExistingSaleZoneService()
                    {
                        SaleEntityZoneServiceEntity = existingSaleEntity
                    });
                }
            }

            ExistingSaleZoneServices.Set(context, existingEntities);
        }
    }
}
