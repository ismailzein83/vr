using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordFieldChoice
    {
        public Guid DataRecordFieldChoiceId { get; set; }
        public string Name { get; set; }
        public DataRecordFieldChoiceSettings Settings { get; set; }
    }
    public class DataRecordFieldChoiceSettings
    {
        public List<Choice> Choices { get; set; }
    }
    public class Choice
    {
        public int Value { get; set; }

        public string Text { get; set; }
    }
}
