using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class College
    {
        public int CollegeId { get; set; }
        
        public string Name { get; set; }
        
        public int UniversityId { get; set; }

        public CollegeInfoType CollegeInfo { get; set; }

        public List<Description> DescriptionString { get; set; }
      
    }
}