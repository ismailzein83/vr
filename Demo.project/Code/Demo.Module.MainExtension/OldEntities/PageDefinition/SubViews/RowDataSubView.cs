using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.PageDefinition.SubViews
{
    public class RowDataSubView : PageDefinitionSubView
    {

      
        public override Guid ConfigId
        {
            get { return new Guid("9E021F00-73F7-455C-A47F-988ED3390190"); }
        }

        public override string RunTimeEditor
        {
            get { return ("demo-module-page-run-time-row-data"); }
        }
       
    }
}
