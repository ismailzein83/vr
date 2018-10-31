using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Student
    {
        public int StudentId { get; set; }
       
        public string Name { get; set; }

        public int SchoolId { get; set; }


        public int Age { get; set; }
        public StudentSettings Settings { get; set; }

        public int DemoCountryId { get; set; }

        public int DemoCityId { get; set; }

    }
   public class StudentSettings
   {
       public PaymentMethod PaymentMethod { get; set; }
   }
   public abstract class PaymentMethod
   {
       public abstract Guid ConfigId { get; }

       public abstract string GetPaymentMethodDescription();
   }


  


}