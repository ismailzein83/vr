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
        public long FileId { get; set; }

        public CDRFileReader FileReader { get; set; }

        public override void ReadCDRs(IReadCDRsFromSourceContext context)
        {
            if (this.FileReader == null)
                throw new NullReferenceException("FileReader");
            Action<IEnumerable<CDR>> onCDRsReceived = (cdrs) => context.OnCDRsReceived(cdrs);
            VRFileManager fileManager = new VRFileManager("CDRComparison_FileCDRSource");

            var file = fileManager.GetFile(this.FileId);
            if (file == null)
                throw new Exception(String.Format("FileId {0}", this.FileId));
            using (var readFromFileContext = new ReadCDRsFromFileContext(file, onCDRsReceived))
            {
                this.FileReader.ReadCDRs(readFromFileContext);
            }
        }

        public override CDRSample ReadSample(IReadSampleFromSourceContext context)
        {
            VRFileManager fileManager = new VRFileManager("CDRComparison_FileCDRSource");

            var file = fileManager.GetFile(this.FileId);
            if (file == null)
                throw new Exception(String.Format("File '{0}' was not found", this.FileId));

            CDRSample cdrSample;
            using (var readSampleFromFileContext = new ReadSampleFromFileContext(file))
            {
                cdrSample = this.FileReader.ReadSample(readSampleFromFileContext);
            }
            return cdrSample;
        }
    }
}
