using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.ExcelConversion.Entities;

namespace Vanrise.ExcelConversion.MainExtensions
{
    public class ConstantFieldMapping : FieldMapping
    {
        public override Guid ConfigId { get { return new Guid("F0EEEAA9-4FA6-4917-B860-CA285B4761BE"); } }

        public string FieldValue;

        public override object GetFieldValue(IGetFieldValueContext context)
        {
            return FieldValue;
        }
    }
}
