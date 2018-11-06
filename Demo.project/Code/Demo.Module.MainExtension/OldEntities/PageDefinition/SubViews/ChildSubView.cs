using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.PageDefinition.SubViews
{
    public class ChildSubView : PageDefinitionSubView
    {

      
        public override Guid ConfigId
        {
            get { return new Guid("2470242d-f355-4ab6-970f-c5934c23d3cd"); }
        }

        public int PageDefinitionId { get; set; }
        public string FieldName { get; set; }
        public override string RunTimeEditor
        {
            get { return ("demo-module-page-run-time-child-page-definition"); }
        }
       
    }
}
