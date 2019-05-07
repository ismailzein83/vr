using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class ItemGroupingDataSourceSettings : InvoiceDataSourceSettings
    {
        public override Guid ConfigId { get { return new Guid("F41D670E-F079-4D38-A2B5-9B6394299EFA"); } }
        public Guid ItemGroupingId { get; set; }
        public List<DataSourceDimension> Dimensions { get; set; }
        public List<DataSourceMeasure> Measures { get; set; }
        public string GroupingClassFQTN { get; set; }
        public RecordFilterGroup RecordFilter { get; set; }
        public override IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context)
        {
            var invoice = context.InvoiceActionContext.GetInvoice();
            if (invoice == null)
                throw new NullReferenceException("invoice");
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoice.InvoiceTypeId);
            if(invoiceType == null)
                throw new NullReferenceException(string.Format("invoiceType {0}", invoice.InvoiceTypeId));
            if (invoiceType.Settings == null)
                throw new NullReferenceException(string.Format("invoiceType.Settings {0}", invoice.InvoiceTypeId));
            if (invoiceType.Settings.ItemGroupings == null)
                throw new NullReferenceException(string.Format("invoiceType.Settings.ItemGroupings {0}", invoice.InvoiceTypeId));

            var itemGrouping = invoiceType.Settings.ItemGroupings.FirstOrDefault(x => x.ItemGroupingId == this.ItemGroupingId);


            InvoiceItemManager invoiceItemManager = new InvoiceItemManager();
            var invoiceItems = context.InvoiceActionContext.GetInvoiceItems(new List<String> { itemGrouping.ItemSetName},CompareOperator.Equal);// 
            InvoiceItemGroupingManager invoiceItemGroupingManager = new Business.InvoiceItemGroupingManager();
           
            GroupingInvoiceItemQuery query = new GroupingInvoiceItemQuery
            {
                DimensionIds  = this.Dimensions.Select(x=>x.DimensionId).ToList(),
                MeasureIds =  this.Measures.Select(x=>x.MeasureId).ToList(),
                InvoiceTypeId = invoice.InvoiceTypeId,
                ItemGroupingId =  this.ItemGroupingId
            };

           var groupedItems =  invoiceItemGroupingManager.ApplyFinalGroupingAndFiltering(new GroupingInvoiceItemQueryContext(query), invoiceItems, query.DimensionIds, query.MeasureIds, null, itemGrouping);


         //   RecordFilterManager recordFilterManager = new RecordFilterManager();
            var type = Type.GetType(this.GroupingClassFQTN);
         
            List<dynamic> invoiceItemsDetails = new List<dynamic>();
            List<PropertyInfo> propertyInfo = new List<PropertyInfo>();

         //   Dictionary<string, DataRecordField> recordTypeFieldsByName = new Dictionary<string, DataRecordField>();
            for (int i = 0; i < this.Dimensions.Count; i++)
            {
                var dimension = this.Dimensions[i];
                var dimensionObj = itemGrouping.DimensionItemFields.FirstOrDefault(x => x.DimensionItemFieldId == dimension.DimensionId);
                //recordTypeFieldsByName.Add(dimensionObj.FieldName, new DataRecordField
                //{
                //    Name = dimensionObj.FieldName,
                //    Title = dimensionObj.FieldDescription,
                //    Type = dimensionObj.FieldType
                //});

                propertyInfo.Add(type.GetProperty(dimensionObj.FieldName));
                propertyInfo.Add(type.GetProperty(string.Format("{0}Description",dimensionObj.FieldName)));
            }
            foreach (var measure in this.Measures)
            {
                var measureObj = itemGrouping.AggregateItemFields.FirstOrDefault(x => x.AggregateItemFieldId == measure.MeasureId);
                //recordTypeFieldsByName.Add(measureObj.FieldName, new DataRecordField
                //{
                //    Name = measureObj.FieldName,
                //    Title = measureObj.FieldDescription,
                //    Type = measureObj.FieldType
                //});
                propertyInfo.Add(type.GetProperty(measureObj.FieldName));
            }

            foreach (var item in groupedItems)
            {
              //  Dictionary<string, dynamic> dataRecord = new Dictionary<string, dynamic>();

                var classInstanse = Activator.CreateInstance(type);
                int counter = 0;
                for (int i = 0; i < this.Dimensions.Count; i++)
                {
                    var dimension = this.Dimensions[i];
                    var dimensionObj = itemGrouping.DimensionItemFields.FirstOrDefault(x => x.DimensionItemFieldId == dimension.DimensionId);
                    var dimesionObjValue = item.DimensionValues[i].Value;

                   // dataRecord.Add(dimensionObj.FieldName, dimesionObjValue);

                    var dimesionObjDescription = item.DimensionValues[i].Name;
                    SetObjectProperty(classInstanse, propertyInfo[counter], dimesionObjValue);
                    counter++;
                    SetObjectProperty(classInstanse, propertyInfo[counter], dimesionObjDescription);
                    counter++;
                }
                foreach (var measure in this.Measures)
                {

                    var measureObj = itemGrouping.AggregateItemFields.FirstOrDefault(x => x.AggregateItemFieldId == measure.MeasureId);
                    var measureObjValue = item.MeasureValues[measureObj.FieldName].Value;

                   // dataRecord.Add(measureObj.FieldName, measureObjValue);

                    SetObjectProperty(classInstanse, propertyInfo[counter], measureObjValue);
                    counter++;
                }
             //   if (RecordFilter == null || recordFilterManager.IsFilterGroupMatch(RecordFilter, new DataRecordDictFilterGenericFieldMatchContext(dataRecord, recordTypeFieldsByName)))
                    invoiceItemsDetails.Add(classInstanse);
            }
          
            return invoiceItemsDetails;

        }
        void SetObjectProperty(object theObject, PropertyInfo propertyinfo, object value)
        {
            propertyinfo.SetValue(theObject, value);
        }
        public class DataSourceDimension
        {
            public Guid DimensionId { get; set; }
        }
        public class DataSourceMeasure
        {
            public Guid MeasureId { get; set; }
        }
    }
}
