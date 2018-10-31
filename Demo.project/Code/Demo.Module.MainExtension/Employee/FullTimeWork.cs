using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Employee
{
    public class FullTimeWork : Work
    {

        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public int TotalSalary { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("{444D7077-1733-4184-8CBD-C93475035519}"); }
        }

        public override string GetWorkDescription()
        {
            return string.Format("Work Type : Full Time,TotalSalary : {0}{1}  ", TotalSalary,CurrencyName);

        }
    }
}
