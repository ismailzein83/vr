using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class RAPeriodDefinitionOnAfterSaveHandler : GenericBEOnAfterSaveHandler
    {
        public override Guid ConfigId { get { return new Guid("db7ea63c-0407-4712-a95c-7fc253db8fdf"); } }
        public override void Execute(IGenericBEOnAfterSaveHandlerContext context)
        {
            //            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            //            context.ThrowIfNull("context");
            //            context.NewEntity.ThrowIfNull("context.NewEntity");
            //            context.NewEntity.FieldValues.ThrowIfNull("context.NewEntity.FieldValues");
            //            if (context.BusinessEntityDefinitionId == null)
            //                throw new NullReferenceException("BusinessEntityDefinitionId");
            //            var from = (DateTime)context.NewEntity.FieldValues.GetRecord("From");
            //            if (from == null)
            //                throw new NullReferenceException("from");
            //            var to = (DateTime)context.NewEntity.FieldValues.GetRecord("To");
            //            if (to == null)
            //                throw new NullReferenceException("to");
            //            var repeat = context.NewEntity.FieldValues.GetRecord("Repeat");
            //            if(repeat != null)
            //            {
            //                for (var i = 0; i < (int)repeat; i++)
            //                {
            //                    var genericBusinessEntityToAdd = new GenericBusinessEntityToAdd
            //                    {
            //                        BusinessEntityDefinitionId = context.BusinessEntityDefinitionId,
            //                        FieldValues = new Dictionary<string, object>()
            //                    };

            //                    genericBusinessEntityToAdd.FieldValues.Add("From", from.AddMonths(1));
            //                    genericBusinessEntityToAdd.FieldValues.Add("URL", applicationURL);
            //                    var insertOperationOutput = genericBusinessEntityManager.AddGenericBusinessEntity(genericBusinessEntityToAdd);
            //                }
            //}
            //            }

        }        
    }
}
