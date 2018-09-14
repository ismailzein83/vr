//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Vanrise.GenericData.Entities
//{
//    public class CompositeRecordConditionDefinition
//    {
//        public string Name { get; set; }

//        public CompositeRecordConditionSettings Settings { get; set; }
//    }

//    public abstract class CompositeRecordConditionSettings
//    {
//        public abstract Guid ConfigId { get; }

//        public abstract void GetFields(ICompositeRecordConditionSettingsGetFieldsContext context);
//    }

//    public interface ICompositeRecordConditionSettingsGetFieldsContext
//    {
//        Dictionary<string, DataRecordField> Fields { set; }
//    }

//    public class CompositeRecordConditionSettingsGetFieldsContext : ICompositeRecordConditionSettingsGetFieldsContext
//    {
//        public Dictionary<string, DataRecordField> Fields { get; set; }
//    }
//}