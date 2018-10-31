using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.PageDefinition
{
    public class DatetimeFieldType : FieldType
    {

      
        public override Guid ConfigId
        {
            get { return new Guid("16D59257-81C8-41B6-A743-524319AA6663"); }
        }

        public override string RunTimeEditor
        {
            get { return ("demo-module-page-run-time-datetime-field"); }
        }
        public override string RunTimeFilter
        {
            get { return ("demo-module-page-run-time-datetime-filter"); }
        }
        public override bool IsMatch(object field, object filter)
        {
            if (field.Equals(filter)) return true;

            else return false;
        }
    }
}
