using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.Context
{
    public class ItemGroupingFilterFieldMatchContext : IRecordFilterGenericFieldMatchContext
    {
        public GroupingInvoiceItemDetail _parentGroupingInvoiceItemDetail { get; set; }
        public Dictionary<string,ItemGroupingFieldInfo> _fieldInfo { get; set; }

        public ItemGroupingFilterFieldMatchContext(GroupingInvoiceItemDetail parentGroupingInvoiceItemDetail, Dictionary<string, ItemGroupingFieldInfo> fieldInfo)
        {
            if (parentGroupingInvoiceItemDetail == null)
                throw new ArgumentNullException("parentGroupingInvoiceItemDetail");
            if (fieldInfo == null)
                throw new NullReferenceException(String.Format("fieldInfo"));
            _fieldInfo = fieldInfo;
            _parentGroupingInvoiceItemDetail = parentGroupingInvoiceItemDetail;
        }
        public object GetFieldValue(string fieldName, out GenericData.Entities.DataRecordFieldType fieldType)
        {
            string dimensionName = fieldName;
            
            ItemGroupingFieldInfo fieldInfo;
            if(!_fieldInfo.TryGetValue(dimensionName,out fieldInfo))
                throw new NullReferenceException(String.Format("_fieldInfo. dimensionName '{0}'", dimensionName));

            fieldType = fieldInfo.FieldType;
            if(fieldInfo.MeasureName != null)
            {
                return _parentGroupingInvoiceItemDetail.MeasureValues[fieldInfo.MeasureName].Value;

            }else
            {
                return _parentGroupingInvoiceItemDetail.DimensionValues[fieldInfo.DimensionIndex].Value;
            }
        }
    }
}
