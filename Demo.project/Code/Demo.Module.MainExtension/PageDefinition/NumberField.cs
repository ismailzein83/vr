using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.PageDefinition
{
    public class NumberFieldType : FieldType
    {

      
        public override Guid ConfigId
        {
            get { return new Guid("6CE39A5B-342D-4789-9216-DB1C58E73E31"); }
        }

        public override string RunTimeEditor
        {
            get { return ("demo-module-page-run-time-number-field"); }
        }
        public override string RunTimeFilter
        {
            get { return ("demo-module-page-run-time-number-filter"); }
        }
        public override bool IsMatch(object field, object filter)
        {
            if (field.Equals(filter)) return true;

            else return false;
        }
    }
}

