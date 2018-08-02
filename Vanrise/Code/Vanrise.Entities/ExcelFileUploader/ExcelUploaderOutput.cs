using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class ExcelUploaderOutput
    {
        public long FileId { get; set;}
        public bool IsSucceeded{ get; set; }
        public Guid? FileUniqueId { get; set; }
    }
} 
