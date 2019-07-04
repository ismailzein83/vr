using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericEditorDefinitionSetting
{
    public class NumberRangeGenericEditorDefinition : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId => new Guid("6568F466-C2FF-4FF4-AA1D-3862BEB2EBC5");
        public override string RuntimeEditor { get { return "vr-genericdata-numberrangeeditor-runtime"; } }
        public string RangeVariableName { get; set; }
    }

    public class NumberRangeGenericEditorDefinitionSettings : RangeGenericEditorDefinitionSettings
    {
        public long FromNumber { get; set; }
        public long ToNumber { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public override List<object> GetRangeValues()
        {
            if (FromNumber > ToNumber)
                return null;
            if (FromNumber == ToNumber)
            {
                return new List<object> { GetEvaluatedNumber(FromNumber.ToString())  };
            }

            List<Object> numbers = new List<object>();
            for (var i = FromNumber; i <= ToNumber; i++)
            {
                numbers.Add(GetEvaluatedNumber(i.ToString()));
            }
            return numbers;
        }

        private string GetEvaluatedNumber(string number)
        {

            if (Prefix != null)
                number = $"{Prefix}{number}";

            if (Suffix != null)
                number = $"{number}{Suffix}";

            return number;
        }
    }
}
