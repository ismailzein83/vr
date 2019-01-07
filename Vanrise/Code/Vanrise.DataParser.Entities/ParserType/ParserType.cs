using System;

namespace Vanrise.DataParser.Entities
{
    public class ParserType
    {
        public Guid ParserTypeId { get; set; }

        public string Name { get; set; }

        public ParserTypeSettings Settings { get; set; }
    }   
}