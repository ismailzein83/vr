using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Employee
{
   public class PartTimeWork:Work{

       public int Hours { get; set; }
       public int CurrencyId { get; set; }
       public string CurrencyName { get; set; }
       public int SalaryPerHour { get; set; }
       public override Guid ConfigId
       {
           get { return new Guid("{C1930022-EB16-48CB-8E38-3EBF138EF8FD}"); }
       }

       public override string GetWorkDescription()
       {
           return string.Format("Work Type : Part Time,Number of Hours : {0},Salary Per Hour : {1}{2}  ", Hours, SalaryPerHour, CurrencyName);

       }
    }
}
