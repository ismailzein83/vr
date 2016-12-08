using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        public override IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context)
        {
            var invoice = context.InvoiceActionContext.GetInvoice;
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
            var invoiceItems = context.InvoiceActionContext.GetInvoiceItems(new List<String> { itemGrouping.ItemSetName});// 
            InvoiceItemGroupingManager invoiceItemGroupingManager = new Business.InvoiceItemGroupingManager();
           
            GroupingInvoiceItemQuery query = new GroupingInvoiceItemQuery
            {
                DimensionIds  = this.Dimensions.Select(x=>x.DimensionId).ToList(),
                MeasureIds =  this.Measures.Select(x=>x.MeasureId).ToList(),
                InvoiceTypeId = invoice.InvoiceTypeId,
                ItemGroupingId =  this.ItemGroupingId
            };

           var groupedItems =  invoiceItemGroupingManager.ApplyFinalGroupingAndFiltering(new GroupingInvoiceItemQueryContext(query), invoiceItems, query.DimensionIds, query.MeasureIds, null, itemGrouping);


            var type = Type.GetType(this.GroupingClassFQTN);
         
            List<dynamic> invoiceItemsDetails = new List<dynamic>();
            foreach (var item in groupedItems)
            {
                var classInstanse = Activator.CreateInstance(type);
                for (int i = 0; i < this.Dimensions.Count; i++)
                {
                    var dimension = this.Dimensions[i];
                    var dimensionObj = itemGrouping.DimensionItemFields.FirstOrDefault(x => x.DimensionItemFieldId == dimension.DimensionId);
                    var dimesionObjValue = item.DimensionValues[i].Value;
                    SetObjectProperty(classInstanse, dimensionObj.FieldName, dimesionObjValue);
                }
                foreach (var measure in this.Measures)
                {
                    var measureObj = itemGrouping.AggregateItemFields.FirstOrDefault(x => x.AggregateItemFieldId == measure.MeasureId);
                    var measureObjValue = item.MeasureValues[measureObj.FieldName].Value;
                    SetObjectProperty(classInstanse, measureObj.FieldName, measureObjValue);
                }
                invoiceItemsDetails.Add(classInstanse);
            }
          
            return invoiceItemsDetails;

        }
        void SetObjectProperty(object theObject, string propertyName, object value)
        {
            Type type = theObject.GetType();
            var property = type.GetProperty(propertyName);
            property.SetValue(theObject,value);
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
