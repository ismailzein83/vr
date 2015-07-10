using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Fzero.FraudAnalysis.Data.MySQL;
using Vanrise.Integration.Adapters.FtpReceiverAdapter;
using Vanrise.Integration.Entities;
using Vanrise.Integration.Data;
using System.Globalization;


namespace Vanrise.Fzero.DevRuntime.Tasks
{
    public class WalidTask : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Walid Task started");

            var adapter = new FileReceiveAdapter();
            adapter.ActionAfterImport = Actions.Rename;
            adapter.AllowSSH = true;
            adapter.Directory = "/var/mysql/5.5/data";
            adapter.Extension = ".DAT";
            adapter.Password = "P@ssw0rd";
            adapter.UserName = "root";
            adapter.ServerIP = "192.168.110.241";

            List<CDR> CDRs = new List<CDR>();

            adapter.ImportData(data => { 
                
                StreamReaderImportedData reader = new StreamReaderImportedData();

                while (!reader.StreamReader.EndOfStream)
                {
                    var i = reader.StreamReader.ReadLine();

                    CDR cdr = new CDR();
                    cdr.MSISDN = i.Substring(145, 20).Trim();
                    cdr.IMSI = i.Substring(125, 20).Trim();
                    cdr.Destination = i.Substring(198, 20).Trim();
                    cdr.CallClass = i.Substring(434, 10).Trim();
                    cdr.SubType = i.Substring(165, 10).Trim();
                    cdr.IMEI = i.Substring(105, 20).Trim();
                    cdr.CellId = i.Substring(252, 22).Trim();
                    cdr.InTrunk = i.Substring(414, 20).Trim();
                    cdr.OutTrunk = i.Substring(394, 20).Trim();


                    DateTime ConnectDateTime;
                    if (DateTime.TryParseExact(i.Substring(221, 14).Trim(), "yyyyMddHHmmss", CultureInfo.InvariantCulture,
                                               DateTimeStyles.None, out ConnectDateTime))
                        cdr.ConnectDateTime = ConnectDateTime;



                    int callType = 0;
                    if (int.TryParse(i.Substring(102, 3).Trim(), out callType))
                        cdr.CallType = callType;

                    decimal cellLatitude;
                    if (decimal.TryParse(i.Substring(609, 9).Trim(), out cellLatitude))
                        cdr.CellLatitude = cellLatitude;


                    decimal durationInSeconds;
                    if (decimal.TryParse(i.Substring(235, 5).Trim(), out durationInSeconds))
                        cdr.DurationInSeconds = durationInSeconds;


                    decimal upVolume;
                    if (decimal.TryParse(i.Substring(588, 10).Trim(), out upVolume))
                        cdr.UpVolume = upVolume;


                    decimal cellLongitude;
                    if (decimal.TryParse(i.Substring(618, 9).Trim(), out cellLongitude))
                        cdr.CellLongitude = cellLongitude;


                    decimal downVolume;
                    if (decimal.TryParse(i.Substring(598, 10).Trim(), out downVolume))
                        cdr.DownVolume = downVolume;


                    CDRs.Add(cdr);
                }
            
            });

            Console.Write(CDRs.Count);
            Console.WriteLine("END");
            Console.ReadKey();
        }

    }
}
