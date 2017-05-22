using System;
using System.Collections.Generic;
using System.IO;
using Vanrise.Common;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using Vanrise.DataParser.Entities.HexTLV;
using Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers;
using Vanrise.DataParser.MainExtensions.HexTLV.RecordParsers;

namespace Mediation.Runtime.DataParser
{
    public class DataParserTester
    {
        public void ReadFile()
        {
            DateTimeOffset do1 = new DateTimeOffset(2017, 05, 22, 5, 0, 0, new TimeSpan(-5, 0, 0));
            var newDate = do1.ToLocalTime().DateTime;
            ParseHuaweiNamibia();

            //ParseEricsson();
        }

        #region Huawei Namibia

        private void ParseHuaweiNamibia()
        {
            var fileStream = new FileStream(@"c:\GW-MSCOLY2951.dat", FileMode.Open, FileAccess.Read);
            ReadData_Huawei(fileStream);

            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(fileStream, new Guid("e2a77834-86da-42ba-9501-c3eb81f5f60b"), (parsedBatch) =>
            {
                Vanrise.GenericData.Business.DataRecordTypeManager dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
                Type dataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(parsedBatch.RecordType);

                List<dynamic> data = new List<dynamic>();
                foreach (Vanrise.DataParser.Entities.FldDictParsedRecord item in parsedBatch.Records)
                {
                    dynamic record = Activator.CreateInstance(dataRecordRuntimeType) as dynamic;
                    record.FillDataRecordTypeFromDictionary(item.FieldValues);
                    data.Add(record);
                }
                Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(data, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                //mappedBatches.Add("StoreWhsCDRRecords", batch);


            });

        }

        private void ReadData_Huawei(FileStream fileStream)
        {
            HexTLVParserType hexParser = new HexTLVParserType
            {
                RecordParser = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_30_SubRecordsParsersByTag()
                },
                RecordParserTemplates = GetTemplates_Huawei()
            };


            Action<ParsedRecord> onRecordParsed = (parsedRecord) =>
            {

            };
            ParserTypeExecuteContext parserTypeExecuteContext = new ParserTypeExecuteContext(onRecordParsed)
            {
                Input = new StreamDataParserInput
                {
                    Stream = fileStream
                }
            };


            ParserType parserType = new ParserType
            {
                Name = "Huawei Namibia Parser",
                ParserTypeId = new Guid("e2a77834-86da-42ba-9501-c3eb81f5f60b"),
                Settings = new ParserTypeSettings
                {
                    ExtendedSettings = hexParser
                }
            };

            string settings = Serializer.Serialize(parserType.Settings);

            hexParser.Execute(parserTypeExecuteContext);
        }

        private Dictionary<Guid, HexTLVRecordParser> GetTemplates_Huawei()
        {
            Dictionary<Guid, HexTLVRecordParser> templates = new Dictionary<Guid, HexTLVRecordParser>();
            templates.Add(new Guid("9AB5792A-BB25-412C-BFCD-791515F7C893"), new HexTLVRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = GetTemplateParsers_Huawei()
                }
            });

            return templates;
        }

        private Dictionary<string, HexTLVRecordParser> GetTemplateParsers_Huawei()
        {
            Dictionary<string, HexTLVRecordParser> parsers = new Dictionary<string, HexTLVRecordParser>();

            parsers.Add("A3", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A3_FieldParsers_Huawei()
                    },
                    RecordType = "WHS_CDR",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    }
                }

            });
            parsers.Add("A4", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A4_FieldParsers_Huawei()
                    },
                    RecordType = "WHS_CDR",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    }
                }
            });
            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A3_FieldParsers_Huawei()
        {
            Dictionary<string, HexTLVFieldParser> parsers = new Dictionary<string, HexTLVFieldParser>();

            parsers.Add("80", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("81", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "CGPN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "CDPN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "RecordEntity",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("A4", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.StringParser
                                     {
                                          FieldName = "MSCIncomingRoute"
                                     }                            
                                }
                            }
                        }
                    }
                }
            });
            parsers.Add("A5", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.StringParser
                                     {
                                          FieldName = "MSCOutgoingRoute"
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("87", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.DateTimeParser
                {
                    FieldName = "ConnectDateTime",
                    DateTimeParsingType = DateTimeParsingType.DateTime,
                    WithOffset = true
                }
            });


            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.DateTimeParser
                {
                    FieldName = "DisconnectDateTime",
                    DateTimeParsingType = DateTimeParsingType.DateTime,
                    WithOffset = true

                }
            });

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "DurationInSeconds",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8B", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CauseForTermination",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AC", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                                     {
                                          FieldName = "Diagnostics"                                          
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("8D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("99", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "HLC"
                }
            });

            parsers.Add("BF8102", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"83", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                                     {
                                          FieldName = "BasicService"                                          
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("BF8105", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                                     {
                                          FieldName = "AdditionalInfoCharge"
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "ChangeAreaCode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8120", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "RoamingNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8127", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "MSCIncomingCircuit"
                }
            });

            parsers.Add("9F8128", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "MSCID"
                }
            });

            parsers.Add("9F812A", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "CallEmplPriority"
                }
            });

            parsers.Add("9F8143", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "CUGIndicatorAccess"
                }
            });

            parsers.Add("9F8146", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "MSCIncomingRouteAttribute"
                }
            });

            parsers.Add("9F8148", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "NetworkCallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("9F815A", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "DisconnectParty"
                }
            });

            parsers.Add("9F8161", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "AudioDataType"
                }
            });


            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("9F8175", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "TranslatedNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("BF8177", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                                     {
                                          FieldName = "LAC",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                                     {
                                          FieldName = "CellId",
                                           NumberType = NumberType.Int
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F8179", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "FirstMSC"
                }
            });

            parsers.Add("9F817A", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "LastMSC"
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A4_FieldParsers_Huawei()
        {
            Dictionary<string, HexTLVFieldParser> parsers = new Dictionary<string, HexTLVFieldParser>();

            parsers.Add("80", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });


            parsers.Add("81", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "CGPN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "CDPN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "RecordEntity",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("A4", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.StringParser
                                     {
                                          FieldName = "MSCIncomingRoute"
                                     }                            
                                }
                            }
                        }
                    }
                }
            });
            parsers.Add("A5", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.StringParser
                                     {
                                          FieldName = "MSCOutgoingRoute"
                                     }                            
                                }
                            }
                        }
                    }
                }
            });


            parsers.Add("87", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.DateTimeParser
                {
                    FieldName = "ConnectDateTime",
                    DateTimeParsingType = DateTimeParsingType.DateTime,
                    WithOffset = true
                }
            });

            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.DateTimeParser
                {
                    FieldName = "DisconnectDateTime",
                    DateTimeParsingType = DateTimeParsingType.DateTime,
                    WithOffset = true
                }
            });

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "DurationInSeconds",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8B", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CauseForTermination",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AC", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                                     {
                                          FieldName = "Diagnostics"                                          
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("8D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("99", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "HLC"
                }
            });

            parsers.Add("BF8102", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"83", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                                     {
                                          FieldName = "BasicService"                                          
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("BF8105", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                                     {
                                          FieldName = "AdditionalInfoCharge"                                          
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "ChangeAreaCode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8120", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "RoamingNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8127", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "MSCIncomingCircuit"
                }
            });

            parsers.Add("9F8128", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "MSCID"
                }
            });

            parsers.Add("9F812A", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "CallEmplPriority"
                }
            });

            parsers.Add("9F8143", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "CUGIndicatorAccess"
                }
            });

            parsers.Add("9F8146", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "MSCIncomingRouteAttribute"
                }
            });

            parsers.Add("9F8148", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "NetworkCallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("9F815A", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "DisconnectParty"
                }
            });

            parsers.Add("9F8161", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "AudioDataType"
                }
            });


            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.BigInt

                }
            });

            parsers.Add("9F8175", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "TranslatedNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("BF8177", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                                     {
                                          FieldName = "LAC",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                                     {
                                          FieldName = "CellId",
                                           NumberType = NumberType.Int
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F8179", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "FirstMSC"
                }
            });

            parsers.Add("9F817A", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "LastMSC"
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVRecordParser> Get_30_SubRecordsParsersByTag()
        {
            Dictionary<string, HexTLVRecordParser> subParser = new Dictionary<string, HexTLVRecordParser>();

            subParser.Add("30", new HexTLVRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_30_SplitRecordsParsersByTag()
                }
            });

            return subParser;
        }

        private Dictionary<string, HexTLVRecordParser> Get_30_SplitRecordsParsersByTag()
        {
            Dictionary<string, HexTLVRecordParser> subParser = new Dictionary<string, HexTLVRecordParser>();
            subParser.Add("A1", new HexTLVRecordParser
            {
                Settings = new ExecuteTemplateRecordParser
                {
                    RecordParserTemplateId = new Guid("9AB5792A-BB25-412C-BFCD-791515F7C893")
                }

            });
            return subParser;
        }

        #endregion

        #region Ericsson
        private void ParseEricsson()
        {
            var fileStream = new FileStream(@"c:\BGMSS2.TTFILE00.2017031916025933", FileMode.Open, FileAccess.Read);
            ReadData_Ericsson(fileStream);
        }

        public void ReadData_Ericsson(FileStream stream)
        {
            HexTLVParserType hexParser = new HexTLVParserType
            {
                RecordParser = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_A0_SubRecordsParsersByTag()
                },
                RecordParserTemplates = GetTemplates()
            };


            Action<ParsedRecord> onRecordParsed = (parsedRecord) =>
            {

            };
            ParserTypeExecuteContext parserTypeExecuteContext = new ParserTypeExecuteContext(onRecordParsed)
            {
                Input = new StreamDataParserInput
                {
                    Stream = stream
                }
            };

            hexParser.Execute(parserTypeExecuteContext);
        }

        private Dictionary<Guid, HexTLVRecordParser> GetTemplates()
        {
            Dictionary<Guid, HexTLVRecordParser> templates = new Dictionary<Guid, HexTLVRecordParser>();
            templates.Add(new Guid("8861E765-B865-43AB-AAF7-39DFB1079E04"), new HexTLVRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = GetTemplateParsers()
                }
            });

            return templates;
        }

        private Dictionary<string, HexTLVRecordParser> GetTemplateParsers()
        {
            Dictionary<string, HexTLVRecordParser> parsers = new Dictionary<string, HexTLVRecordParser>();

            parsers.Add("A0", new HexTLVRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_A0_CreateRecordsParsersByTag()
                }
            });
            return parsers;
        }

        private Dictionary<string, HexTLVRecordParser> Get_A0_SubRecordsParsersByTag()
        {
            Dictionary<string, HexTLVRecordParser> subParser = new Dictionary<string, HexTLVRecordParser>();

            subParser.Add("A0", new HexTLVRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_Template_CreateRecordsParsersByTag()
                }
            });

            subParser.Add("A1", new HexTLVRecordParser
            {
                Settings = new ExecuteTemplateRecordParser
                {
                    RecordParserTemplateId = new Guid("8861E765-B865-43AB-AAF7-39DFB1079E04")
                }
            });
            return subParser;
        }

        private Dictionary<string, HexTLVRecordParser> Get_Template_CreateRecordsParsersByTag()
        {
            Dictionary<string, HexTLVRecordParser> subParser = new Dictionary<string, HexTLVRecordParser>();

            subParser.Add("A0", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = GetFieldParsers()
                    },
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>(),
                    RecordType = "Transit"
                }
            });
            subParser.Add("A7", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A7_FieldParsers()
                    },
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>(),
                    RecordType = "MSTerminatingSMSinMSC"
                }
            });

            return subParser;
        }

        private Dictionary<string, HexTLVRecordParser> Get_A0_CreateRecordsParsersByTag()
        {
            Dictionary<string, HexTLVRecordParser> subParser = new Dictionary<string, HexTLVRecordParser>();

            subParser.Add("A0", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = GetFieldParsers()
                    },
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>(),
                    RecordType = "Transit"
                }
            });
            subParser.Add("A7", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A7_FieldParsers()
                    },
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>(),
                    RecordType = "MSTerminatingSMSinMSC"
                }
            });

            return subParser;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A7_FieldParsers()
        {
            Dictionary<string, HexTLVFieldParser> parsers = new Dictionary<string, HexTLVFieldParser>();

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "CalledPartyNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });
            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> GetFieldParsers()
        {
            Dictionary<string, HexTLVFieldParser> parsers = new Dictionary<string, HexTLVFieldParser>();

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new BoolFieldParser
                {
                    FieldName = "Bool"
                }
            });

            return parsers;
        }

        #endregion

    }

    #region Classes


    public class ParserTypeExecuteContext : IParserTypeExecuteContext
    {
        Action<ParsedRecord> _OnRecordParsed;
        public ParserTypeExecuteContext(Action<ParsedRecord> onRecordParsed)
        {
            _OnRecordParsed = onRecordParsed;
        }

        public ParsedRecord CreateRecord(string recordType)
        {
            FldDictParsedRecord parsedRecord = new FldDictParsedRecord
            {

            };
            return parsedRecord;
        }

        public IDataParserInput Input
        {
            get;
            set;
        }

        public void OnRecordParsed(ParsedRecord parsedRecord)
        {
            _OnRecordParsed(parsedRecord);
        }
    }

    public class TagValueParserExecuteContext : ITagValueParserExecuteContext
    {

        public ParsedRecord Record
        {
            get;
            set;
        }

        public byte[] TagValue
        {
            get;
            set;
        }
    }

    public class DataParserInput : IDataParserInput
    {
        public byte[] Data
        {
            get;
            set;
        }
    }

    #endregion

}
