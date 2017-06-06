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

        #region Huawei Iraq

        private string GetHuaweiParserSettings()
        {
            HexTLVParserType hexParser = new HexTLVParserType
            {
                RecordParser = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_30_SubRecordsParsersByTag_Iraq()
                },
                RecordParserTemplates = GetTemplates_Huawei_Iraq()
            };

            ParserType parserType = new ParserType
            {
                Name = "Huawei Iraq Parser",
                ParserTypeId = new Guid("3B0C2ED7-CC17-46C0-8F96-697BD185B273"),
                Settings = new ParserTypeSettings
                {
                    ExtendedSettings = hexParser
                }
            };

            return Serializer.Serialize(parserType.Settings);
        }

        private Dictionary<string, HexTLVRecordParser> Get_30_SubRecordsParsersByTag_Iraq()
        {
            Dictionary<string, HexTLVRecordParser> subParser = new Dictionary<string, HexTLVRecordParser>();

            subParser.Add("30", new HexTLVRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_30_SplitRecordsParsersByTag_Iraq()
                }
            });

            return subParser;
        }
        private Dictionary<string, HexTLVRecordParser> Get_30_SplitRecordsParsersByTag_Iraq()
        {
            Dictionary<string, HexTLVRecordParser> subParser = new Dictionary<string, HexTLVRecordParser>();
            subParser.Add("A1", new HexTLVRecordParser
            {
                Settings = new ExecuteTemplateRecordParser
                {
                    RecordParserTemplateId = new Guid("E9FD1F21-4D0A-467B-8DC9-885543BE1E76")
                }

            });
            return subParser;
        }

        private Dictionary<Guid, HexTLVRecordParser> GetTemplates_Huawei_Iraq()
        {
            Dictionary<Guid, HexTLVRecordParser> templates = new Dictionary<Guid, HexTLVRecordParser>();
            templates.Add(new Guid("E9FD1F21-4D0A-467B-8DC9-885543BE1E76"), new HexTLVRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = GetTemplateParsers_Huawei_Iraq()
                }
            });

            return templates;
        }

        private Dictionary<string, HexTLVRecordParser> GetTemplateParsers_Huawei_Iraq()
        {
            Dictionary<string, HexTLVRecordParser> parsers = new Dictionary<string, HexTLVRecordParser>();

            parsers.Add("A0", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A0_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "MobileCDR",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    }
                }

            });
            parsers.Add("A1", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A1_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "MobileCDR",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    }
                }
            });

            parsers.Add("A3", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A3_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "MobileCDR",
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
                        FieldParsersByTag = Get_A4_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "MobileCDR",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    }
                }
            });

            parsers.Add("A5", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A5_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "MobileCDR",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    }
                }
            });

            parsers.Add("BF64", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A100_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "MobileCDR",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    }
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A0_FieldParsers_Huawei_Iraq()
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
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
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
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.StringParser
                                     {
                                          FieldName = "SAC"
                                     }                            
                                }
                            },
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.StringParser
                                     {
                                          FieldName = "LocationAreaCode"
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("99", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F21", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F45", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F810E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8112", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F8149", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.DateTimeParser
                {
                    FieldName = "SetupTime",
                    WithOffset = false,
                    DateTimeParsingType = DateTimeParsingType.DateTime
                }
            });

            parsers.Add("9F814D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });


            parsers.Add("9F8170", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "ZoneCode"
                }
            });

            parsers.Add("9F8171", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8135", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8104", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "CalledIMEI",
                    RemoveHexa = true
                }
            });


            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A1_FieldParsers_Huawei_Iraq()
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
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F814D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("A9", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.StringParser
                                     {
                                          FieldName = "SAC"
                                     }                            
                                }
                            },
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.StringParser
                                     {
                                          FieldName = "LocationAreaCode"
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("96", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F36", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8112", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F814B", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.DateTimeParser
                {
                    FieldName = "SetupTime",
                    WithOffset = false,
                    DateTimeParsingType = DateTimeParsingType.DateTime
                }
            });

            parsers.Add("9F8150", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });


            parsers.Add("9F8170", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "ZoneCode"
                }
            });

            parsers.Add("9F8171", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F814E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "VoiceIndicator",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A3_FieldParsers_Huawei_Iraq()
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
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("96", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8112", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F8149", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.DateTimeParser
                {
                    FieldName = "SetupTime",
                    WithOffset = false,
                    DateTimeParsingType = DateTimeParsingType.DateTime
                }
            });

            parsers.Add("9F814D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8171", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817C", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "Origioi"
                }
            });


            parsers.Add("9F816F", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A4_FieldParsers_Huawei_Iraq()
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
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("96", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8112", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F8149", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.DateTimeParser
                {
                    FieldName = "SetupTime",
                    WithOffset = false,
                    DateTimeParsingType = DateTimeParsingType.DateTime
                }
            });

            parsers.Add("9F814D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8171", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817C", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F815B", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A5_FieldParsers_Huawei_Iraq()
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

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("8A", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8F", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("97", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F810E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });


            parsers.Add("9F8149", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.DateTimeParser
                {
                    FieldName = "SetupTime",
                    WithOffset = false,
                    DateTimeParsingType = DateTimeParsingType.DateTime
                }
            });

            parsers.Add("9F814D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8171", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817C", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F827A", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "InputCalledNumber",
                    RemoveHexa = true
                }
            });
            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A6_FieldParsers_Huawei_Iraq()
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
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            parsers.Add("A7", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.StringParser
                                     {
                                          FieldName = "SAC"
                                     }                            
                                }
                            },
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.StringParser
                                     {
                                          FieldName = "LocationAreaCode"
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F821F", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "SMSType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8160", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "TypeOfSubscribers",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F815F", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "NetworkOperatorId"
                }
            });

            parsers.Add("9F8241", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallOrigin",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "MessageReference"
                }
            });

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.DateTimeParser
                {
                    FieldName = "OriginationTime",
                    WithOffset = false,
                    DateTimeParsingType = DateTimeParsingType.DateTime
                }
            });

            parsers.Add("8C", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "DestinationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("8F", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "LocationExtension"
                }
            });

            parsers.Add("9F8135", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8163", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "UserType",
                    NumberType = NumberType.Int
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A7_FieldParsers_Huawei_Iraq()
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

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            parsers.Add("A7", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.StringParser
                                     {
                                          FieldName = "SAC"
                                     }                            
                                }
                            },
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.StringParser
                                     {
                                          FieldName = "LocationAreaCode"
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F821F", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "SMSType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8160", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "TypeOfSubscribers",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F815F", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "NetworkOperatorId"
                }
            });

            parsers.Add("9F8241", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallOrigin",
                    NumberType = NumberType.Int
                }
            });


            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.DateTimeParser
                {
                    FieldName = "DeliveryTime",
                    WithOffset = false,
                    DateTimeParsingType = DateTimeParsingType.DateTime
                }
            });

            parsers.Add("9F8127", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "OriginatingRNCorBSCId"
                }
            });

            parsers.Add("9F8128", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "OriginatingMSCId"
                }
            });

            parsers.Add("9F8149", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "Origination",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F814A", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "CallReference"
                }
            });

            parsers.Add("9F814B", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "TariffCode",
                    NumberType = NumberType.Int
                }
            });
            parsers.Add("9F816D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "ChargeLevel",
                    NumberType = NumberType.Int
                }
            });
            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A100_FieldParsers_Huawei_Iraq()
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
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
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
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.StringParser
                                     {
                                          FieldName = "SAC"
                                     }                            
                                }
                            },
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.StringParser
                                     {
                                          FieldName = "LocationAreaCode"
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("99", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F21", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8112", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F8135", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8104", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "CalledIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F8149", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.DateTimeParser
                {
                    FieldName = "SetupTime",
                    WithOffset = false,
                    DateTimeParsingType = DateTimeParsingType.DateTime
                }
            });

            parsers.Add("9F814D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });


            parsers.Add("9F8170", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "ZoneCode"
                }
            });

            parsers.Add("9F8171", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F827A", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "InputCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F810E", new HexTLVFieldParser
            {
                Settings = new Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers.BCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
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
