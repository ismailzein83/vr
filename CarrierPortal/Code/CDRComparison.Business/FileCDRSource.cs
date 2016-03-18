using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace CDRComparison.Business
{
    public class FileCDRSource : CDRSource
    {
        public List<long> FileIds { get; set; }

        public CDRFileReader FileReader { get; set; }

        public override void ReadCDRs(IReadCDRsFromSourceContext context)
        {
            if (this.FileIds == null)
                throw new NullReferenceException("FileIds");
            if (this.FileReader == null)
                throw new NullReferenceException("FileReader");
            Action<IEnumerable<CDR>> onCDRsReceived = (cdrs) => context.OnCDRsReceived(cdrs);
            VRFileManager fileManager = new VRFileManager();
            foreach(var fileId in this.FileIds)
            {
                var file = fileManager.GetFile(fileId);
                if (file == null)
                    throw new Exception(String.Format("file {0}", fileId));
                using (var readFromFileContext = new ReadCDRsFromFileContext(file, onCDRsReceived))
                {
                    this.FileReader.ReadCDRs(readFromFileContext);
                }
            }
        }
    }
}
