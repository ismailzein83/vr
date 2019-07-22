using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class VRWorkflowExpression
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetCode(IVRWorkflowExpressionGetCodeContext context);
    }

    public interface IVRWorkflowExpressionGetCodeContext
    {
    }

    public class VRWorkflowCodeExpression : VRWorkflowExpression
    {
        public override Guid ConfigId => new Guid("46408CC8-8D65-4CAC-B30A-1F625E6DC5ED");

        public string CodeExpression { get; set; }

        public override string GetCode(IVRWorkflowExpressionGetCodeContext context)
        {
            return this.CodeExpression;
        }
    }
    public class VRWorkflowFieldTypeExpression : VRWorkflowExpression
    {
        public override Guid ConfigId => new Guid("1BD963E6-1069-48FC-A64E-79D671E21B89");
        public DataRecordFieldType FieldType { get; set; }
        public Object Value { get; set; }
        public override string GetCode(IVRWorkflowExpressionGetCodeContext context)
        {
            return null;
        }
    }
    public class VRWorkflowExpressionJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            Newtonsoft.Json.Linq.JToken token = Newtonsoft.Json.Linq.JToken.Load(reader);

            if (token.Type == Newtonsoft.Json.Linq.JTokenType.String)
                return new VRWorkflowCodeExpression { CodeExpression = token.ToObject<string>() };
            else
            {
                return Vanrise.Common.Serializer.Deserialize(token.ToString());
            }
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
