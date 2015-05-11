using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Data;
using System.Data.SqlClient;
using System.IO;
using Vanrise.Data.SQL;


namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class NumberProfileDataManager : BaseSQLDataManager, INumberProfileDataManager
    {
        public NumberProfileDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public void LoadCDR(DateTime from, DateTime to, int? batchSize, Action<NormalCDR> onBatchReady)
        {
            string query_GetCDRRange = @"SELECT  [Id] ,[MSISDN] ,[IMSI] ,[ConnectDateTime] ,[Destination] ,[DurationInSeconds] ,[DisconnectDateTime] ,[Call_Class]  ,[IsOnNet] ,[Call_Type] ,[Sub_Type] ,[IMEI]
                                                ,[BTS_Id]  ,[Cell_Id]  ,[SwitchRecordId]  ,[Up_Volume]  ,[Down_Volume] ,[Cell_Latitude]  ,[Cell_Longitude]  ,[In_Trunk]  ,[Out_Trunk]  ,[Service_Type]  ,[Service_VAS_Name] FROM NormalCDR
                                                 with(nolock)    where connectDateTime >= @From and connectDateTime <=@To  order by MSISDN; ";



            ExecuteReaderText(query_GetCDRRange, (reader) =>
                {

                    NormalCDR normalCDR = new NormalCDR();

                    normalCDR.callType = GetReaderValue<int>(reader, "Call_Type");
                    normalCDR.bTSId = GetReaderValue<int>(reader, "BTS_Id");
                    normalCDR.connectDateTime = GetReaderValue<DateTime>(reader, "ConnectDateTime");
                    normalCDR.id = (int)reader["Id"];
                    normalCDR.iMSI = reader["IMSI"] as string;
                    normalCDR.durationInSeconds = GetReaderValue<Decimal>(reader, "DurationInSeconds");
                    normalCDR.disconnectDateTime = GetReaderValue<DateTime>(reader, "DisconnectDateTime");
                    normalCDR.callClass = reader["Call_Class"] as string;
                    normalCDR.isOnNet = GetReaderValue<Byte>(reader, "IsOnNet");
                    normalCDR.subType = reader["Sub_Type"] as string;
                    normalCDR.iMEI = reader["IMEI"] as string;
                    normalCDR.cellId = reader["Cell_Id"] as string;
                    normalCDR.switchRecordId = GetReaderValue<int>(reader, "SwitchRecordId");
                    normalCDR.upVolume = GetReaderValue<Decimal>(reader, "Up_Volume");
                    normalCDR.downVolume = GetReaderValue<Decimal>(reader, "Down_Volume");
                    normalCDR.cellLatitude = GetReaderValue<Decimal>(reader, "Cell_Latitude");
                    normalCDR.cellLongitude = GetReaderValue<Decimal>(reader, "Cell_Longitude");
                    normalCDR.inTrunk = reader["In_Trunk"] as string;
                    normalCDR.outTrunk = reader["Out_Trunk"] as string;
                    normalCDR.serviceType = GetReaderValue<int>(reader, "Service_Type");
                    normalCDR.serviceVASName = reader["Service_VAS_Name"] as string;
                    normalCDR.destination = reader["Destination"] as string;
                    normalCDR.mSISDN = reader["MSISDN"] as string;

                    onBatchReady(normalCDR);



                    //List<NumberProfile> numberProfileBatch = new List<NumberProfile>();

                    //// CDR Facts
                    //string _mSISDN = string.Empty;
                    //string _destination = string.Empty;
                    //int _callType=0;
                    //int _bTSId=0;
                    //int _id = 0;
                    //string _iMSI = string.Empty;
                    //decimal _durationInSeconds = 0;
                    //DateTime _disconnectDateTime = new DateTime();
                    //string _callClass = string.Empty;
                    //short _isOnNet = 0;
                    //string _subType = string.Empty;
                    //string _iMEI = string.Empty;
                    //string _cellId = string.Empty;
                    //int _switchRecordId = 0;
                    //decimal _upVolume = 0;
                    //decimal _downVolume = 0;
                    //decimal _cellLatitude = 0;
                    //decimal _cellLongitude = 0;
                    //string _inTrunk = string.Empty;
                    //string _outTrunk = string.Empty;
                    //int _serviceType = 0;
                    //string _serviceVASName = string.Empty;
                    //DateTime _connectDateTime = new DateTime();




                    //// Agregates
                    //NumberProfile numberProfie = new NumberProfile();
                    //HashSet<string> DestinationsIn = new HashSet<string>();
                    //HashSet<string> DestinationsOut = new HashSet<string>();

                    //HashSet<string> MSISDNsIn = new HashSet<string>();
                    //HashSet<string> MSISDNsOut = new HashSet<string>();

                    //HashSet<int> BTSIds= new HashSet<int>();
                    //HashSet<string> IMEIs = new HashSet<string>();
                    //HashSet<decimal> callOutDurs= new HashSet<decimal>();
                    //HashSet<decimal> callInDurs = new HashSet<decimal>();
                    //int countOutCalls=0;
                    //int countInCalls = 0;
                    //int countOutFails = 0;
                    //int countInFails = 0;
                    //int countInOffNets = 0;
                    //int countOutOffNets = 0;
                    //int countInOnNets = 0;
                    //int countOutOnNets = 0;
                    //int countInInters = 0;
                    //int countOutInters = 0;
                    //int countOutSMSs = 0;
                    //decimal totalDataVolume = 0;

                    //int count = 0;
                    //int currentIndex = 0;
                    //while (reader.Read())
                    //{
                    //    currentIndex++;
                    //    if(currentIndex == 10000)
                    //    {
                    //        count += currentIndex;
                    //        currentIndex = 0;
                    //        Console.WriteLine("{0} rows read", count);
                    //    }

                    //    _callType = GetReaderValue<int>(reader, "Call_Type");
                    //    _bTSId = GetReaderValue<int>(reader, "BTS_Id");
                    //    _connectDateTime = GetReaderValue<DateTime>(reader, "ConnectDateTime");
                    //    _id = (int)reader["Id"];
                    //    _iMSI = reader["IMSI"] as string;
                    //    _durationInSeconds = GetReaderValue<Decimal>(reader, "DurationInSeconds");
                    //    _disconnectDateTime = GetReaderValue<DateTime>(reader, "DisconnectDateTime");
                    //    _callClass = reader[ "Call_Class"] as string;
                    //    _isOnNet = GetReaderValue<Byte>(reader, "IsOnNet");
                    //    _subType = reader[ "Sub_Type"] as string;
                    //    _iMEI = reader[ "IMEI"] as string;
                    //    _cellId = reader[ "Cell_Id"] as string;
                    //    _switchRecordId = GetReaderValue<int>(reader, "SwitchRecordId");
                    //    _upVolume = GetReaderValue<Decimal>(reader, "Up_Volume");
                    //    _downVolume = GetReaderValue<Decimal>(reader, "Down_Volume");
                    //    _cellLatitude = GetReaderValue<Decimal>(reader, "Cell_Latitude");
                    //    _cellLongitude = GetReaderValue<Decimal>(reader, "Cell_Longitude");
                    //    _inTrunk = reader[ "In_Trunk"] as string;
                    //    _outTrunk = reader[ "Out_Trunk"] as string;
                    //    _serviceType = GetReaderValue<int>(reader, "Service_Type");
                    //    _serviceVASName = reader[ "Service_VAS_Name"] as string;
                    //    _destination = reader["Destination"] as string;

                    //    //continue;




                    //    if (_mSISDN == string.Empty)
                    //    {
                    //        numberProfie = new NumberProfile();
                    //        _mSISDN = reader["MSISDN"] as string;
                    //    }

                    //    else if (_mSISDN != reader["MSISDN"] as string)
                    //    {
                    //        numberProfileBatch.Add(numberProfie);
                    //        if (batchSize.HasValue && numberProfileBatch.Count == batchSize)
                    //        {
                    //            onBatchReady(numberProfileBatch);
                    //            numberProfileBatch = new List<NumberProfile>();
                    //        }

                    //        numberProfie = new NumberProfile();
                    //        DestinationsIn = new HashSet<string>();
                    //        DestinationsOut = new HashSet<string>();
                    //        MSISDNsIn = new HashSet<string>();
                    //        MSISDNsOut = new HashSet<string>();
                    //        BTSIds = new HashSet<int>();
                    //        IMEIs = new HashSet<string>();
                    //        callOutDurs = new HashSet<decimal>();
                    //        callInDurs = new HashSet<decimal>();
                    //        countOutCalls = 0;
                    //        countInCalls = 0;
                    //        countOutFails = 0;
                    //        countInFails = 0;
                    //        countInOffNets = 0;
                    //        countOutOffNets = 0;
                    //        countInOnNets = 0;
                    //        countOutOnNets = 0;
                    //        countInInters = 0;
                    //        countOutInters = 0;
                    //        countOutSMSs = 0;
                    //        totalDataVolume = 0;

                    //        _mSISDN = reader["MSISDN"] as string;
                    //    }


                    //    numberProfie.subscriberNumber = _mSISDN;
                        


                       




                       

                    //  // Filling Agregates
                        
                    //    numberProfie.periodId = PeriodId;
                    //    numberProfie.fromDate=_connectDateTime;
                    //    numberProfie.isOnNet = 1;
                        
                    //    totalDataVolume += (_upVolume + _downVolume);
                    //    numberProfie.totalDataVolume = totalDataVolume ;




                    //    if (PeriodId == (int)Enums.Period.Day)
                    //    {
                    //        numberProfie.toDate=_connectDateTime.AddDays(1);
                    //    }


                    //    if (_callType == (int)Enums.CallType.outgoingVoiceCall)
                    //    {
                    //        numberProfie.countOutCalls = ++countOutCalls;


                    //        if (!DestinationsOut.Contains(_destination))
                    //        {
                    //            DestinationsOut.Add(_destination);
                    //            numberProfie.diffOutputNumb = DestinationsOut.Count();
                    //        }

                    //        if (!DestinationsOut.Contains(_destination))
                    //        {
                    //            DestinationsOut.Add(_destination);
                    //            numberProfie.diffOutputNumb = DestinationsOut.Count();
                    //        }
                          
                                   

                    //        if (_durationInSeconds == 0)
                    //            numberProfie.countOutFail = ++countOutFails;
                    //        else
                    //        {
                    //            callOutDurs.Add(_durationInSeconds / 60);
                    //            numberProfie.callOutDurAvg = callOutDurs.Average();
                    //            numberProfie.totalInVolume = callOutDurs.Sum();
                    //        }

                    //        if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ASIACELL) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.KOREKTEL)))
                    //            numberProfie.countOutOffNet = ++countOutOffNets;
                    //        else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ZAINIQ) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.VAS) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INV)))
                    //            numberProfie.countOutOnNet = ++countOutOnNets;
                    //        else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INTL)))
                    //            numberProfie.countOutInter = ++countOutInters;
                    //    }
                            

                    //    if (_callType == (int)Enums.CallType.incomingVoiceCall)
                    //    {
                    //        numberProfie.countOutCalls = ++countInCalls;

                    //        if (!DestinationsIn.Contains(_destination))
                    //        {
                    //            DestinationsIn.Add(_destination);
                    //            numberProfie.diffInputNumbers = DestinationsIn.Count();
                    //        }
                                   

                    //        if (_durationInSeconds == 0)
                    //            numberProfie.countInFail = ++countInFails;
                    //        else
                    //        {
                    //            callInDurs.Add(_durationInSeconds / 60);
                    //            numberProfie.callInDurAvg = callInDurs.Average();
                    //            numberProfie.totalInVolume = callInDurs.Sum();
                    //        }

                    //        if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ASIACELL) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.KOREKTEL)))
                    //            numberProfie.countInOffNet = ++countInOffNets;
                    //        else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ZAINIQ) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.VAS) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INV)))
                    //            numberProfie.countInOnNet = ++countInOnNets;
                    //        else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INTL)))
                    //            numberProfie.countInInter = ++countInInters;
                    //    }

                    //    else if (_callType == (int)Enums.CallType.outgoingSms)
                    //    {
                    //        numberProfie.countOutSMS = ++countOutSMSs;
                    //    }
                            
                       



                    //    if ((_callType == (int)Enums.CallType.incomingVoiceCall || _callType == (int)Enums.CallType.outgoingVoiceCall || _callType == (int)Enums.CallType.incomingSms || _callType == (int)Enums.CallType.outgoingSms))
                    //    {
                    //        if (!IMEIs.Contains(_iMEI) )
                    //            IMEIs.Add(_iMEI);
                    //        numberProfie.totalIMEI = IMEIs.Count();



                    //        if (!BTSIds.Contains(_bTSId) )
                    //            BTSIds.Add(_bTSId);
                    //        numberProfie.totalBTS = BTSIds.Count();
                    //    }
                    //}

                    //if (numberProfileBatch.Count > 0)
                    //    onBatchReady(numberProfileBatch);

                },
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@From", from));
                    cmd.Parameters.Add(new SqlParameter("@To", to));
                });
        }

    }
}
