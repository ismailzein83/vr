using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Data;
using MySql.Data.MySqlClient;
using System.IO;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Data.MySQL;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class NumberProfileDataManager : BaseMySQLDataManager, INumberProfileDataManager
    {
        public NumberProfileDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        //public void LoadNumberProfile(DateTime from, DateTime to,  int? batchSize, Action<List<Vanrise.Fzero.FraudAnalysis.Entities.NumberProfile>> onBatchReady)
        //{

        //    int PeriodId = 6;


        //    MySQLManager manager = new MySQLManager();
        //    string query_GetCDRRange = "SELECT SQL_BIG_RESULT * FROM NormalCDR  where connectDateTime >= @From and connectDateTime <=@To  order by MSISDN ;";
        //    manager.ExecuteReader(query_GetCDRRange,
        //        (cmd) =>
        //        {
        //            cmd.Parameters.AddWithValue("@From", from);
        //            cmd.Parameters.AddWithValue("@To", to);
        //        }, (reader) =>
        //        {
        //            List<NumberProfile> numberProfileBatch = new List<NumberProfile>();

        //            // CDR Facts
        //            string _mSISDN = string.Empty;
        //            string _destination = string.Empty;
        //            int _callType=0;
        //            int _bTSId=0;
        //            int _id = 0;
        //            string _iMSI = string.Empty;
        //            decimal _durationInSeconds = 0;
        //            DateTime _disconnectDateTime = new DateTime();
        //            string _callClass = string.Empty;
        //            short _isOnNet = 0;
        //            string _subType = string.Empty;
        //            string _iMEI = string.Empty;
        //            string _cellId = string.Empty;
        //            int _switchRecordId = 0;
        //            decimal _upVolume = 0;
        //            decimal _downVolume = 0;
        //            decimal _cellLatitude = 0;
        //            decimal _cellLongitude = 0;
        //            string _inTrunk = string.Empty;
        //            string _outTrunk = string.Empty;
        //            int _serviceType = 0;
        //            string _serviceVASName = string.Empty;
        //            DateTime _connectDateTime = new DateTime();



        //            int index=0;



        //            // Agregates
        //            NumberProfile numberProfie = new NumberProfile();
        //            HashSet<string> DestinationsIn = new HashSet<string>();
        //            HashSet<string> DestinationsOut = new HashSet<string>();
        //            HashSet<int> BTSIds= new HashSet<int>();
        //            HashSet<string> IMEIs = new HashSet<string>();
        //            HashSet<decimal> callOutDurs= new HashSet<decimal>();
        //            HashSet<decimal> callInDurs = new HashSet<decimal>();
        //            int countOutCalls=0;
        //            int countInCalls = 0;
        //            int countOutFails = 0;
        //            int countInFails = 0;
        //            int countInOffNets = 0;
        //            int countOutOffNets = 0;
        //            int countInOnNets = 0;
        //            int countOutOnNets = 0;
        //            int countInInters = 0;
        //            int countOutInters = 0;
        //            int countOutSMSs = 0;
        //            decimal totalDataVolume = 0;


        //            while (reader.Read())
        //            {

        //                //Console.WriteLine((++index).ToString());

        //                _destination = reader["Destination"].ToString();
        //                _callType = Helper.AsInt(reader["Call_Type"].ToString());
        //                _bTSId = Helper.AsInt(reader["BTS_Id"].ToString());
        //                _connectDateTime = Helper.AsDateTime(reader["ConnectDateTime"].ToString());
        //                _id = Helper.AsInt(reader["Id"].ToString());
        //                _iMSI = reader["IMSI"].ToString();
        //                _durationInSeconds = Helper.AsDecimal(reader["DurationInSeconds"].ToString());
        //                _disconnectDateTime = Helper.AsDateTime(reader["DisconnectDateTime"].ToString());
        //                _callClass = reader["Call_Class"].ToString();
        //                _isOnNet = Helper.AsShortInt(reader["IsOnNet"].ToString());
        //                _subType = reader["Sub_Type"].ToString();
        //                _iMEI = reader["IMEI"].ToString();
        //                _cellId = reader["Cell_Id"].ToString();
        //                _switchRecordId = Helper.AsInt(reader["SwitchRecordId"].ToString());
        //                _upVolume = Helper.AsDecimal(reader["Up_Volume"].ToString());
        //                _downVolume = Helper.AsDecimal(reader["Down_Volume"].ToString());
        //                _cellLatitude = Helper.AsDecimal(reader["Cell_Latitude"].ToString());
        //                _cellLongitude = Helper.AsDecimal(reader["Cell_Longitude"].ToString());
        //                _inTrunk = reader["In_Trunk"].ToString();
        //                _outTrunk = reader["Out_Trunk"].ToString();
        //                _serviceType = Helper.AsInt(reader["Service_Type"].ToString());
        //                _serviceVASName = reader["Service_VAS_Name"].ToString();

                        
        //                //Check if New MSISDN
        //                if (_mSISDN == string.Empty)
        //                {
        //                    numberProfie = new NumberProfile();
        //                    _mSISDN = reader["MSISDN"].ToString();
        //                    countOutCalls=0;
        //                }

        //                else if (_mSISDN != reader["MSISDN"].ToString())
        //                {
        //                    numberProfileBatch.Add(numberProfie);

        //                    if (batchSize.HasValue && numberProfileBatch.Count == batchSize)
        //                    {
        //                        onBatchReady(numberProfileBatch);
        //                        numberProfileBatch = new List<NumberProfile>();
        //                    }

        //                    numberProfie = new NumberProfile();
        //                    _mSISDN = reader["MSISDN"].ToString();
        //                }




                       

        //              // Filling Agregates
        //                numberProfie.subscriberNumber = _mSISDN;
        //                numberProfie.periodId = PeriodId;
        //                numberProfie.fromDate=_connectDateTime;
        //                numberProfie.isOnNet = 1;
                        
        //                totalDataVolume += (_upVolume + _downVolume);
        //                numberProfie.totalDataVolume = totalDataVolume ;




        //                if (PeriodId == (int)Enums.Period.Day)
        //                {
        //                    numberProfie.toDate=_connectDateTime.AddDays(1);
        //                }


        //                if (_callType == (int)Enums.CallType.outgoingVoiceCall)
        //                {
        //                    numberProfie.countOutCalls = ++countOutCalls;

        //                    if (!DestinationsOut.Contains(_destination))
        //                    {
        //                        DestinationsOut.Add(_destination);
        //                        numberProfie.diffOutputNumb = DestinationsOut.Count();
        //                    }
                                   

        //                    if (_durationInSeconds == 0)
        //                        numberProfie.countOutFail = ++countOutFails;
        //                    else
        //                    {
        //                        callOutDurs.Add(_durationInSeconds / 60);
        //                        numberProfie.callOutDurAvg = callOutDurs.Average();
        //                        numberProfie.totalInVolume = callOutDurs.Sum();
        //                    }

        //                    if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ASIACELL) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.KOREKTEL)))
        //                        numberProfie.countOutOffNet = ++countOutOffNets;
        //                    else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ZAINIQ) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.VAS) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INV)))
        //                        numberProfie.countOutOnNet = ++countOutOnNets;
        //                    else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INTL)))
        //                        numberProfie.countOutInter = ++countOutInters;
        //                }
                            

        //                if (_callType == (int)Enums.CallType.incomingVoiceCall)
        //                {
        //                    numberProfie.countOutCalls = ++countInCalls;

        //                    if (!DestinationsIn.Contains(_destination))
        //                    {
        //                        DestinationsIn.Add(_destination);
        //                        numberProfie.diffInputNumbers = DestinationsIn.Count();
        //                    }
                                   

        //                    if (_durationInSeconds == 0)
        //                        numberProfie.countInFail = ++countInFails;
        //                    else
        //                    {
        //                        callInDurs.Add(_durationInSeconds / 60);
        //                        numberProfie.callInDurAvg = callInDurs.Average();
        //                        numberProfie.totalInVolume = callInDurs.Sum();
        //                    }

        //                    if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ASIACELL) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.KOREKTEL)))
        //                        numberProfie.countInOffNet = ++countInOffNets;
        //                    else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ZAINIQ) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.VAS) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INV)))
        //                        numberProfie.countInOnNet = ++countInOnNets;
        //                    else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INTL)))
        //                        numberProfie.countInInter = ++countInInters;
        //                }

        //                else if (_callType == (int)Enums.CallType.outgoingSms)
        //                {
        //                    numberProfie.countOutSMS = ++countOutSMSs;
        //                }
                            
                       



        //                if ((_callType == (int)Enums.CallType.incomingVoiceCall || _callType == (int)Enums.CallType.outgoingVoiceCall || _callType == (int)Enums.CallType.incomingSms || _callType == (int)Enums.CallType.outgoingSms))
        //                {
        //                    if (!IMEIs.Contains(_iMEI) )
        //                        IMEIs.Add(_iMEI);
        //                    numberProfie.totalIMEI = IMEIs.Count();



        //                    if (!BTSIds.Contains(_bTSId) )
        //                        BTSIds.Add(_bTSId);
        //                    numberProfie.totalBTS = BTSIds.Count();
        //                }
        //            }

        //            if (numberProfileBatch.Count > 0)
        //                onBatchReady(numberProfileBatch);

        //        });
        //}


        public void LoadCDR(DateTime from, DateTime to, int? batchSize, Action<NormalCDR> onBatchReady)
        {
            MySQLManager manager = new MySQLManager();
            string query_GetCDRRange = @"SELECT  `Id` ,`MSISDN` ,`IMSI` ,`ConnectDateTime` ,`Destination` ,`DurationInSeconds` ,`DisconnectDateTime` ,`Call_Class`  ,`IsOnNet` ,`Call_Type` ,`Sub_Type` ,`IMEI`
                                                ,`BTS_Id`  ,`Cell_Id`  ,`SwitchRecordId`  ,`Up_Volume`  ,`Down_Volume` ,`Cell_Latitude`  ,`Cell_Longitude`  ,`In_Trunk`  ,`Out_Trunk`  ,`Service_Type`  ,`Service_VAS_Name` FROM NormalCDR
                                                    where connectDateTime >= @From and connectDateTime <=@To  order by MSISDN; ";



            manager.ExecuteReader(query_GetCDRRange,
                (cmd) =>
                {
                    cmd.Parameters.AddWithValue("@From", from);
                    cmd.Parameters.AddWithValue("@To", to);
                }, (reader) =>
            {

                NormalCDR normalCDR = new NormalCDR();
                int count = 0;
                int currentIndex = 0;


                while (reader.Read())
                {
                    normalCDR.MSISDN = reader["MSISDN"] as string;
                    normalCDR.Destination = reader["Destination"].ToString();
                    normalCDR.CallType = Helper.AsInt(reader["Call_Type"].ToString());
                    normalCDR.BTSId = Helper.AsInt(reader["BTS_Id"].ToString());
                    normalCDR.ConnectDateTime = Helper.AsDateTime(reader["ConnectDateTime"].ToString());
                    normalCDR.Id = Helper.AsInt(reader["Id"].ToString());
                    normalCDR.IMSI = reader["IMSI"].ToString();
                    normalCDR.DurationInSeconds = Helper.AsDecimal(reader["DurationInSeconds"].ToString());
                    normalCDR.DisconnectDateTime = Helper.AsDateTime(reader["DisconnectDateTime"].ToString());
                    normalCDR.CallClass = reader["Call_Class"].ToString();
                    normalCDR.IsOnNet = Helper.AsShortInt(reader["IsOnNet"].ToString());
                    normalCDR.SubType = reader["Sub_Type"].ToString();
                    normalCDR.IMEI = reader["IMEI"].ToString();
                    normalCDR.CellId = reader["Cell_Id"].ToString();
                    normalCDR.SwitchRecordId = Helper.AsInt(reader["SwitchRecordId"].ToString());
                    normalCDR.UpVolume = Helper.AsDecimal(reader["Up_Volume"].ToString());
                    normalCDR.DownVolume = Helper.AsDecimal(reader["Down_Volume"].ToString());
                    normalCDR.CellLatitude = Helper.AsDecimal(reader["Cell_Latitude"].ToString());
                    normalCDR.CellLongitude = Helper.AsDecimal(reader["Cell_Longitude"].ToString());
                    normalCDR.InTrunk = reader["In_Trunk"].ToString();
                    normalCDR.OutTrunk = reader["Out_Trunk"].ToString();
                    normalCDR.ServiceType = Helper.AsInt(reader["Service_Type"].ToString());
                    normalCDR.ServiceVASName = reader["Service_VAS_Name"].ToString();

                    currentIndex++;
                    if (currentIndex == 10000)
                    {
                        count += currentIndex;
                        currentIndex = 0;
                        Console.WriteLine("{0} rows read", count);
                    }

                    onBatchReady(normalCDR);
                }



            });
        }
    }
}
