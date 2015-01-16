using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TABS.Addons.CDRStores
{
    [NamedAddon("Folder CDR Store", "Stores cdrs in a folder as files of csv")]
    public class FolderCDRStore : Extensibility.ICDRStore
    {
        #region ICDRStore Members
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(FolderCDRStore));

        public string Name { get { return typeof(FolderCDRStore).FullName; } }
        public string Description { get; set; }

        public string HelpHtml
        {
            get
            {
                return
                    @"Stores cdrs in a folder as files of T.One separated values (each CDR is a line).<br/> 
                The configuration string should be the full path for the destination folder.<br/> 
                The Configuration Options of format <b>OPTION=value</b> are (each on a line):<br/> 
                    - FORMAT: the file name format, example <b>FORMAT=CDR.{0:yyyyMMddHH}.S{1:00}.tsv</b> (0 for attempt Datetime, 1 for switch ID)<br/>
                    - ZIP: the file contents are zipped TRUE / FALSE. If TRUE the file name will be appended with a .gz extension.<br/>
                ";
            }
        }

        public string ConfigString { get; set; }
        public string ConfigOptions { get; set; }
        public bool IsEnabled { get; set; }

        string[] options { get { return ConfigOptions.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries); } }

        string Option(string optionName, string defaultValue)
        {
            string header = optionName + "=";
            string optionLine = options.Where(o => o.Trim().StartsWith(header, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            return optionLine != null ? optionLine.Substring(header.Length) : defaultValue;
        }

        public void Put(NHibernate.ISession session, IEnumerable<CDR> cdrs)
        {
            System.IO.DirectoryInfo storeInfo = new DirectoryInfo(ConfigString);
            long counter = 0;
            CDR currentCDR = null;
            Dictionary<string, StreamWriter> writers = new Dictionary<string, StreamWriter>();
            bool useZip = "TRUE".Equals(Option("ZIP", "FALSE"));

            try
            {
                string fileNameFormat = Option("FORMAT", "CDR.{0:yyyyMMddHH}.cdr");
                StringBuilder sb = new StringBuilder();
                foreach (CDR cdr in cdrs)
                {
                    counter++;
                    currentCDR = cdr;

                    // The cdr file
                    string name = string.Format(fileNameFormat, cdr.AttemptDateTime, cdr.Switch.SwitchID);
                    string filename = string.Format("{0}\\{1}", storeInfo.FullName, name);
                    if (useZip && !filename.ToLower().EndsWith(".gz"))
                        filename += ".gz";
                    // The writer
                    StreamWriter writer = null;
                    if (!writers.TryGetValue(filename, out writer))
                    {
                        log.InfoFormat("Creating writer for {0}", filename);
                        FileInfo file = new FileInfo(filename);
                        
                        // Create a 64K buffered stream
                        FileStream fileStream = new FileStream(file.FullName, FileMode.Append, FileAccess.Write, FileShare.Read, 64 * 1024, true);
                        System.IO.Stream stream = 
                            useZip
                            ? (System.IO.Stream)new System.IO.Compression.GZipStream(fileStream, System.IO.Compression.CompressionMode.Compress)
                            : (System.IO.Stream)fileStream;
                        writer = new StreamWriter(stream);
                        writer.AutoFlush = true;
                        writer.NewLine = "\n";
                        writers[filename] = writer;
                    }
                    // write
                    writer.WriteLine(cdr.ToString());
                }
                
            }
            catch (Exception ex)
            {
                log.Error(string.Format("CDR file writer failure (error) at #{2}, ID: {0}, Tag: {1}", currentCDR.IDonSwitch, currentCDR.Tag, counter), ex);
                throw ex;
            }
            finally
            {
                foreach (StreamWriter writer in writers.Values)
                {
                    writer.Flush();
                    writer.Close();                    
                }
                log.Info("Flushed all files");
            }
        }
        
        

        public IEnumerable<CDR> Get(NHibernate.ISession session, DateTime from, DateTime till)
        {
            throw new NotImplementedException("File CDR Store Does not support Getting CDRs");
        }

        #endregion
    }
}
