using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class PageDefinition
    {
        public int PageDefinitionId { get; set; }
       
        public string Name { get; set; }

        public Details Details { get; set; }
    }

    
    public class Details
    {
       public List<Field> Fields { get; set; }
       public List<SubView> SubViews { get; set; }
       public List<Filter> Filters { get; set; } 
    }
  
    public class Field
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public FieldType FieldType { get; set; }
        public bool IsRequired { get; set; }
    }
    public class SubView
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public PageDefinitionSubView PageDefinitionSubViewSettings { get; set; }

    }
    public class Filter
    {
        public string fieldName { get; set; }

    }

    public abstract class FieldType
    {
        public abstract Guid ConfigId { get; }
        public abstract string RunTimeEditor { get; }
        public abstract string RunTimeFilter { get; }
        public abstract bool IsMatch(object field,object filter);
    }

    public abstract class PageDefinitionSubView
    {
        public abstract Guid ConfigId { get; }
        public abstract string RunTimeEditor { get; }

    }

    public abstract class PageDefinitionGenericSubView
    {
        public abstract Guid ConfigId { get; }
        public abstract int PageDefinitionId { get; }

    }
}