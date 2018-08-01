using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class FileUploadResult
    {
        public long FileId { get; set; }

        public string Name { get; set; }

        public Guid? FileUniqueId { get; set; }
    }
}
