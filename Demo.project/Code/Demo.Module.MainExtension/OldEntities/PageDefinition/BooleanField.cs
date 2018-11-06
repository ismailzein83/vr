using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.PageDefinition
{
    public class BooleanFilter
    {
        public List<bool> BoolValues { get; set; }
    }
    public class BooleanFieldType : FieldType
    {

        public override Guid ConfigId
        {
            get { return new Guid("DB999318-1D75-4766-ACD4-5BC4CD0A4F0C"); }
        }

        public override string RunTimeEditor
        {
            get { return ("demo-module-page-run-time-boolean-field"); }
        }

        public override string RunTimeFilter
        {
            get { return ("demo-module-page-run-time-boolean-filter"); }
        }




        public class TestFilterItem
        {

        }
        public override bool IsMatch(object field, object filter)
        {
            
                BooleanFilter booleanFilter = new BooleanFilter();
                booleanFilter = (BooleanFilter)filter;

                if (booleanFilter.BoolValues == null) return true;
              
                else
                
                    foreach (var item in booleanFilter.BoolValues)
                    {
                        if ((bool)field == item)
                            return true;
                    } return false;
                

              
        }
    }
}
