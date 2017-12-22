using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Business
{
    public abstract class DataGroupingHandler
    {
        public abstract string GetItemGroupingKey(IDataGroupingHandlerGetItemGroupingKeyContext context);

        public abstract void UpdateExistingItemFromNew(IDataGroupingHandlerUpdateExistingFromNewContext context);

        public virtual void FinalizeGrouping(IDataGroupingHandlerFinalizeGroupingContext context)
        {
            context.FinalResults = context.GroupedItems.Cast<dynamic>().ToList();
        }

        public virtual string SerializeItems(List<IDataGroupingItem> items)
        {
            return Serializer.Serialize(items);
        }

        public virtual List<IDataGroupingItem> DeserializeItems(string serializedItems)
        {
            if (serializedItems == null)
                throw new ArgumentNullException("serializedItems");
            dynamic deserializedAsObject = Serializer.Deserialize(serializedItems);
            List<IDataGroupingItem> output = new List<IDataGroupingItem>();
            foreach (var item in deserializedAsObject)
            {
                output.Add(item as IDataGroupingItem);
            }
            return output;
        }

        public virtual string SerializeFinalResultItems(List<dynamic> finalResultItems)
        {
            return Serializer.Serialize(finalResultItems);
        }

        public virtual List<dynamic> DeserializeFinalResultItems(string serializedItems)
        {
            if (serializedItems == null)
                throw new ArgumentNullException("serializedItems");
            dynamic deserializedAsObject = Serializer.Deserialize(serializedItems);
            List<dynamic> output = new List<dynamic>();
            foreach (var item in deserializedAsObject)
            {
                output.Add(item as dynamic);
            }
            return output;
        }
    }

    public interface IDataGroupingHandlerGetItemGroupingKeyContext
    {
        IDataGroupingItem Item { get; }
    }

    public class DataGroupingHandlerGetItemGroupingKeyContext : IDataGroupingHandlerGetItemGroupingKeyContext
    {
        public IDataGroupingItem Item { get; set; }
    }

    public interface IDataGroupingHandlerUpdateExistingFromNewContext
    {
        IDataGroupingItem Existing { get; }

        IDataGroupingItem New { get; }
    }

    public interface IDataGroupingHandlerFinalizeGroupingContext
    {
        List<IDataGroupingItem> GroupedItems { get; }

        List<dynamic> FinalResults { set; }
    }

    public class DataGroupingHandlerFinalizeGroupingContext : IDataGroupingHandlerFinalizeGroupingContext
    {

        public List<IDataGroupingItem> GroupedItems
        {
            get;
            set;
        }

        public List<dynamic> FinalResults
        {
            get;
            set;
        }
    }

    public class DataGroupingHandlerUpdateExistingFromNewContext : IDataGroupingHandlerUpdateExistingFromNewContext
    {
        public IDataGroupingItem Existing
        {
            get;
            set;
        }

        public IDataGroupingItem New
        {
            get;
            set;
        }
    }
}
