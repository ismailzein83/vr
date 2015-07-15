using System;
using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.BusinessProcess;
//using Vanrise.BusinessProcess.Client;
//using Vanrise.BusinessProcess.Entities;
//using Vanrise.Fzero.CDRImport.Entities;
//using Vanrise.Queueing;
//using Vanrise.Runtime;
//using Vanrise.Fzero.FraudAnalysis.Data.MySQL;
//using Vanrise.Integration.Adapters.BaseDB;
//using Vanrise.Integration.Adapters.SQLReceiveAdapter;
using Vanrise.Integration.Entities;
//using Vanrise.Integration.Data;
//using System.Globalization;



namespace Vanrise.Fzero.DevRuntime.Tasks
{
    public class WalidTask : ITask
    {
        //public void Execute()
        //{
        //    Console.WriteLine("Walid Task started");

        //    var adapter = new FTPReceiveAdapter();
        //    adapter.ActionAfterImport = (int) Integration.Adapters.BaseFTP.TPReceiveAdapter.Actions.Rename;
        //    adapter.Directory = "/CDRAnalysis/DAT";
        //    adapter.Extension = ".DAT";
        //    adapter.Password = "P@ssw0rd";
        //    adapter.UserName = "devftpuser";
        //    adapter.ServerIP = "192.168.110.185"; 

        //    List<CDR> CDRs = new List<CDR>();


        //    adapter.ImportData( 
                
                
        //        data => {   

        //        while (!((StreamReaderImportedData)data).StreamReader.EndOfStream)
        //        {
        //            var i = ((StreamReaderImportedData)data).StreamReader.ReadLine();

        //            CDR cdr = new CDR();
        //            cdr.MSISDN = i.Substring(145, 20).Trim();
        //            cdr.IMSI = i.Substring(125, 20).Trim();
        //            cdr.Destination = i.Substring(198, 20).Trim();
        //            cdr.CallClass = i.Substring(434, 10).Trim();
        //            cdr.SubType = i.Substring(165, 10).Trim();
        //            cdr.IMEI = i.Substring(105, 20).Trim();
        //            cdr.CellId = i.Substring(252, 22).Trim();
        //            cdr.InTrunk = i.Substring(414, 20).Trim();
        //            cdr.OutTrunk = i.Substring(394, 20).Trim();


        //            DateTime ConnectDateTime;
        //            if (DateTime.TryParseExact(i.Substring(221, 14).Trim(), "yyyyMddHHmmss", CultureInfo.InvariantCulture,
        //                                       DateTimeStyles.None, out ConnectDateTime))
        //                cdr.ConnectDateTime = ConnectDateTime;



        //            int callType = 0;
        //            if (int.TryParse(i.Substring(102, 3).Trim(), out callType))
        //                cdr.CallType = callType;

        //            decimal cellLatitude;
        //            if (decimal.TryParse(i.Substring(609, 9).Trim(), out cellLatitude))
        //                cdr.CellLatitude = cellLatitude;


        //            decimal durationInSeconds;
        //            if (decimal.TryParse(i.Substring(235, 5).Trim(), out durationInSeconds))
        //                cdr.DurationInSeconds = durationInSeconds;


        //            decimal upVolume;
        //            if (decimal.TryParse(i.Substring(588, 10).Trim(), out upVolume))
        //                cdr.UpVolume = upVolume;


        //            decimal cellLongitude;
        //            if (decimal.TryParse(i.Substring(618, 9).Trim(), out cellLongitude))
        //                cdr.CellLongitude = cellLongitude;


        //            decimal downVolume;
        //            if (decimal.TryParse(i.Substring(598, 10).Trim(), out downVolume))
        //                cdr.DownVolume = downVolume;


        //            CDRs.Add(cdr);
        //        }
            
        //    });

        //    Console.Write(CDRs.Count);
        //    Console.WriteLine("END");
        //    Console.ReadKey();
        //}



        public void Execute()
        {
            Console.WriteLine("Walid Task started");

            var adapter = new Vanrise.Integration.Adapters.SQLReceiveAdapter.SQLReceiveAdapter();
            adapter.ConnectionString = "server=192.168.110.185;user id=walid;password=P@ssw0rd;persistsecurityinfo=True;database=CDRAnalysisMobile_WF";
            adapter.Description = "Description";
            adapter.Query = "select top 10 * from FraudAnalysis.NormalCDR";

            adapter.ImportData(

                data =>
                {
                    Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch();
                    batch.cdrs = new List<Vanrise.Fzero.CDRImport.Entities.CDR>();
                    System.IO.StreamReader sr = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data)).StreamReader;
                    while (!sr.EndOfStream)
                    {
                        var i = sr.ReadLine();

                        Vanrise.Fzero.CDRImport.Entities.CDR cdr = new Vanrise.Fzero.CDRImport.Entities.CDR();
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
                        if (DateTime.TryParseExact(i.Substring(221, 14).Trim(), "yyyyMddHHmmss", System.Globalization.CultureInfo.InvariantCulture,
                                                   System.Globalization.DateTimeStyles.None, out ConnectDateTime))
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


                        batch.cdrs.Add(cdr);
                    }

                }  
                );

            Console.WriteLine("END");
            Console.ReadKey();
        }

    }
}
