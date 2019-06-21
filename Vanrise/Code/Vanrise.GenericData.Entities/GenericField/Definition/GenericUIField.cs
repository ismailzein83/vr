using System;
namespace Vanrise.GenericData.Entities
{
    public abstract class GenericUIField
    {
        public string FieldPath { get; set; }
        public string FieldTitle { get; set; }
        public Object FieldViewSettings { get; set; }
		public string TextResourceKey { get; set; }
        public Object DefaultFieldValue { get; set; }
	}

}
