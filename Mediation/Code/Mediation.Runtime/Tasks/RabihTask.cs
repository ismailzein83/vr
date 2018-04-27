using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.BP.Arguments;
using Mediation.Generic.Entities;
using Mediation.Generic.MainExtensions.MediationOutputHandlers;
using Mediation.Runtime.DataParser;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Caching.Runtime;
using Vanrise.Common;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers;
using Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.FieldParsers;
using Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.RecordParsers;
using Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.RecordParsers;
using Vanrise.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Mediation.Runtime.Tasks
{
    public class RabihTask : ITask
    {
        public void Execute()
        {
            //string called = "sip:2001@aaapartnership.multinet;sip:0@aaapartnership.multinet";

            //var calls = StripAndGetNumbers(called);

            DataParserTester tester = new DataParserTester();
            tester.GenerateMediationSettings();

            RunImportProcess();
        }

        List<string> StripAndGetNumbers(string numberIds)
        {
            List<string> numbers = new List<string>();

            foreach (var numberId in numberIds.Split(';'))
            {
                string[] calls = numberId.Split(new char[] { ':', '@' });
                if (calls.Length > 1)
                {
                    numbers.Add(calls[1]);
                }
                else
                    numbers.Add(calls[0]);
            }

            return numbers;
        }

        void RunImportProcess()
        {
            //MediationDefinition definition = new MediationDefinition
            //{
            //    OutputHandlers = new List<MediationOutputHandlerDefinition>(),
            //    ParsedRecordTypeId = new Guid(),
            //    ParsedRecordIdentificationSetting = new ParsedRecordIdentificationSetting()
            //    {
            //        StatusMappings = new List<StatusMapping>()
            //    }
            //};
            //definition.OutputHandlers.Add(new MediationOutputHandlerDefinition()
            //{
            //    Handler = new StoreRecordsOutputHandler()
            //    {
            //        DataRecordStorageId = new Guid("0B1837DF-C8CE-4B2A-B07E-1A9F75408741")
            //    },
            //    OutputRecordName = "cookedCDR"
            //});
            //string serielized = Vanrise.Common.Serializer.Serialize(definition);

            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            //SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(summaryQueueActivationService);

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(schedulerService);

            //Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(bigDataService);


            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            //CachingRuntimeService cachingRuntimeService = new CachingRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(cachingRuntimeService);

            //CachingDistributorRuntimeService cachingDistributorRuntimeService = new CachingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(cachingDistributorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            Console.ReadKey();
        }

        #region Huawei Ogero

        private string GetHuaweiParserSettings_Ogero()
        {
            BinaryParserType hexParser = new BinaryParserType
            {
                RecordParser = new HeaderRecordParser
                {
                    RecordLengthIndex = 5,
                    HeaderLength = 3,
                    PackageRecordParser = new BinaryRecordParser
                    {
                        Settings = new SplitByTagRecordParser
                        {
                            SubRecordsParsersByTag = GetHuaweiSubRecordsParser()
                        }
                    }
                }
            };

            ParserType parserType = new ParserType
            {
                Name = "Huawei Ogero Parser",
                ParserTypeId = new Guid("504A12E9-61D2-4E31-B193-1D43749DC055"),
                Settings = new ParserTypeSettings
                {
                    ExtendedSettings = hexParser
                }
            };

            return Serializer.Serialize(parserType.Settings);
        }

        private Dictionary<string, BinaryRecordParser> GetHuaweiSubRecordsParser()
        {
            Dictionary<string, BinaryRecordParser> result = new Dictionary<string, BinaryRecordParser>();

            return result;
        }

        private Dictionary<string, BinaryRecordParser> GetTemplateParsers_Huawei_Ogero()
        {
            Dictionary<string, BinaryRecordParser> parsers = new Dictionary<string, BinaryRecordParser>();

            parsers.Add("BF45", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_B4_FieldParsers_Huawei_Ogero()
                    },
                    RecordType = "GPRS",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 2}
                    }
                }

            });

            return parsers;
        }

        private Dictionary<string, BinaryFieldParser> Get_B4_FieldParsers_Huawei_Ogero()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            parsers.Add("80", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });


            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9D", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargingID",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("96", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            parsers.Add("87", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "AccessPointNameNI"
                }
            });

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "PDPType"
                }
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ServedPDPAddress",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new BoolFieldParser
                {
                    FieldName = "DynamicAddressFlag"
                }
            });

            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = GetHuaweiDateTimeParser("RecordOpeningTime")
            });

            parsers.Add("9F26", new BinaryFieldParser
            {
                Settings = GetHuaweiDateTimeParser("StartTime")
            });

            parsers.Add("9F27", new BinaryFieldParser
            {
                Settings = GetHuaweiDateTimeParser("StopTime")
            });

            parsers.Add("8E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "Duration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForRecClosing",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("91", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordSequenceNumber",
                    NumberType = NumberType.Int
                }
            });


            parsers.Add("92", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "NodeID"
                }
            });

            parsers.Add("94", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocalSequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("95", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "APNSelectionMode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("97", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargingCharacteristics",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("98", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChSelectionMode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RATType",
                    NumberType = NumberType.Int
                }
            });
            parsers.Add("9F2D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "PDNConnectionID",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F25", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GWPLMNIdentifier "
                }
            });

            parsers.Add("9F20", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "UserLocationInformation"
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "GWAddress"                                          
                                     }                            
                                }
                            },
                            {"82", new BinaryFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "GWAddress"                                          
                                     }                            
                                }
                            },
                            {"81", new BinaryFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "GWAddress"                                          
                                     }                            
                                }
                            },
                            {"83", new BinaryFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "GWAddress"                                          
                                     }                            
                                }
                            }

                        }
                    }
                }
            });

            parsers.Add("86", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "ServingNodeAddress"                                          
                                     }                            
                                }
                            },
                            {"82", new BinaryFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "ServingNodeAddress"                                          
                                     }                            
                                }
                            },
                            {"81", new BinaryFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "ServingNodeAddress"                                          
                                     }                            
                                }
                            },
                            {"83", new BinaryFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "ServingNodeAddress"                                          
                                     }                            
                                }
                            }

                        }
                    }
                }
            });


            return parsers;
        }
        DateTimeParser GetHuaweiDateTimeParser(string fieldName)
        {
            return new DateTimeParser
            {
                FieldName = fieldName,
                WithOffset = true,
                DateTimeParsingType = DateTimeParsingType.DateTime,
                YearIndex = 0,
                MonthIndex = 1,
                DayIndex = 2,
                HoursIndex = 3,
                MinutesIndex = 4,
                SecondsIndex = 5,
                TimeShiftIndicatorIndex = 6,
                HoursTimeShiftIndex = 7,
                MinutesTimeShiftIndex = 8,
                IsBCD = true
            };
        }

        #endregion
    }
}
