using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Business;
using Vanrise.ExcelConversion.Entities;

namespace Vanrise.ExcelConversion.MainExtensions.FieldMappings
{
    public class ConcatenateFieldMapping : FieldMapping
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("17abfa66-d659-4e5c-b1c9-f7b408e8de00"); } }
        public List<ConcatenatedPart> Parts { get; set; }

        public override object GetFieldValue(IGetFieldValueContext context)
        {
            if (this.Parts == null)
                throw new NullReferenceException("Parts");
            StringBuilder builder = new StringBuilder();
            GetConcatenatedPartTextContext getPartTextContext = new GetConcatenatedPartTextContext
            {
                Workbook = context.Workbook,
                Sheet = context.Sheet,
                Row = context.Row
            };

            foreach (var part in this.Parts)
            {
                builder.Append(part.GetPartText(getPartTextContext));
            }
            return builder.ToString();
        }
    }
}
