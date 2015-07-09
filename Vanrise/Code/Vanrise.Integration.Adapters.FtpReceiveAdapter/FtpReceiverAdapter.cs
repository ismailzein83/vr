using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.FtpReceiverAdapter
{
    public class FileReceiveAdapter : BaseReceiveAdapter
    {





        public string FolderPath { get; set; }

        public override void ImportData(Action<IImportedData> receiveData)
        {
            //foreach (var fileObj in Directory.EnumerateFiles(this.FolderPath, "*.txt"))
            //{
                
            //}




        //    string sFTPDir = System.Configuration.ConfigurationManager.AppSettings["SFTP_Dir"].ToString();
        //    handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Started Importing CDRs to Transit Database");
        //    var sftp = new Rebex.Net.Sftp();
        //    sftp.Connect(System.Configuration.ConfigurationManager.AppSettings["SERVER"].ToString());
        //    sftp.Login(System.Configuration.ConfigurationManager.AppSettings["Server_Username"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Server_Pasword"].ToString());
        //    if (sftp.GetConnectionState().Connected)
        //    {
        //        // set current directory
        //        sftp.ChangeDirectory(sFTPDir);
        //        // get items within the current directory
        //        SftpItemCollection currentItems = sftp.GetList();
        //        if (currentItems.Count > 0)
        //        {
        //            foreach (var fileObj in currentItems)
        //            {
        //                if (!fileObj.IsDirectory && fileObj.Name.ToUpper().Contains(".DAT"))
        //                {
        //                    String filePath = sFTPDir + "/" + fileObj.Name;
        //                    String newFilePath = sFTPDir + "/" + fileObj.Name.Replace(".DAT", ".old");

        //                    var stream = new MemoryStream();
        //                    sftp.GetFile(filePath, stream);
        //                    byte[] data = stream.ToArray();
        //                    List<CDR> CDRs = new List<CDR>();
        //                    using (var ms = stream)
        //                    {
        //                        ms.Position = 0;
        //                        var sr = new StreamReader(ms);

        //                        while (!sr.EndOfStream)
        //                        {
        //                            var i = sr.ReadLine();

        //                            CDR cdr = new CDR();
        //                            cdr.MSISDN = i.Substring(145, 20).Trim();
        //                            cdr.IMSI = i.Substring(125, 20).Trim();
        //                            cdr.Destination = i.Substring(198, 20).Trim();
        //                            cdr.CallClass = i.Substring(434, 10).Trim();
        //                            cdr.SubType = i.Substring(165, 10).Trim();
        //                            cdr.IMEI = i.Substring(105, 20).Trim();
        //                            cdr.CellId = i.Substring(252, 22).Trim();
        //                            cdr.InTrunk = i.Substring(414, 20).Trim();
        //                            cdr.OutTrunk = i.Substring(394, 20).Trim();


        //                            DateTime ConnectDateTime;
        //                            if (DateTime.TryParseExact(i.Substring(221, 14).Trim(), "yyyyMddHHmmss", CultureInfo.InvariantCulture,
        //                                                       DateTimeStyles.None, out ConnectDateTime))
        //                                cdr.ConnectDateTime = ConnectDateTime;



        //                            int callType = 0;
        //                            if (int.TryParse(i.Substring(102, 3).Trim(), out callType))
        //                                cdr.CallType = callType;

        //                            decimal cellLatitude;
        //                            if (decimal.TryParse(i.Substring(609, 9).Trim(), out cellLatitude))
        //                                cdr.CellLatitude = cellLatitude;


        //                            decimal durationInSeconds;
        //                            if (decimal.TryParse(i.Substring(235, 5).Trim(), out durationInSeconds))
        //                                cdr.DurationInSeconds = durationInSeconds;


        //                            decimal upVolume;
        //                            if (decimal.TryParse(i.Substring(588, 10).Trim(), out upVolume))
        //                                cdr.UpVolume = upVolume;


        //                            decimal cellLongitude;
        //                            if (decimal.TryParse(i.Substring(618, 9).Trim(), out cellLongitude))
        //                                cdr.CellLongitude = cellLongitude;


        //                            decimal downVolume;
        //                            if (decimal.TryParse(i.Substring(598, 10).Trim(), out downVolume))
        //                                cdr.DownVolume = downVolume;


        //                            CDRs.Add(cdr);
        //                        }
        //                    }

        //                    inputArgument.OutputQueue.Enqueue(new ImportedCDRBatch()
        //                    {
        //                        cdrs = CDRs
        //                    });
        //                    sftp.Rename(filePath, newFilePath);

        //                }
        //            }
        //        }
        //        sftp.Disconnect();
        //    }
        //    handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Finshed Importing CDRs to Transit Database");








































        //    receiveData(new TextFileImportedData() { Content = "Extracted Data" });
        }






        















    }
}
