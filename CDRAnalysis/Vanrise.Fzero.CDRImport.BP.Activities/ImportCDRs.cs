using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rebex.Net;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing;
using System.IO;

namespace Vanrise.Fzero.CDRImport.BP.Activities
{

    #region Arguments Classes

    public class ImportCDRsInput
    {
        public BaseQueue<ImportedCDRBatch> OutputQueue { get; set; }

    }

    #endregion

    public class ImportCDRs : BaseAsyncActivity<ImportCDRsInput>
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<ImportedCDRBatch>> OutputQueue { get; set; }

        #endregion

        protected override void DoWork(ImportCDRsInput inputArgument, AsyncActivityHandle handle)
        {

            var sftp = new Rebex.Net.Sftp();
            sftp.Connect("192.168.110.241");
            sftp.Login("root", "P@ssw0rd");
            if (sftp.GetConnectionState().Connected)
            {
                // set current directory
                sftp.ChangeDirectory("/var/mysql/5.5/data");
                // get items within the current directory
                SftpItemCollection currentItems = sftp.GetList();
                if (currentItems.Count > 0)
                {
                    foreach (var fileObj in currentItems)
                    {
                        if (!fileObj.IsDirectory && fileObj.Name.ToUpper().Contains(".DAT"))
                        {
                            String filePath = "/var/mysql/5.5/data" + "/" + fileObj.Name;
                            String newFilePath =  "/var/mysql/5.5/data" + "/" + fileObj.Name.Replace(".DAT", ".old");
                            
                            var stream = new MemoryStream();
                            sftp.GetFile(filePath, stream);
                            byte[] data = stream.ToArray();

                            List<CDR> CDRs = new List<CDR>();
                            using (var ms = stream)
                            {
                                ms.Position = 0;
                                var sr = new StreamReader(ms);

                                while (! sr.EndOfStream)
                                {
                                    var i = sr.ReadLine();

                                    CDR cdr = new CDR();
                                    cdr.Source_Type = i.Substring(0, 9);
                                    cdr.Source_Name = i.Substring(9, 19);
                                    cdr.Source_File = i.Substring(20, 29);
                                    cdr.Record_Type = i.Substring(30, 39);
                                    cdr.Call_Type = i.Substring(40, 49);
                                    cdr.IMEI = i.Substring(50, 59);
                                    cdr.IMEI14 = i.Substring(60, 69);
                                    cdr.Entity = i.Substring(70, 79);
                                    CDRs.Add(cdr);
                                }
                            }

                            inputArgument.OutputQueue.Enqueue(new ImportedCDRBatch()
                            {
                                cdrs = CDRs
                            });

                            sftp.Rename(filePath, newFilePath);
                            break;
                        }
                    }
                }
                sftp.Disconnect();
            }

        }

        protected override ImportCDRsInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new ImportCDRsInput
            {
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

    }
}
