using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers
{
    public class ConnectionIdentificationPackageParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("345EEBD2-630C-4490-98C8-BE7E78600B69"); } }
        public string ConnectionIdentificationFieldName { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            int connectionIdentification =  ParserHelper.ByteToNumber(context.FieldValue);
            context.Record.SetFieldValue(this.ConnectionIdentificationFieldName, connectionIdentification);
        }
    }
}