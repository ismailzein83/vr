using System;
using System.IO;
using System.Text.RegularExpressions;
using Vanrise.BEBridge.Entities;

namespace Vanrise.BEBridge.MainExtensions.SourceBEReaders
{
    public class FileSourceReader : SourceBEReader
    {

        public FileSourceReaderSetting Setting { get; set; }
        public override void RetrieveUpdatedBEs(ISourceBEReaderRetrieveUpdatedBEsContext context)
        {
            FileSourceBatch fileBatch = new FileSourceBatch();
            string mask = string.IsNullOrEmpty(Setting.Mask) ? "" : Setting.Mask;
            Regex regEx = new Regex(mask);

            if (Directory.Exists(Setting.Directory))
            {
                DirectoryInfo d = new DirectoryInfo(Setting.Directory);
                FileInfo[] Files = d.GetFiles("*" + Setting.Extension);
                foreach (FileInfo file in Files)
                {
                    if (regEx.IsMatch(file.Name))
                    {
                        context.OnSourceBEBatchRetrieved(GetFileSourceBatch(file), null);
                    }
                }
            }
            else
            {
                throw new DirectoryNotFoundException(string.Format("directory {0}", Setting.Directory));
            }
        }
        FileSourceBatch GetFileSourceBatch(FileInfo fileInfo)
        {
            string fileName = string.Format("{0}/{1}", Setting.Directory, fileInfo.Name);
            byte[] content = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                content = new byte[fs.Length];
                fs.Read(content, 0, (int)fs.Length);
            }
            if (content == null)
                throw new NullReferenceException(string.Format("content"));
            return new FileSourceBatch
            {
                FileName = fileInfo.Name,
                Content = content
            };
        }
    }

    public class FileSourceReaderSetting
    {
        public Guid ConfigId
        {
            get
            {
                return new Guid("e2f68462-88d7-41ac-863f-f21a8fd5cc48");
            }
        }
        public string Extension { get; set; }
        public string Mask { get; set; }
        public string Directory { get; set; }
    }
}
