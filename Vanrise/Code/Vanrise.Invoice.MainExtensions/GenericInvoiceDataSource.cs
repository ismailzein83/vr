using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class GenericInvoiceDataSource : InvoiceDataSourceSettings
    {
        public override Guid ConfigId { get { return new Guid("5DC49B0F-4963-4917-A105-F95506559D32"); } }
        public Guid BusinessEntityDefinitionId { get; set; }
        public List<String> FieldNames { get; set; }
        public RecordFilterGroup FilterGroup { get; set; }

        public override IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context)
        {
            //    GenericBusinessEntityManager genericBeManager = new GenericBusinessEntityManager();
            //    GenericBusinessEntityDefinitionManager genericBeDefinitionManager = new GenericBusinessEntityDefinitionManager();
            //    var genericBEDefinitionSetting = genericBeDefinitionManager.GetGenericBEDefinitionSettings(BusinessEntityDefinitionId, true);
            //    genericBEDefinitionSetting.ThrowIfNull("genericBEDefinitionSetting", BusinessEntityDefinitionId);

            //    var entities = genericBeManager.GetAllGenericBusinessEntities(BusinessEntityDefinitionId, FieldNames, FilterGroup);

            //    if (entities == null)
            //        return null;

            //    List<dynamic> dataSourceItems = new List<dynamic>();
            //    foreach (var entity in entities)
            //    {
            //        DataRecordObject item = new DataRecordObject(genericBEDefinitionSetting.DataRecordTypeId, entity.FieldValues);
            //        dataSourceItems.Add(item.Object);
            //    }

            //    return dataSourceItems;
            return null;
        }
    }
}
