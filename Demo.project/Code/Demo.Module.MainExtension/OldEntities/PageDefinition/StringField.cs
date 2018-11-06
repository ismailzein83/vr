using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.PageDefinition
{
    public class StringFieldType : FieldType
    {

      
        public override Guid ConfigId
        {
            get { return new Guid("73865F6F-025D-4251-AF19-D8E98B849E0A"); }
        }

        public override string RunTimeEditor
        {
            get { return ("demo-module-page-run-time-string-field"); }
        }
        public override string RunTimeFilter
        {
            get { return ("demo-module-page-run-time-string-filter"); }
        }
        public override bool IsMatch(object field, object filter)
        {
            if (((string)field).ToLower().Contains((string)filter)) return true;
            
            else return false;
        }
    }
}

