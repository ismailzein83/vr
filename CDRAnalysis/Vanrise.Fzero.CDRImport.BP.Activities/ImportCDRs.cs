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
using System.Globalization;

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
                                    cdr.MSISDN = i.Substring(145,20).Trim();
                                    cdr.IMSI = i.Substring(125,20).Trim();
                                    //cdr.ConnectDateTime = DateTime.ParseExact(i.Substring(221, 14).Trim(),"yyyyMddHHmmss",CultureInfo.InvariantCulture);
                                    cdr.Destination = i.Substring( 198,20).Trim();
                                    cdr.DurationInSeconds = i.Substring( 434,10).Trim();
                                    cdr.Call_Class = i.Substring( 235,5).Trim();
                                    cdr.Call_Type = i.Substring( 102,3).Trim();
                                    cdr.Sub_Type = i.Substring( 165,10).Trim();
                                    cdr.IMEI = i.Substring( 105,20).Trim();
                                    cdr.BTS_Id = i.Substring(252, 21).Trim();
                                    cdr.Cell_Id = i.Substring( 252,22).Trim();
                                    cdr.Up_Volume = i.Substring( 588,10).Trim();
                                    cdr.Down_Volume = i.Substring( 598,10).Trim();
                                    cdr.Cell_Latitude = i.Substring( 609,9).Trim();
                                    cdr.Cell_Longitude = i.Substring( 618,9).Trim();
                                    cdr.In_Trunk = i.Substring( 414,20).Trim();
                                    cdr.Out_Trunk = i.Substring( 394,20).Trim();

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
