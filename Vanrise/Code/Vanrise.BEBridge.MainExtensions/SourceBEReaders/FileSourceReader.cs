using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

            if (System.IO.Directory.Exists(Setting.Directory))
            {
                try
                {
                    DirectoryInfo d = new DirectoryInfo(Setting.Directory);
                    FileInfo[] Files = d.GetFiles("*" + Setting.Extension);
                    foreach (FileInfo file in Files)
                    {
                        if (regEx.IsMatch(file.Name))
                        {
                            GetFileContent(context.OnSourceBEBatchRetrieved, file);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new DirectoryNotFoundException(string.Format("directory {0}", Setting.Directory));
            }
        }

        void GetFileContent(Action<FileSourceBatch, SourceBEBatchRetrievedContext> fileBatchAction, FileInfo fileInfo)
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
            FileSourceBatch batch = new FileSourceBatch
            {
                FileName = fileInfo.Name,
                Content = content
            };
            fileBatchAction(batch, null);
        }
    }

    public class FileSourceReaderSetting
    {
        public string Extension { get; set; }
        public string Mask { get; set; }
        public string Directory { get; set; }
    }
}
