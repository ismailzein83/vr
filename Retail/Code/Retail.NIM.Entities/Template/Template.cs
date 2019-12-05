using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{

    public enum TemplateType { Node = 1, Part = 2 }

    public class Template
    {
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public Guid TypeId { get; set; }
        public TemplateType TemplateType { get; set; }
       // public TemplateSettings Settings { get; set; }
    }
    //public class TemplateSettings
    //{
    //    public Dictionary<string,Object> TemplateData { get; set; }
    //}

    public enum TemplateItemType { Part = 1, Port = 2 }

    public class TemplateItem
    {
        public int TemplateItemId { get; set; }
        public int TemplateId { get; set; }
        public TemplateItemType ItemType { get; set; }
        public int? ParentItemId { get; set; }
        public Guid TypeId { get; set; }
        public TemplateItemSettings Settings { get; set; }

    }
    public class TemplateItemSettings
    {
        public Dictionary<string, Object> ItemData { get; set; }
    }
}
