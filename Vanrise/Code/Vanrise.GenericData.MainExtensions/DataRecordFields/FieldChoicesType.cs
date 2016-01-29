using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldChoicesType : DataRecordFieldType
    {
        public List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        public int Value { get; set; }

        public string Text { get; set; }
    }
}
