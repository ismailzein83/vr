using System;
using System.Collections.Generic;
using System.IO;
using Vanrise.Common;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using Vanrise.DataParser.Entities.HexTLV;
using Vanrise.DataParser.MainExtensions.CompositeFieldParsers;
using Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers;
using Vanrise.DataParser.MainExtensions.HexTLV.RecordParsers;
using Vanrise.DataParser.MainExtensions.HexTLV2.FieldParsers;

namespace Mediation.Runtime.DataParser
{
    public class DataParserTester
    {
        public void ReadFile()
        {

            string settings = GetHuaweiNamibiaParserSettings();
            settings = GetHuaweiIraqParserSettings();
            settings = GetHuaweiIraqParserSettings_GPRS();
            settings = GetEricssonIraqParserSettings();
            settings = GetNokiaParserSettings();
            //DateTimeOffset do1 = new DateTimeOffset(2017, 05, 22, 5, 0, 0, new TimeSpan(-5, 0, 0));
            //var newDate = do1.ToLocalTime().DateTime;
            //ParseHuaweiNamibia();

            //ParseEricsson();
        }

        #region Huawei Namibia

        private string GetHuaweiNamibiaParserSettings()
        {
            HexTLVParserType hexParser = new HexTLVParserType
            {
                RecordParser = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_30_SubRecordsParsersByTag()
                },
                RecordParserTemplates = GetTemplates_Huawei()
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

            return Serializer.Serialize(parserType.Settings);

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

            parsers.Add("A6", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A6_SMSMO_FieldParsers_Huawei()
                    },
                    RecordType = "SMS",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    }
                }
            });

            parsers.Add("A7", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A7_SMSMT_FieldParsers_Huawei()
                    },
                    RecordType = "SMS",
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
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("81", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CGPN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CDPN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RecordEntity",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("A4", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new StringParser
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
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new StringParser
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
                Settings = GetHuaweiDateTimeParser("ConnectDateTime")
            });


            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("DisconnectDateTime")
            });

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "DurationInSeconds",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8B", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForTermination",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AC", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new HexaParser
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
                Settings = new NumberFieldParser
                {
                    FieldName = "CallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("99", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "HLC"
                }
            });

            parsers.Add("BF8102", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"83", new HexTLVFieldParser
                                {
                                     Settings = new HexaParser
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
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new HexaParser
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
                Settings = new NumberFieldParser
                {
                    FieldName = "ChangeAreaCode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8120", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RoamingNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8127", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "MSCIncomingCircuit"
                }
            });

            parsers.Add("9F8128", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "MSCID"
                }
            });

            parsers.Add("9F812A", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CallEmplPriority"
                }
            });

            parsers.Add("9F8143", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CUGIndicatorAccess"
                }
            });

            parsers.Add("9F8146", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "MSCIncomingRouteAttribute"
                }
            });

            parsers.Add("9F8148", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "NetworkCallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("9F815A", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "DisconnectParty"
                }
            });

            parsers.Add("9F8161", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "AudioDataType"
                }
            });


            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("9F8175", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "TranslatedNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("BF8177", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "LAC",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
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
                Settings = new HexaParser
                {
                    FieldName = "FirstMSC"
                }
            });

            parsers.Add("9F817A", new HexTLVFieldParser
            {
                Settings = new HexaParser
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
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });


            parsers.Add("81", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CGPN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CDPN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RecordEntity",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("A4", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new StringParser
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
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new StringParser
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
                Settings = GetHuaweiDateTimeParser("ConnectDateTime")
            });

            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("DisconnectDateTime")
            });

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "DurationInSeconds",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8B", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForTermination",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AC", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new HexaParser
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
                Settings = new NumberFieldParser
                {
                    FieldName = "CallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("99", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "HLC"
                }
            });

            parsers.Add("BF8102", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"83", new HexTLVFieldParser
                                {
                                     Settings = new HexaParser
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
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new HexaParser
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
                Settings = new NumberFieldParser
                {
                    FieldName = "ChangeAreaCode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8120", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RoamingNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8127", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "MSCIncomingCircuit"
                }
            });

            parsers.Add("9F8128", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "MSCID"
                }
            });

            parsers.Add("9F812A", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CallEmplPriority"
                }
            });

            parsers.Add("9F8143", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CUGIndicatorAccess"
                }
            });

            parsers.Add("9F8146", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "MSCIncomingRouteAttribute"
                }
            });

            parsers.Add("9F8148", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "NetworkCallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("9F815A", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "DisconnectParty"
                }
            });

            parsers.Add("9F8161", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "AudioDataType"
                }
            });


            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.BigInt

                }
            });

            parsers.Add("9F8175", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "TranslatedNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("BF8177", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "LAC",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
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
                Settings = new HexaParser
                {
                    FieldName = "FirstMSC"
                }
            });

            parsers.Add("9F817A", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "LastMSC"
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A6_SMSMO_FieldParsers_Huawei()
        {
            Dictionary<string, HexTLVFieldParser> parsers = new Dictionary<string, HexTLVFieldParser>();

            parsers.Add("80", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("81", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "MSClassmark",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "ServiceCenter",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new HexTLVFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "RecordingEntity",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("A7", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "SAC",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "LocationAreaCode",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "MessageReference",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("MessageTime")
            });

            parsers.Add("AA", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SMSResult",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AB", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "RecordExtensions"
                }
            });

            parsers.Add("8C", new HexTLVFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "DestinationNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("AD", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new BCDNumberParser
                                     {
                                          FieldName = "GsmSCFAddress",
                                          AIsZero = false,
                                          RemoveHexa = true
                                     }                            
                                }
                            },
                            {"82", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "ServiceKey",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"83", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "DefaultSMSHandling",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"84", new HexTLVFieldParser
                                {
                                     Settings = new HexaParser
                                     {
                                          FieldName = "FreeFormatData"
                                     }                            
                                }
                            },
                            {"85", new HexTLVFieldParser
                                {
                                     Settings = new BCDNumberParser
                                     {
                                          FieldName = "CAMELSMSCAddress",
                                          AIsZero = false,
                                          RemoveHexa = true
                                     }                            
                                }
                            }
                            
                        }
                    }
                }
            });

            parsers.Add("8E", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SystemType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8F", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "LocationExtension"
                }
            });

            parsers.Add("8F8102", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "BasicService",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("BF8105", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "ChargeIndicator",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new HexaParser
                                     {
                                          FieldName = "ChargeParameters"
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F810C", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ClassMark"
                }
            });

            parsers.Add("9F810D", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargedParty",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8127", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "OriginatingRNCorBSCId"
                }
            });

            parsers.Add("9F8128", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "OriginatingMSCId"
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F813E", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "SubscriberCategory"
                }
            });

            parsers.Add("9F8140", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "FirstMCC_MNC"
                }
            });

            parsers.Add("9F8143", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "SMSUserDataType"
                }
            });

            parsers.Add("9F8144", new HexTLVFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "SMSText"
                }
            });

            parsers.Add("9F8145", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MaxNumberOfSMSInConcatenatedSMS",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8146", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ConcatenatedSMSReferenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8147", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumberOfCurrentSMS",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8148", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "HotBillingTag",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8149", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CallReference"
                }
            });

            parsers.Add("9F814A", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TariffCode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F815F", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "NetworkOperatorId"
                }
            });

            parsers.Add("9F8160", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TypeOfSubscribers",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8163", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "UserType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816B", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "OSS_ServicesUsed"
                }
            });

            parsers.Add("9F816D", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargeLevel",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8170", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ZoneCode"
                }
            });

            parsers.Add("9F8201", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "RoutingCategory"
                }
            });

            parsers.Add("9F8116", new HexTLVFieldParser
            {
                Settings = new BoolFieldParser
                {
                    FieldName = "SMMODirect"
                }
            });

            parsers.Add("9F821D", new HexTLVFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OfficeName"
                }
            });

            parsers.Add("9F821E", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F821F", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SMSType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8221", new HexTLVFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "SMMOCommandType"
                }
            });

            parsers.Add("9F8222", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SwitchMode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F823D", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "AdditionalRoutingCategory",
                    NumberType = NumberType.Int
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A7_SMSMT_FieldParsers_Huawei()
        {
            Dictionary<string, HexTLVFieldParser> parsers = new Dictionary<string, HexTLVFieldParser>();

            parsers.Add("80", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });


            parsers.Add("81", new HexTLVFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "ServiceCenter",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "MSClassmark",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new HexTLVFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "RecordingEntity",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("A7", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "SAC",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "LocationAreaCode",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("MessageTime")
            });

            parsers.Add("A9", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SMSResult",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AA", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "RecordExtensions"
                }
            });

            parsers.Add("8B", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SystemType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8D", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "LocationExtension"
                }
            });

            parsers.Add("AC", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new BCDNumberParser
                                     {
                                          FieldName = "GsmSCFAddress",
                                          AIsZero = false,
                                          RemoveHexa = true
                                     }                            
                                }
                            },
                            {"82", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "ServiceKey",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"83", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "DefaultSMSHandling",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"84", new HexTLVFieldParser
                                {
                                     Settings = new HexaParser
                                     {
                                          FieldName = "FreeFormatData"
                                     }                            
                                }
                            },
                            {"85", new HexTLVFieldParser
                                {
                                     Settings = new BCDNumberParser
                                     {
                                          FieldName = "CAMELSMSCAddress",
                                          AIsZero = false,
                                          RemoveHexa = true
                                     }                            
                                }
                            }
                            
                        }
                    }
                }
            });

            parsers.Add("BF8102", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "BasicService",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("BF8105", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "ChargeIndicator",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new HexaParser
                                     {
                                          FieldName = "ChargeParameters"
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F810C", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ClassMark"
                }
            });

            parsers.Add("9F810D", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargedParty",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8127", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "OriginatingRNCorBSCId"
                }
            });

            parsers.Add("9F8128", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "OriginatingMSCId"
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F813E", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "SubscriberCategory"
                }
            });

            parsers.Add("9F8140", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "FirstMCC_MNC"
                }
            });

            parsers.Add("9F8143", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "SMSUserDataType"
                }
            });

            parsers.Add("9F8144", new HexTLVFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "SMSText"
                }
            });

            parsers.Add("9F8145", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MaxNumberOfSMSInConcatenatedSMS",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8146", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ConcatenatedSMSReferenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8147", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumberOfCurrentSMS",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8148", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "HotBillingTag",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8149", new HexTLVFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "Origination",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("9F814A", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CallReference"
                }
            });

            parsers.Add("9F814B", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TariffCode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F815F", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "NetworkOperatorId"
                }
            });

            parsers.Add("9F8160", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TypeOfSubscribers",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816D", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargeLevel",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8170", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ZoneCode"
                }
            });

            parsers.Add("9F8201", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "RoutingCategory"
                }
            });

            parsers.Add("9F821D", new HexTLVFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OfficeName"
                }
            });

            parsers.Add("9F821E", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F821F", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SMSType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8222", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SwitchMode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F823D", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "AdditionalRoutingCategory",
                    NumberType = NumberType.Int
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

        public string GetEricssonIraqParserSettings()
        {
            HexTLVParserType hexParser = new HexTLVParserType
            {
                RecordParser = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_A0_SubRecordsParsersByTag()
                },
                RecordParserTemplates = GetTemplates()
            };

            ParserType parserType = new ParserType
            {
                ParserTypeId = new Guid("BA810002-0B4D-4563-9A0D-EE228D69A1A6"),
                Settings = new ParserTypeSettings
                {
                    ExtendedSettings = hexParser
                }
            };

            return Serializer.Serialize(parserType.Settings);
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
                    SubRecordsParsersByTag = Get_Template_CreateRecordsParsersByTag()
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
                        FieldParsersByTag = Get_A0_Transit_FieldParsers()
                    },
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> 
                    { 
                        new ParsedRecordFieldConstantValue
                            {
                             FieldName = "RecordType",
                             Value = 5
                            },
                            new ParsedRecordFieldConstantValue
                            {
                             FieldName = "SwitchId",
                             Value = 2
                            }
                    },
                    RecordType = "MobileCDR",
                    TempFieldsNames = GetTempFieldsName(),
                    CompositeFieldsParsers = GetCompositeFieldParsers()
                }
            });

            subParser.Add("A1", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A1_MSOriginating_FieldParsers()
                    },
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    { 
                        new ParsedRecordFieldConstantValue
                            {
                             FieldName = "RecordType",
                             Value = 0
                            },
                            new ParsedRecordFieldConstantValue
                            {
                             FieldName = "SwitchId",
                             Value = 2
                            }
                    },
                    RecordType = "MobileCDR",
                    TempFieldsNames = GetTempFieldsName(),
                    CompositeFieldsParsers = GetCompositeFieldParsers()
                }
            });

            subParser.Add("A2", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A2_Roaming_FieldParsers()
                    },
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    { 
                        new ParsedRecordFieldConstantValue
                            {
                             FieldName = "RecordType",
                             Value = 101
                            },
                            new ParsedRecordFieldConstantValue
                            {
                             FieldName = "SwitchId",
                             Value = 2
                            }
                    },
                    RecordType = "MobileCDR",
                    TempFieldsNames = GetTempFieldsName(),
                    CompositeFieldsParsers = GetCompositeFieldParsers()
                }
            });

            subParser.Add("A3", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A3_CallForwarding_FieldParsers()
                    },
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    { 
                        new ParsedRecordFieldConstantValue
                            {
                             FieldName = "RecordType",
                             Value = 100
                            },
                            new ParsedRecordFieldConstantValue
                            {
                             FieldName = "SwitchId",
                             Value = 2
                            }
                    },
                    RecordType = "MobileCDR",
                    TempFieldsNames = GetTempFieldsName(),
                    CompositeFieldsParsers = GetCompositeFieldParsers()
                }
            });

            subParser.Add("A4", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A4_MSTerminating_FieldParsers()
                    },
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    { 
                        new ParsedRecordFieldConstantValue
                            {
                             FieldName = "RecordType",
                             Value = 1
                            },
                            new ParsedRecordFieldConstantValue
                            {
                             FieldName = "SwitchId",
                             Value = 2
                            }
                    },
                    RecordType = "MobileCDR",
                    TempFieldsNames = GetTempFieldsName(),
                    CompositeFieldsParsers = GetCompositeFieldParsers()
                }
            });

            subParser.Add("A5", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A5_MSOriginatingSMS_FieldParsers()
                    },
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    { 
                        new ParsedRecordFieldConstantValue
                            {
                             FieldName = "RecordType",
                             Value = 6
                            },
                            new ParsedRecordFieldConstantValue
                            {
                             FieldName = "SwitchId",
                             Value = 2
                            }
                    },
                    RecordType = "SMS"
                }
            });

            subParser.Add("A7", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A7_MSTerminatingSMS_FieldParsers()
                    },
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    { 
                        new ParsedRecordFieldConstantValue
                            {
                             FieldName = "RecordType",
                             Value = 7
                            },
                            new ParsedRecordFieldConstantValue
                            {
                             FieldName = "SwitchId",
                             Value = 2
                            }
                    },
                    RecordType = "SMS"
                }
            });
            return subParser;
        }

        private HashSet<string> GetTempFieldsName()
        {
            return new HashSet<string> { "Date", "StartTime", "StopTime" };
        }

        private List<CompositeFieldsParser> GetCompositeFieldParsers()
        {
            return new List<CompositeFieldsParser> 
                    { 
                        new DateTimeCompositeParser{DateFieldName = "Date",TimeFieldName = "StartTime",FieldName = "ConnectDateTime"},
                        new DateTimeCompositeParser{DateFieldName = "Date",TimeFieldName = "StopTime",FieldName = "DisconnectDateTime"}
                    };
        }

        private Dictionary<string, HexTLVFieldParser> Get_A0_Transit_FieldParsers()
        {
            Dictionary<string, HexTLVFieldParser> parsers = new Dictionary<string, HexTLVFieldParser>();

            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "Date",
                    DateTimeParsingType = DateTimeParsingType.Date,
                    WithOffset = false,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2
                }
            });

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StartTime"
                }
            });

            parsers.Add("8A", new HexTLVFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StopTime"
                }
            });

            parsers.Add("8B", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9D", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F50", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F54", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledChargeAreaCode",
                    RemoveHexa = true
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A1_MSOriginating_FieldParsers()
        {
            Dictionary<string, HexTLVFieldParser> parsers = new Dictionary<string, HexTLVFieldParser>();

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "Date",
                    DateTimeParsingType = DateTimeParsingType.Date,
                    WithOffset = false,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2
                }
            });
            parsers.Add("8A", new HexTLVFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StartTime"
                }
            });

            parsers.Add("8B", new HexTLVFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StopTime"
                }
            });

            parsers.Add("8C", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("87", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });


            parsers.Add("9F7E", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8102", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9B", new HexTLVFieldParser
            {
                Settings = new CallLocationInformationParser
                {
                    FieldName = "CallingLocationInformation_First"
                }
            });

            parsers.Add("9C", new HexTLVFieldParser
            {
                Settings = new CallLocationInformationParser
                {
                    FieldName = "CalledLocationInformation_First"
                }
            });

            parsers.Add("99", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A2_Roaming_FieldParsers()
        {
            Dictionary<string, HexTLVFieldParser> parsers = new Dictionary<string, HexTLVFieldParser>();

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "Date",
                    DateTimeParsingType = DateTimeParsingType.Date,
                    WithOffset = false,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2
                }
            });
            parsers.Add("8A", new HexTLVFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StartTime"
                }
            });

            parsers.Add("8B", new HexTLVFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StopTime"
                }
            });
            parsers.Add("8C", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9C", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F3A", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F3D", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A3_CallForwarding_FieldParsers()
        {
            Dictionary<string, HexTLVFieldParser> parsers = new Dictionary<string, HexTLVFieldParser>();

            parsers.Add("8D", new HexTLVFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "Date",
                    DateTimeParsingType = DateTimeParsingType.Date,
                    WithOffset = false,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2
                }
            });

            parsers.Add("8E", new HexTLVFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StartTime"
                }
            });

            parsers.Add("8F", new HexTLVFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StopTime"
                }
            });
            parsers.Add("90", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F53", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F56", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9C", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A4_MSTerminating_FieldParsers()
        {
            Dictionary<string, HexTLVFieldParser> parsers = new Dictionary<string, HexTLVFieldParser>();

            parsers.Add("8A", new HexTLVFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "Date",
                    DateTimeParsingType = DateTimeParsingType.Date,
                    WithOffset = false,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2
                }
            });

            parsers.Add("8B", new HexTLVFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StartTime"
                }
            });

            parsers.Add("8C", new HexTLVFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StopTime"
                }
            });

            parsers.Add("8D", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("86", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("87", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });


            parsers.Add("9F70", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F72", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F24", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9B", new HexTLVFieldParser
            {
                Settings = new CallLocationInformationParser
                {
                    FieldName = "CallingLocationInformation_First"
                }
            });

            parsers.Add("9C", new HexTLVFieldParser
            {
                Settings = new CallLocationInformationParser
                {
                    FieldName = "CalledLocationInformation_First"
                }
            });

            parsers.Add("99", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A5_MSOriginatingSMS_FieldParsers()
        {
            Dictionary<string, HexTLVFieldParser> parsers = new Dictionary<string, HexTLVFieldParser>();

            parsers.Add("87", new HexTLVFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "MessageTime",
                    DateTimeParsingType = DateTimeParsingType.Date,
                    WithOffset = false,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2
                }
            });

            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "MessageTime",
                    DateTimeParsingType = DateTimeParsingType.Time,
                    WithOffset = false
                }
            });


            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            return parsers;
        }

        private Dictionary<string, HexTLVFieldParser> Get_A7_MSTerminatingSMS_FieldParsers()
        {
            Dictionary<string, HexTLVFieldParser> parsers = new Dictionary<string, HexTLVFieldParser>();

            parsers.Add("86", new HexTLVFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "MessageTime",
                    DateTimeParsingType = DateTimeParsingType.Date,
                    WithOffset = false,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2
                }
            });

            parsers.Add("87", new HexTLVFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "MessageTime",
                    DateTimeParsingType = DateTimeParsingType.Time,
                    WithOffset = false
                }
            });


            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            //parsers.Add("9F53", new HexTLVFieldParser
            //{
            //    Settings = new TBCDNumberParser
            //    {
            //        FieldName = "CallingChargeAreaCode",
            //        RemoveHexa = true
            //    }
            //});
            return parsers;
        }

        #endregion

        #region Huawei Iraq
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
                        TimeShiftIndicatorIndex = 6,
                        SecondsIndex = 5,
                        HoursTimeShiftIndex = 7,
                        MinutesTimeShiftIndex = 8
                    };
        }
        private string GetHuaweiIraqParserSettings()
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

            parsers.Add("A6", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A6_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "SMS",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    }
                }

            });

            parsers.Add("A7", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_A7_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "SMS",
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
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("81", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("AC", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "SAC",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "LocationAreaCode",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("99", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F21", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F45", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F810E", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8112", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("97", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("ConnectDateTime")
            });

            parsers.Add("98", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("DisconnectDateTime")
            });

            //parsers.Add("97", new HexTLVFieldParser
            //{
            //    Settings = new DateTimeParser
            //    {
            //        FieldName = "SetupTime",
            //        WithOffset = true,
            //        DateTimeParsingType = DateTimeParsingType.DateTime
            //    }
            //});

            parsers.Add("9F814D", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });


            parsers.Add("9F8170", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ZoneCode"
                }
            });

            parsers.Add("9F8171", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817E", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8135", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8104", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
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
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("81", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F814D", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("A9", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "SAC",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "LocationAreaCode",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("96", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9E", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F36", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "ChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8112", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });


            parsers.Add("94", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("ConnectDateTime")
            });

            parsers.Add("95", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("DisconnectDateTime")
            });

            //parsers.Add("9F814B", new HexTLVFieldParser
            //{
            //    Settings = new DateTimeParser
            //    {
            //        FieldName = "SetupTime",
            //        WithOffset = false,
            //        DateTimeParsingType = DateTimeParsingType.DateTime
            //    }
            //});

            parsers.Add("9F8150", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });


            parsers.Add("9F8170", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ZoneCode"
                }
            });

            parsers.Add("9F8171", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817E", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F814E", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "VoiceIndicator",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
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
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("81", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8E", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("96", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8112", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("87", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("ConnectDateTime")
            });

            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("DisconnectDateTime")
            });

            //parsers.Add("9F8149", new HexTLVFieldParser
            //{
            //    Settings = new DateTimeParser
            //    {
            //        FieldName = "SetupTime",
            //        WithOffset = false,
            //        DateTimeParsingType = DateTimeParsingType.DateTime
            //    }
            //});

            parsers.Add("9F814D", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8171", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817C", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Origioi"
                }
            });


            parsers.Add("9F816F", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
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
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("81", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8E", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("96", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8112", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("87", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("ConnectDateTime")
            });

            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("DisconnectDateTime")
            });

            //parsers.Add("9F8149", new HexTLVFieldParser
            //{
            //    Settings = new DateTimeParser
            //    {
            //        FieldName = "SetupTime",
            //        WithOffset = false,
            //        DateTimeParsingType = DateTimeParsingType.DateTime
            //    }
            //});

            parsers.Add("9F814D", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8171", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817C", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F815B", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
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
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("8A", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8F", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("97", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F810E", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("ConnectDateTime")
            });

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("ConnectDateTime")
            });

            //parsers.Add("9F8149", new HexTLVFieldParser
            //{
            //    Settings = new DateTimeParser
            //    {
            //        FieldName = "SetupTime",
            //        WithOffset = false,
            //        DateTimeParsingType = DateTimeParsingType.DateTime
            //    }
            //});

            parsers.Add("9F814D", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8171", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817C", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F827A", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
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
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("81", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            parsers.Add("A7", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "SAC",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "LocationAreaCode",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F821F", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SMSType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8160", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TypeOfSubscribers",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F815F", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "NetworkOperatorId"
                }
            });

            parsers.Add("9F8241", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallOrigin",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "MessageReference"
                }
            });

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("MessageTime")
            });

            parsers.Add("8C", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "DestinationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("8F", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "LocationExtension"
                }
            });

            parsers.Add("9F8135", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8163", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
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
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            parsers.Add("A7", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "SAC",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "LocationAreaCode",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F821F", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SMSType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8160", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TypeOfSubscribers",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F815F", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "NetworkOperatorId"
                }
            });

            parsers.Add("9F8241", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallOrigin",
                    NumberType = NumberType.Int
                }
            });


            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("MessageTime")
            });

            parsers.Add("9F8127", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "OriginatingRNCorBSCId"
                }
            });

            parsers.Add("9F8128", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "OriginatingMSCId"
                }
            });

            parsers.Add("9F8149", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "Origination",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F814A", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CallReference"
                }
            });

            parsers.Add("9F814B", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TariffCode",
                    NumberType = NumberType.Int
                }
            });
            parsers.Add("9F816D", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
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
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("81", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });



            parsers.Add("AC", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new StringParser
                                     {
                                          FieldName = "SAC"
                                     }                            
                                }
                            },
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new StringParser
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
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F21", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8112", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F8135", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8104", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F813C", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("97", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("ConnectDateTime")
            });

            parsers.Add("98", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("DisconnectDateTime")
            });

            //parsers.Add("9F8149", new HexTLVFieldParser
            //{
            //    Settings = new DateTimeParser
            //    {
            //        FieldName = "SetupTime",
            //        WithOffset = false,
            //        DateTimeParsingType = DateTimeParsingType.DateTime
            //    }
            //});

            parsers.Add("9F814D", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });


            parsers.Add("9F8170", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ZoneCode"
                }
            });

            parsers.Add("9F8171", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817E", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F827A", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "InputCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F810E", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            return parsers;
        }
        private string GetHuaweiIraqParserSettings_GPRS()
        {
            HexTLVParserType hexParser = new HexTLVParserType
            {
                RecordParser = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = GetTemplateParsers_Huawei_Iraq_GPRS()
                }
            };

            ParserType parserType = new ParserType
            {
                Name = "Huawei Iraq GPRS Parser",
                ParserTypeId = new Guid("16B6AF8D-6A15-46A1-9C19-CCFAC1EBBDDE"),
                Settings = new ParserTypeSettings
                {
                    ExtendedSettings = hexParser
                }
            };

            return Serializer.Serialize(parserType.Settings);
        }
        private Dictionary<string, HexTLVRecordParser> GetTemplateParsers_Huawei_Iraq_GPRS()
        {
            Dictionary<string, HexTLVRecordParser> parsers = new Dictionary<string, HexTLVRecordParser>();

            parsers.Add("B4", new HexTLVRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = Get_B4_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "GPRS",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    }
                }

            });

            return parsers;
        }
        private Dictionary<string, HexTLVFieldParser> Get_B4_FieldParsers_Huawei_Iraq()
        {
            Dictionary<string, HexTLVFieldParser> parsers = new Dictionary<string, HexTLVFieldParser>();

            parsers.Add("80", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("81", new HexTLVFieldParser
            {
                Settings = new BoolFieldParser
                {
                    FieldName = "NetworkInitiation"
                }
            });

            parsers.Add("83", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("A5", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "SGSN_Address"                                          
                                     }                            
                                }
                            },
                            {"82", new HexTLVFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "SGSN_Address"                                          
                                     }                            
                                }
                            },
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "SGSN_Address"                                          
                                     }                            
                                }
                            },
                            {"83", new HexTLVFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "SGSN_Address"                                          
                                     }                            
                                }
                            }

                        }
                    }
                }
            });

            parsers.Add("86", new HexTLVFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "MSNetworkCapability",
                    RemoveHexa = true
                }
            });

            parsers.Add("87", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RoutingArea",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("88", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocationAreaCode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("89", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CellIdentifier",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8A", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CellIdentifier",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AB", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"80", new HexTLVFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "GGSN_Address"                                          
                                     }                            
                                }
                            },
                            {"82", new HexTLVFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "GGSN_Address"                                          
                                     }                            
                                }
                            },
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "GGSN_Address"                                          
                                     }                            
                                }
                            },
                            {"83", new HexTLVFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "GGSN_Address"                                          
                                     }                            
                                }
                            }

                        }
                    }
                }
            });

            parsers.Add("8C", new HexTLVFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "AccessPointNameNI"
                }
            });

            parsers.Add("8D", new HexTLVFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "PDPType"
                }
            });

            parsers.Add("90", new HexTLVFieldParser
            {
                Settings = GetHuaweiDateTimeParser("RecordOpeningTime")
            });

            parsers.Add("91", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "Duration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("92", new HexTLVFieldParser
            {
                Settings = new BoolFieldParser
                {
                    FieldName = "SGSN_Change"
                }
            });

            parsers.Add("93", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForRecClosing",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("B4", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"84", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "Diagnostics",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("95", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordSequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("96", new HexTLVFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "NodeID"
                }
            });

            parsers.Add("98", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocalSequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("99", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "APNSelectionMode",
                    NumberType = NumberType.Int
                }
            });


            parsers.Add("9A", new HexTLVFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "AccessPointNameOI"
                }
            });

            parsers.Add("9B", new HexTLVFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            parsers.Add("9C", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargingCharacteristics",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9D", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SystemType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F1F", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RNC_UnsentDownlinkVolume",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F20", new HexTLVFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChSelectionMode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F21", new HexTLVFieldParser
            {
                Settings = new BoolFieldParser
                {
                    FieldName = "DynamicAddressFlag"
                }
            });

            parsers.Add("AF", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"30", new HexTLVFieldParser
                                {
                                      Settings = new SequenceFieldParser
                                        {
                                            FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                                            {
                                                FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                                                {
                                                    {"81", new HexTLVFieldParser
                                                        {
                                                             Settings = new HexaParser
                                                             {
                                                                  FieldName = "QosRequested"
                                                             }                            
                                                        }
                                                    },
                                                    {"82", new HexTLVFieldParser
                                                        {
                                                             Settings = new HexaParser
                                                             {
                                                                  FieldName = "QosNegotiated"
                                                             }                            
                                                        }
                                                    },
                                                     {"83", new HexTLVFieldParser
                                                        {
                                                             Settings = new NumberFieldParser
                                                             {
                                                                  FieldName = "DataVolumeGPRSUplink",
                                                                  NumberType = NumberType.Int
                                                             }                            
                                                        }
                                                    },
                                                    {"84", new HexTLVFieldParser
                                                        {
                                                             Settings = new NumberFieldParser
                                                             {
                                                                  FieldName = "DataVolumeGPRSDownlink",
                                                                  NumberType = NumberType.Int
                                                             }                            
                                                        }
                                                    },
                                                    {"85", new HexTLVFieldParser
                                                        {
                                                             Settings = new NumberFieldParser
                                                             {
                                                                  FieldName = "ChangeCondition",
                                                                  NumberType = NumberType.Int
                                                             }                            
                                                        }
                                                    },
                                                    {"86", new HexTLVFieldParser
                                                        {
                                                             Settings = new DateTimeParser
                                                             {
                                                                  FieldName = "ChangeTime",
                                                                  DateTimeParsingType = DateTimeParsingType.DateTime,
                                                                  WithOffset = true
                                                             }                            
                                                        }
                                                    }
                                                }
                                            }
                                        }                           
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("BE", new HexTLVFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.HexTLVFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, HexTLVFieldParser>
                        {
                            {"81", new HexTLVFieldParser
                                {
                                     Settings = new HexaParser
                                     {
                                          FieldName = "SCFAddress"
                                     }                            
                                }
                            },
                            {"82", new HexTLVFieldParser
                                {
                                     Settings = new HexaParser
                                     {
                                          FieldName = "ServiceKey"
                                     }                            
                                }
                            },
                            {"83", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "DefaultTransactionHandling",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"84", new HexTLVFieldParser
                                {
                                     Settings = new StringParser
                                     {
                                          FieldName = "CamelAccessPointNameNI"
                                     }                            
                                }
                            },
                            {"85", new HexTLVFieldParser
                                {
                                     Settings = new StringParser
                                     {
                                          FieldName = "CamelAccessPointNameOI"
                                     }                            
                                }
                            },
                            {"86", new HexTLVFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "NumberOfDPEncountered",
                                          NumberType = NumberType.Int
                                     }                            
                                }
                            },
                            {"87", new HexTLVFieldParser
                                {
                                     Settings = new StringParser
                                     {
                                          FieldName = "LevelOfCamelService"
                                     }                            
                                }
                            },
                            {"88", new HexTLVFieldParser
                                {
                                     Settings = new HexaParser
                                     {
                                          FieldName = "FreeFormatData"
                                     }                            
                                }
                            },
                            {"89", new HexTLVFieldParser
                                {
                                     Settings = new BoolFieldParser
                                     {
                                          FieldName = "FFDAppendIndicator"
                                     }                            
                                }
                            }

                        }
                    }
                }
            });

            return parsers;
        }

        #endregion

        #region Nokia

        public string GetNokiaParserSettings()
        {
            HexTLVParserType hexParser = new HexTLVParserType
            {
                RecordParser = new SplitByBlockRecordParser
                {
                    RecordParser = new HexTLVRecordParser
                    {
                        Settings = new SplitByOffSetRecordParser
                        {
                            LengthNbOfBytes = 2,
                            RecordParser = new HexTLVRecordParser
                            {
                                Settings = new SplitByPositionedTypeRecordParser
                                {
                                    RecordTypeLength = 1,
                                    RecordTypePosition = 2,
                                    SubRecordsParsersByRecordType = GetNokiaSubRecordsParsers()
                                }
                            },
                            ReverseLengthBytes = true
                        }
                    },
                    BlockSize = 8176,
                    DataLengthBytesLength = 2,
                    DataLengthIndex = 6,
                    LengthNbOfBytes = 2,
                    ReverseLengthBytes = true
                }
            };

            ParserType parserType = new ParserType
            {
                ParserTypeId = new Guid("230BEDB5-A3EE-4CBE-802C-DFDAA2A2D438"),
                Settings = new ParserTypeSettings
                {
                    ExtendedSettings = hexParser
                }
            };

            return Serializer.Serialize(parserType.Settings);
        }
        private Dictionary<string, HexTLVRecordParser> GetNokiaSubRecordsParsers()
        {
            Dictionary<string, HexTLVRecordParser> recordParsers = new Dictionary<string, HexTLVRecordParser>();


            recordParsers.Add("1", new HexTLVRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_MOC_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = 0
                     },
                    new ParsedRecordFieldConstantValue
                    {
                        FieldName = "SwitchId",
                        Value = 3
                    }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),

                    RecordType = "MobileCDR"

                }
            });

            recordParsers.Add("2", new HexTLVRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_MOT_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = 1
                     },
                    new ParsedRecordFieldConstantValue
                    {
                        FieldName = "SwitchId",
                        Value = 3
                    }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
                    RecordType = "MobileCDR"

                }
            });


            recordParsers.Add("3", new HexTLVRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_Forward_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = 100
                     },
                    new ParsedRecordFieldConstantValue
                    {
                        FieldName = "SwitchId",
                        Value = 3
                    }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
                    RecordType = "MobileCDR"

                }
            });

            recordParsers.Add("4", new HexTLVRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_Roaming_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = 101
                     },
                    new ParsedRecordFieldConstantValue
                    {
                        FieldName = "SwitchId",
                        Value = 3
                    }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
                    RecordType = "MobileCDR"

                }
            });

            recordParsers.Add("17", new HexTLVRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_PSTN_Originated_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = 17
                     },
                    new ParsedRecordFieldConstantValue
                    {
                        FieldName = "SwitchId",
                        Value = 3
                    }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
                    RecordType = "MobileCDR"

                }
            });

            recordParsers.Add("18", new HexTLVRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_PSTN_Terminated_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = 18
                     },
                    new ParsedRecordFieldConstantValue
                    {
                        FieldName = "SwitchId",
                        Value = 3
                    }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
                    RecordType = "MobileCDR"

                }
            });

            recordParsers.Add("19", new HexTLVRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_PBX_Originated_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = 19
                     },
                    new ParsedRecordFieldConstantValue
                    {
                        FieldName = "SwitchId",
                        Value = 3
                    }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
                    RecordType = "MobileCDR"

                }
            });


            recordParsers.Add("20", new HexTLVRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_PBX_Terminated_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = 21
                     },
                    new ParsedRecordFieldConstantValue
                    {
                        FieldName = "SwitchId",
                        Value = 3
                    }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
                    RecordType = "MobileCDR"

                }
            });

            recordParsers.Add("8", new HexTLVRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_SMMO(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = 6
                     },
                    new ParsedRecordFieldConstantValue
                    {
                        FieldName = "SwitchId",
                        Value = 3
                    }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_SMS_Nokia(),
                    RecordType = "SMS"

                }
            });
            recordParsers.Add("9", new HexTLVRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_SMMT(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = 7
                     },
                    new ParsedRecordFieldConstantValue
                    {
                        FieldName = "SwitchId",
                        Value = 3
                    }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_SMS_Nokia(),
                    RecordType = "SMS"

                }
            });

            return recordParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_MOC_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingNumber"
                    }
                },
                Length = 10,
                Position = 44

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledNumber"
                    }
                },
                Length = 12,
                Position = 73
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledIMEI"
                    }
                },
                Length = 8,
                Position = 64
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingIMEI"
                    }
                },
                Length = 8,
                Position = 36
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledIMSI"
                    }
                },
                Length = 8,
                Position = 56
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingIMSI"
                    }
                },
                Length = 8,
                Position = 28
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "SetupTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 204
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "ConnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 136
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "DisconnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 143
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "DurationInSeconds"
                    }
                },
                Length = 3,
                Position = 156
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "LastExchangeISDN"
                    }
                },
                Length = 10,
                Position = 103
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingLocationInformation_First"
                    }
                },
                Length = 2,
                Position = 99
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingLocationInformation_Last"
                    }
                },
                Length = 2,
                Position = 113
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "FirstSiteId"
                    }
                },
                Length = 2,
                Position = 101
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "LastSiteId"
                    }
                },
                Length = 2,
                Position = 115
            });


            return fieldParsers;
        }

        private List<CompositeFieldsParser> GetCompositeFieldParsers_Nokia()
        {
            return new List<CompositeFieldsParser> 
                    { 
                    new TimestampDateTimeCompositeParser
                    {
                        FieldName = "ConnectTimestamp",
                        DateTimeFieldName = "ConnectDateTime",
                        DateTimeShift = new DateTime(1970, 01, 01)
                    },
                    new TimestampDateTimeCompositeParser
                    {
                        FieldName = "DisconnectTimestamp",
                        DateTimeFieldName = "DisconnectDateTime",
                        DateTimeShift = new DateTime(1970, 01, 01)
                    }
                    };
        }

        private List<CompositeFieldsParser> GetCompositeFieldParsers_SMS_Nokia()
        {
            return new List<CompositeFieldsParser> 
                    { 
                    new TimestampDateTimeCompositeParser
                    {
                        FieldName = "MessageTimestamp",
                        DateTimeFieldName = "MessageTime",
                        DateTimeShift = new DateTime(1970, 01, 01)
                    }
                    };
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_MOT_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingNumber"
                    }
                },
                Length = 10,
                Position = 28

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledNumber"
                    }
                },
                Length = 12,
                Position = 54
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledIMEI"
                    }
                },
                Length = 8,
                Position = 46
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledIMSI"
                    }
                },
                Length = 8,
                Position = 38
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "ConnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 166
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "SetupTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 105
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "DisconnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 112
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "DurationInSeconds"
                    }
                },
                Length = 3,
                Position = 125
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "LastExchangeISDN"
                    }
                },
                Length = 10,
                Position = 76
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledLocationInformation_First"
                    }
                },
                Length = 2,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledLocationInformation_Last"
                    }
                },
                Length = 2,
                Position = 86
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "FirstSiteId"
                    }
                },
                Length = 2,
                Position = 74
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "LastSiteId"
                    }
                },
                Length = 2,
                Position = 88
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_Forward_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingNumber"
                    }
                },
                Length = 10,
                Position = 78

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledNumber"
                    }
                },
                Length = 12,
                Position = 37
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledIMEI"
                    }
                },
                Length = 8,
                Position = 57
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledIMSI"
                    }
                },
                Length = 8,
                Position = 49
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingIMSI"
                    }
                },
                Length = 8,
                Position = 29
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "SetupTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 164
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "ConnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 111
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "DisconnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 118
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "DurationInSeconds"
                    }
                },
                Length = 3,
                Position = 132
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_Roaming_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingNumber"
                    }
                },
                Length = 10,
                Position = 28

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledNumber"
                    }
                },
                Length = 12,
                Position = 46
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledIMSI"
                    }
                },
                Length = 8,
                Position = 38
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "SetupTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 136
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "ConnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 91
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "DisconnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 98
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "DurationInSeconds"
                    }
                },
                Length = 3,
                Position = 111
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_SMMO()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingNumber"
                    }
                },
                Length = 10,
                Position = 41

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledNumber"
                    }
                },
                Length = 12,
                Position = 79
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingIMEI"
                    }
                },
                Length = 8,
                Position = 33
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingIMSI"
                    }
                },
                Length = 8,
                Position = 25
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "MessageTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 116
            });


            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_SMMT()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingNumber"
                    }
                },
                Length = 10,
                Position = 81

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledNumber"
                    }
                },
                Length = 12,
                Position = 41
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "ServedIMEI"
                    }
                },
                Length = 8,
                Position = 33
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledIMSI"
                    }
                },
                Length = 8,
                Position = 25
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "MessageTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 105
            });


            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_PSTN_Originated_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingNumber"
                    }
                },
                Length = 12,
                Position = 29

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledNumber"
                    }
                },
                Length = 12,
                Position = 42
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "SetupTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 118
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "ConnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 65
            });
            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "DisconnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 72
            });
            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "DurationInSeconds"
                    }
                },
                Length = 3,
                Position = 86
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_PSTN_Terminated_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingNumber"
                    }
                },
                Length = 12,
                Position = 29

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledNumber"
                    }
                },
                Length = 12,
                Position = 42
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "SetupTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 123
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "ConnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 65
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "DisconnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "DurationInSeconds"
                    }
                },
                Length = 3,
                Position = 86
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_PBX_Originated_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingNumber"
                    }
                },
                Length = 12,
                Position = 29

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledNumber"
                    }
                },
                Length = 12,
                Position = 42
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "SetupTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 115
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "ConnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 65
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "DisconnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "DurationInSeconds"
                    }
                },
                Length = 3,
                Position = 85
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_PBX_Terminated_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CallingNumber"
                    }
                },
                Length = 12,
                Position = 29

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledNumber"
                    }
                },
                Length = 12,
                Position = 42
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "SetupTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 100
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "ConnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 65
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "DisconnectDateTime",
                        DayIndex = 3,
                        MonthIndex = 4,
                        YearIndex = 5,
                        HoursIndex = 2,
                        MinutesIndex = 1,
                        SecondsIndex = 0,
                        WithOffset = false
                    }
                },
                Length = 7,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new HexTLVFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "DurationInSeconds"
                    }
                },
                Length = 3,
                Position = 85
            });

            return fieldParsers;
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


        public IDataParserInput Input
        {
            get;
            set;
        }

        public void OnRecordParsed(ParsedRecord parsedRecord)
        {
            _OnRecordParsed(parsedRecord);
        }

        public ParsedRecord CreateRecord(string recordType, HashSet<string> tempFieldNames)
        {
            FldDictParsedRecord parsedRecord = new FldDictParsedRecord
            {

            };
            return parsedRecord;
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
