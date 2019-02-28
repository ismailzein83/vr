using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Vanrise.Common;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers;
using Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers;
using Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.FieldParsers;
using Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.RecordParsers;
using Vanrise.DataParser.MainExtensions.BinaryParsers.HuaweiParser.FieldParsers;
using Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers;
using Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers.PackageFieldParser;
using Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.RecordParsers;
using Vanrise.DataParser.MainExtensions.CompositeFieldParsers;
using Vanrise.DataParser.MainExtensions.StringFieldParsers;

namespace Mediation.Runtime.DataParser
{
    public class DataParserTester
    {
        static string mediationFolderPath;

        static DataParserTester()
        {
            mediationFolderPath = ConfigurationManager.AppSettings["MediationFolderPath"];
            if (string.IsNullOrEmpty(mediationFolderPath))
                mediationFolderPath = @"D:\Mediation";
        }

        public void GenerateMediationSettings()
        {
            try
            {
                CreateMediationSettingsFile(GetHuaweiNamibiaParserSettings(), "Huawei_Namibia");
                CreateMediationSettingsFile(GetHuaweiIraqParserSettings(), "Huawei_Iraq");
                CreateMediationSettingsFile(GetHuaweiIraqParserSettings_GPRS(), "Huawei_Iraq_GPRS");
                CreateMediationSettingsFile(GetEricssonIraqParserSettings(), "Ericsson_Iraq");
                CreateMediationSettingsFile(GetNokiaParserSettings(), "Nokia_Iraq");
                CreateMediationSettingsFile(GetEricssonParserSettings_GPRS(), "Ericsson_GPRS");
                CreateMediationSettingsFile(GetWHSEricssonOgero_ParserSettings(), "WHS_Ericsson");
                CreateMediationSettingsFile(GetICXNokiaSiemens_ParserSettings(), "ICX_NokiaSiemens");
                CreateMediationSettingsFile(GetICXAlcatel_ParserSettings(), "ICX_Alcatel");
                CreateMediationSettingsFile(GetHuaweiIMSOgero_ParserSettings(), "HuaweiIMS_Ogero");
                CreateMediationSettingsFile(GetHuaweiMGCFOgero_ParserSettings(), "HuaweiMGCF_Ogero");
                CreateMediationSettingsFile(GetHuaweiEPCOgero_ParserSettings(), "HuaweiEPC_Ogero");
                CreateMediationSettingsFile(GetHuaweiMobilis_ParserSettings(), "Huawei_Mobilis");
                CreateMediationSettingsFile(GetEricssonMobilisParserSettings(), "Ericsson_Mobilis");
            }
            catch (Exception ex)
            {
                CreateMediationSettingsFile(ex.StackTrace, "Errors");
            }
        }

        private void CreateMediationSettingsFile(string settings, string name)
        {
            string path = string.Format(@"{0}\{1}.txt", mediationFolderPath, name);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                using (var tw = new StreamWriter(fileStream))
                {
                    tw.Write(settings);
                    tw.Close();
                }
            }
        }

        private DateTimeParser GetDateTimeParser(string fieldName)
        {
            DateTimeParser dateTimeParser = new DateTimeParser()
            {
                FieldName = fieldName,
                DateTimeParsingType = DateTimeParsingType.DateTime,
                IsBCD = true,
                YearIndex = 0,
                MonthIndex = 1,
                DayIndex = 2,
                HoursIndex = 3,
                MinutesIndex = 4,
                SecondsIndex = 5,
                TimeShiftIndicatorIndex = 6,
                HoursTimeShiftIndex = 7,
                MinutesTimeShiftIndex = 8
            };

            return dateTimeParser;
        }

        #region Ogero

        #region Alcatel

        public string GetICXAlcatel_ParserSettings()
        {
            BinaryParserType hexParser = new BinaryParserType
            {
                RecordParser = new SkipRecordParser()
                {
                    RecordStartingTag = "c6c4",
                    RecordParser = new BinaryRecordParser()
                    {
                        Settings = new PositionedBlockRecordParser()
                        {
                            BlockSize = 2064,
                            RecordParser = new BinaryRecordParser()
                            {
                                Settings = new Vanrise.DataParser.MainExtensions.BinaryParsers.AlcatelParsers.RecordParsers.HeaderRecordParser()
                                {
                                    HeaderByteLength = 32,
                                    RecordStartingTag = "c6c4",
                                    TagsToIgnore = new List<string>() { "a5110002", "a5010002" },
                                    RecordParser = new BinaryRecordParser()
                                    {
                                        Settings = new PositionedBlockRecordParser()
                                        {
                                            BlockSize = 42,
                                            RecordParser = new BinaryRecordParser()
                                            {
                                                Settings = new PositionedFieldsRecordParser()
                                                {
                                                    RecordType = "Ogero_ICX_Alcatel_CDR",
                                                    FieldParsers = GetPositionedFieldParsers(),
                                                    CompositeFieldsParsers = GetAlcatelCompositeParsers(),
                                                    ZeroBytesBlockAction = ZeroBytesBlockAction.Skip
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            ParserType parserType = new ParserType
            {
                ParserTypeId = new Guid("30553F01-CE03-4D29-9BF5-80D0D06DFA34"),
                Settings = new ParserTypeSettings { ExtendedSettings = hexParser }
            };

            return Serializer.Serialize(parserType.Settings);
        }
        private List<PositionedFieldParser> GetPositionedFieldParsers()
        {
            List<PositionedFieldParser> positionedFieldParsers = new List<PositionedFieldParser>();

            PositionedFieldParser dateOfCallParser = new PositionedFieldParser()
            {
                Position = 3,
                Length = 2,
                FieldParser = new BinaryFieldParser()
                {
                    Settings = new BCDNumberParser()
                    {
                        FieldName = "DayNumber"
                    }
                }
            };
            positionedFieldParsers.Add(dateOfCallParser);

            PositionedFieldParser timeOfCallParser = new PositionedFieldParser
            {
                Position = 5,
                Length = 3,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new TimeFieldParser
                    {
                        FieldName = "TimeOfCall"
                    }
                }
            };
            positionedFieldParsers.Add(timeOfCallParser);

            PositionedFieldParser aNumberParser = new PositionedFieldParser
            {
                Position = 8,
                Length = 10,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new Vanrise.DataParser.MainExtensions.BinaryParsers.AlcatelParsers.FieldParsers.NumberFieldParser
                    {
                        FieldName = "ANumber",
                        ReservedBytesLength = 6
                    }
                }
            };
            positionedFieldParsers.Add(aNumberParser);

            PositionedFieldParser bNumberParser = new PositionedFieldParser
            {
                Position = 18,
                Length = 12,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new Vanrise.DataParser.MainExtensions.BinaryParsers.AlcatelParsers.FieldParsers.NumberFieldParser
                    {
                        FieldName = "BNumber",
                        ReservedBytesLength = 8
                    }
                }
            };
            positionedFieldParsers.Add(bNumberParser);

            PositionedFieldParser durationInSecondsParser = new PositionedFieldParser
            {
                Position = 30,
                Length = 2,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "DurationInSeconds",
                        FieldBytesLength = 2,
                        FieldIndex = 0,
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(durationInSecondsParser);

            return positionedFieldParsers;
        }
        private List<CompositeFieldsParser> GetAlcatelCompositeParsers()
        {
            List<CompositeFieldsParser> compositeParsers = new List<CompositeFieldsParser>();
            compositeParsers.Add(new FileNameCompositeParser() { FieldName = "FileName" });
            compositeParsers.Add(new DataSourceCompositeParser() { DataSourceFieldName = "DataSourceId" });
            compositeParsers.Add(new DateFromDayNumberCompositeParser() { FieldName = "DateOfCall", YearFieldName = "Year", DayNumberFieldName = "DayNumber" });
            return compositeParsers;
        }

        #endregion

        #region Ericsson 

        public string GetWHSEricssonOgero_ParserSettings()
        {
            //HexTLVParserType hexParser = new HexTLVParserType
            //{
            //    RecordParser = new PositionedBlockRecordParser
            //    {
            //        RecordParser = new PositionedBlockRecordParser//HexTLVRecordParser
            //        {
            //            RecordParser = new HexTLVRecordParser
            //            {
            //                Settings = new SplitByPositionedTypeRecordParser
            //                {
            //                    RecordTypeLength = 2,
            //                    RecordTypePosition = 0,
            //                    SubRecordsParsersByRecordType = GetEricssonOgeroSubRecordsParsers(),
            //                    RecordTypeFieldType = RecordTypeFieldType.String
            //                }
            //            }
            //        },
            //        BlockSize = 115
            //    }
            //};

            BinaryParserType hexParser = new BinaryParserType
            {
                RecordParser = new PositionedBlockRecordParser
                {
                    RecordParser = new BinaryRecordParser//HexTLVRecordParser
                    {
                        Settings = new PositionedBlockRecordParser// SplitByPositionedTypeRecordParser
                        {
                            BlockSize = 115,
                            RecordParser = new BinaryRecordParser
                            {
                                Settings = new SplitByPositionedTypeRecordParser
                                {
                                    RecordTypeLength = 2,
                                    RecordTypePosition = 0,
                                    SubRecordsParsersByRecordType = GetEricssonOgeroSubRecordsParsers(),
                                    RecordTypeFieldType = RecordTypeFieldType.String
                                }
                            }
                        }
                    },
                    BlockSize = 2048
                }
            };

            ParserType parserType = new ParserType
            {
                ParserTypeId = new Guid("6F27B54C-90F3-4332-8437-1ADFFDB8ED2D"),
                Settings = new ParserTypeSettings
                {
                    ExtendedSettings = hexParser
                }
            };

            string val = Serializer.Serialize(parserType.Settings);
            var result = Serializer.Deserialize<ParserTypeSettings>(val);
            return val;
        }
        private Dictionary<string, BinaryRecordParser> GetEricssonOgeroSubRecordsParsers()
        {
            Dictionary<string, BinaryRecordParser> recordParsers = new Dictionary<string, BinaryRecordParser>();

            recordParsers.Add("00", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_00_Ogero_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> {
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = "00"
                     },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "SwitchId",
                            Value = 3
                        }
                    },
                    RecordType = "WHS_Ericsson_CDR"
                    //CompositeFieldsParsers = GetOgeroCompositeFields()
                }
            });
            recordParsers.Add("01", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_01_Ogero_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> {
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = "01"
                     },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "SwitchId",
                            Value = 3
                        }
                    },
                    RecordType = "WHS_Ericsson_CDR"
                    //CompositeFieldsParsers = GetOgeroCompositeFields()
                }
            });

            recordParsers.Add("02", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_02_Ogero_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> {
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = "02"
                     },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "SwitchId",
                            Value = 3
                        }
                    },
                    RecordType = "WHS_Ericsson_CDR"
                    //CompositeFieldsParsers = GetOgeroCompositeFields()
                }
            });
            recordParsers.Add("03", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_04_Ogero_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> {
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = "03"
                     },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "SwitchId",
                            Value = 3
                        }
                    },
                    RecordType = "WHS_Ericsson_CDR"
                    //CompositeFieldsParsers = GetOgeroCompositeFields()
                }
            });

            recordParsers.Add("04", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_04_Ogero_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> {
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = "04"
                     },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "SwitchId",
                            Value = 3
                        }
                    },
                    RecordType = "WHS_Ericsson_CDR"
                    //CompositeFieldsParsers = GetOgeroCompositeFields()
                }
            });

            recordParsers.Add("05", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_04_Ogero_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> {
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = "05"
                     },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "SwitchId",
                            Value = 3
                        }
                    },
                    RecordType = "WHS_Ericsson_CDR"
                    //CompositeFieldsParsers = GetOgeroCompositeFields()
                }
            });

            recordParsers.Add("06", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_04_Ogero_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> {
                     new ParsedRecordFieldConstantValue{
                      FieldName = "RecordType",
                      Value = "06"
                     },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "SwitchId",
                            Value = 3
                        }
                    },
                    RecordType = "WHS_Ericsson_CDR"
                    //CompositeFieldsParsers = GetOgeroCompositeFields()
                }
            });

            //recordParsers.Add("07", new HexTLVRecordParser
            //{
            //    Settings = new PositionedFieldsRecordParser
            //    {
            //        FieldParsers = Get_PositionedFieldParsers_04_Ogero_Call(),
            //        FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
            //         new ParsedRecordFieldConstantValue{
            //          FieldName = "RecordType",
            //          Value = "07"
            //         },
            //            new ParsedRecordFieldConstantValue
            //            {
            //                FieldName = "SwitchId",
            //                Value = 3
            //            }
            //        },
            //        RecordType = "WHS_Ericsson_CDR"
            //        //CompositeFieldsParsers = GetOgeroCompositeFields()
            //    }
            //});

            //recordParsers.Add("08", new HexTLVRecordParser
            //{
            //    Settings = new PositionedFieldsRecordParser
            //    {
            //        FieldParsers = Get_PositionedFieldParsers_04_Ogero_Call(),
            //        FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
            //         new ParsedRecordFieldConstantValue{
            //          FieldName = "RecordType",
            //          Value = "08"
            //         },
            //            new ParsedRecordFieldConstantValue
            //            {
            //                FieldName = "SwitchId",
            //                Value = 3
            //            }
            //        },
            //        RecordType = "WHS_Ericsson_CDR"
            //        //CompositeFieldsParsers = GetOgeroCompositeFields()
            //    }
            //});

            //recordParsers.Add("09", new HexTLVRecordParser
            //{
            //    Settings = new PositionedFieldsRecordParser
            //    {
            //        FieldParsers = Get_PositionedFieldParsers_04_Ogero_Call(),
            //        FieldConstantValues = new List<ParsedRecordFieldConstantValue> { 
            //         new ParsedRecordFieldConstantValue{
            //          FieldName = "RecordType",
            //          Value = "09"
            //         },
            //            new ParsedRecordFieldConstantValue
            //            {
            //                FieldName = "SwitchId",
            //                Value = 3
            //            }
            //        },
            //        RecordType = "WHS_Ericsson_CDR"
            //        //CompositeFieldsParsers = GetOgeroCompositeFields()
            //    }
            //});


            return recordParsers;
        }
        private List<CompositeFieldsParser> GetOgeroCompositeFields()
        {
            List<CompositeFieldsParser> result = new List<CompositeFieldsParser>();

            result.Add(new DateTimeCompositeParser
            {
                DateFieldName = "DateForStartCharging",
                TimeFieldName = "TimeForStartCharging",
                FieldName = "AttemptDateTime"
            });

            return result;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_00_Ogero_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ANumber"
                    }
                },
                Length = 20,
                Position = 4
            });
            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BNumber"
                    }
                },
                Length = 20,
                Position = 24
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CallStatus"
                    }
                },
                Length = 1,
                Position = 2

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CauseForOutput"
                    }
                },
                Length = 1,
                Position = 3
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ACategory"
                    }
                },
                Length = 2,
                Position = 44
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BCategory"
                    }
                },
                Length = 2,
                Position = 46
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ChargedParty"
                    }
                },
                Length = 1,
                Position = 48
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new DateFromTextParser
                        {
                            FieldName = "DateForStartCharging",
                            DateFormat = "yyMMdd"
                        }
                    }
                },
                Length = 6,
                Position = 49
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new TimeFromTextParser
                        {
                            FieldName = "TimeForStartCharging",
                            TimeFormat = "hhmmss"
                        }
                    }
                },
                Length = 6,
                Position = 55
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "ChargeableDuration",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 6,
                Position = 61
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "FaultCode"
                    }
                },
                Length = 5,
                Position = 67
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ExchangeIdentity"
                    }
                },
                Length = 15,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "RecordNumber",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 2,
                Position = 87
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffClass"
                    }
                },
                Length = 3,
                Position = 89
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffSwitchingIndicator"
                    }
                },
                Length = 1,
                Position = 92
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OriginForCharging"
                    }
                },
                Length = 4,
                Position = 93
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OutgoingRoute"
                    }
                },
                Length = 7,
                Position = 97
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "IncomingRoute"
                    }
                },
                Length = 7,
                Position = 104
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "Reserved"
                    }
                },
                Length = 4,
                Position = 111
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_01_Ogero_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ANumber"
                    }
                },
                Length = 20,
                Position = 4
            });
            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BNumber"
                    }
                },
                Length = 20,
                Position = 24
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CallStatus"
                    }
                },
                Length = 1,
                Position = 2

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CauseForOutput"
                    }
                },
                Length = 1,
                Position = 3
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ACategory"
                    }
                },
                Length = 2,
                Position = 44
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BCategory"
                    }
                },
                Length = 2,
                Position = 46
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ChargedParty"
                    }
                },
                Length = 1,
                Position = 48
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new DateFromTextParser
                        {
                            FieldName = "DateForStartCharging",
                            DateFormat = "yyMMdd"
                        }
                    }
                },
                Length = 6,
                Position = 49
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new TimeFromTextParser
                        {
                            FieldName = "TimeForStartCharging",
                            TimeFormat = "hhmmss"
                        }
                    }
                },
                Length = 6,
                Position = 55
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "ChargeableDuration",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 6,
                Position = 61
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "FaultCode"
                    }
                },
                Length = 5,
                Position = 67
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ExchangeIdentity"
                    }
                },
                Length = 15,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            NumberType = NumberType.Int,
                            FieldName = "RecordNumber"
                        }
                    }
                },
                Length = 2,
                Position = 87
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffClass"
                    }
                },
                Length = 3,
                Position = 89
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffSwitchingIndicator"
                    }
                },
                Length = 1,
                Position = 92
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OriginForCharging"
                    }
                },
                Length = 4,
                Position = 93
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OutgoingRoute"
                    }
                },
                Length = 7,
                Position = 97
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "IncomingRoute"
                    }
                },
                Length = 7,
                Position = 104
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "Reserved"
                    }
                },
                Length = 4,
                Position = 111
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_02_Ogero_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ANumber"
                    }
                },
                Length = 20,
                Position = 4
            });
            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BNumber"
                    }
                },
                Length = 20,
                Position = 24
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CallStatus"
                    }
                },
                Length = 1,
                Position = 2

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CauseForOutput"
                    }
                },
                Length = 1,
                Position = 3
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ACategory"
                    }
                },
                Length = 2,
                Position = 44
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BCategory"
                    }
                },
                Length = 2,
                Position = 46
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ChargedParty"
                    }
                },
                Length = 1,
                Position = 48
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new DateFromTextParser
                        {
                            FieldName = "DateForStartCharging",
                            DateFormat = "yyMMdd"
                        }
                    }
                },
                Length = 6,
                Position = 49
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new TimeFromTextParser
                        {
                            FieldName = "TimeForStartCharging",
                            TimeFormat = "hhmmss"
                        }
                    }
                },
                Length = 6,
                Position = 55
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "ChargeableDuration",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 6,
                Position = 61
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "FaultCode"
                    }
                },
                Length = 5,
                Position = 67
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ExchangeIdentity"
                    }
                },
                Length = 15,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "RecordNumber",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 2,
                Position = 87
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffClass"
                    }
                },
                Length = 3,
                Position = 89
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffSwitchingIndicator"
                    }
                },
                Length = 1,
                Position = 92
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OriginForCharging"
                    }
                },
                Length = 4,
                Position = 93
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OutgoingRoute"
                    }
                },
                Length = 7,
                Position = 97
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "IncomingRoute"
                    }
                },
                Length = 7,
                Position = 104
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "Reserved"
                    }
                },
                Length = 4,
                Position = 111
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_03_Ogero_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ANumber"
                    }
                },
                Length = 20,
                Position = 4
            });
            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BNumber"
                    }
                },
                Length = 20,
                Position = 24
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CallStatus"
                    }
                },
                Length = 1,
                Position = 2

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CauseForOutput"
                    }
                },
                Length = 1,
                Position = 3
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ACategory"
                    }
                },
                Length = 2,
                Position = 44
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BCategory"
                    }
                },
                Length = 2,
                Position = 46
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ChargedParty"
                    }
                },
                Length = 1,
                Position = 48
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new DateFromTextParser
                        {
                            FieldName = "DateForStartCharging",
                            DateFormat = "yyMMdd"
                        }
                    }
                },
                Length = 6,
                Position = 49
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new TimeFromTextParser
                        {
                            FieldName = "TimeForStartCharging",
                            TimeFormat = "hhmmss"
                        }
                    }
                },
                Length = 6,
                Position = 55
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "ChargeableDuration",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 6,
                Position = 61
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "FaultCode"
                    }
                },
                Length = 5,
                Position = 67
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ExchangeIdentity"
                    }
                },
                Length = 15,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "RecordNumber",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 2,
                Position = 87
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffClass"
                    }
                },
                Length = 3,
                Position = 89
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffSwitchingIndicator"
                    }
                },
                Length = 1,
                Position = 92
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OriginForCharging"
                    }
                },
                Length = 4,
                Position = 93
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OutgoingRoute"
                    }
                },
                Length = 7,
                Position = 97
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "IncomingRoute"
                    }
                },
                Length = 7,
                Position = 104
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "Reserved"
                    }
                },
                Length = 4,
                Position = 111
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_04_Ogero_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ANumber"
                    }
                },
                Length = 20,
                Position = 4
            });
            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BNumber"
                    }
                },
                Length = 20,
                Position = 24
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CallStatus"
                    }
                },
                Length = 1,
                Position = 2

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CauseForOutput"
                    }
                },
                Length = 1,
                Position = 3
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ACategory"
                    }
                },
                Length = 2,
                Position = 44
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BCategory"
                    }
                },
                Length = 2,
                Position = 46
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ChargedParty"
                    }
                },
                Length = 1,
                Position = 48
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new DateFromTextParser
                        {
                            FieldName = "DateForStartCharging",
                            DateFormat = "yyMMdd"
                        }
                    }
                },
                Length = 6,
                Position = 49
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new TimeFromTextParser
                        {
                            FieldName = "TimeForStartCharging",
                            TimeFormat = "hhmmss"
                        }
                    }
                },
                Length = 6,
                Position = 55
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "ChargeableDuration",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 6,
                Position = 61
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "FaultCode"
                    }
                },
                Length = 5,
                Position = 67
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ExchangeIdentity"
                    }
                },
                Length = 15,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "RecordNumber",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 2,
                Position = 87
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffClass"
                    }
                },
                Length = 3,
                Position = 89
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffSwitchingIndicator"
                    }
                },
                Length = 1,
                Position = 92
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OriginForCharging"
                    }
                },
                Length = 4,
                Position = 93
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OutgoingRoute"
                    }
                },
                Length = 7,
                Position = 97
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "IncomingRoute"
                    }
                },
                Length = 7,
                Position = 104
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "Reserved"
                    }
                },
                Length = 4,
                Position = 111
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_05_Ogero_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ANumber"
                    }
                },
                Length = 20,
                Position = 4
            });
            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BNumber"
                    }
                },
                Length = 20,
                Position = 24
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CallStatus"
                    }
                },
                Length = 1,
                Position = 2

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CauseForOutput"
                    }
                },
                Length = 1,
                Position = 3
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ACategory"
                    }
                },
                Length = 2,
                Position = 44
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BCategory"
                    }
                },
                Length = 2,
                Position = 46
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ChargedParty"
                    }
                },
                Length = 1,
                Position = 48
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new DateFromTextParser
                        {
                            FieldName = "DateForStartCharging",
                            DateFormat = "yyMMdd"
                        }
                    }
                },
                Length = 6,
                Position = 49
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new TimeFromTextParser
                        {
                            FieldName = "TimeForStartCharging",
                            TimeFormat = "hhmmss"
                        }
                    }
                },
                Length = 6,
                Position = 55
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "ChargeableDuration",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 6,
                Position = 61
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "FaultCode"
                    }
                },
                Length = 5,
                Position = 67
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ExchangeIdentity"
                    }
                },
                Length = 15,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "RecordNumber",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 2,
                Position = 87
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffClass"
                    }
                },
                Length = 3,
                Position = 89
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffSwitchingIndicator"
                    }
                },
                Length = 1,
                Position = 92
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OriginForCharging"
                    }
                },
                Length = 4,
                Position = 93
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OutgoingRoute"
                    }
                },
                Length = 7,
                Position = 97
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "IncomingRoute"
                    }
                },
                Length = 7,
                Position = 104
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "Reserved"
                    }
                },
                Length = 4,
                Position = 111
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_06_Ogero_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ANumber"
                    }
                },
                Length = 20,
                Position = 4
            });
            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BNumber"
                    }
                },
                Length = 20,
                Position = 24
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CallStatus"
                    }
                },
                Length = 1,
                Position = 2

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CauseForOutput"
                    }
                },
                Length = 1,
                Position = 3
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ACategory"
                    }
                },
                Length = 2,
                Position = 44
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BCategory"
                    }
                },
                Length = 2,
                Position = 46
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ChargedParty"
                    }
                },
                Length = 1,
                Position = 48
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new DateFromTextParser
                        {
                            FieldName = "DateForStartCharging",
                            DateFormat = "yyMMdd"
                        }
                    }
                },
                Length = 6,
                Position = 49
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new TimeFromTextParser
                        {
                            FieldName = "TimeForStartCharging",
                            TimeFormat = "hhmmss"
                        }
                    }
                },
                Length = 6,
                Position = 55
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "ChargeableDuration",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 6,
                Position = 61
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "FaultCode"
                    }
                },
                Length = 5,
                Position = 67
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ExchangeIdentity"
                    }
                },
                Length = 15,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "RecordNumber",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 2,
                Position = 87
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffClass"
                    }
                },
                Length = 3,
                Position = 89
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffSwitchingIndicator"
                    }
                },
                Length = 1,
                Position = 92
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OriginForCharging"
                    }
                },
                Length = 4,
                Position = 93
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OutgoingRoute"
                    }
                },
                Length = 7,
                Position = 97
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "IncomingRoute"
                    }
                },
                Length = 7,
                Position = 104
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "Reserved"
                    }
                },
                Length = 4,
                Position = 111
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_07_Ogero_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ANumber"
                    }
                },
                Length = 20,
                Position = 4
            });
            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BNumber"
                    }
                },
                Length = 20,
                Position = 24
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CallStatus"
                    }
                },
                Length = 1,
                Position = 2

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CauseForOutput"
                    }
                },
                Length = 1,
                Position = 3
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ACategory"
                    }
                },
                Length = 2,
                Position = 44
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BCategory"
                    }
                },
                Length = 2,
                Position = 46
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ChargedParty"
                    }
                },
                Length = 1,
                Position = 48
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new DateFromTextParser
                        {
                            FieldName = "DateForStartCharging",
                            DateFormat = "yyMMdd"
                        }
                    }
                },
                Length = 6,
                Position = 49
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new TimeFromTextParser
                        {
                            FieldName = "TimeForStartCharging",
                            TimeFormat = "hhmmss"
                        }
                    }
                },
                Length = 6,
                Position = 55
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "ChargeableDuration",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 6,
                Position = 61
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "FaultCode"
                    }
                },
                Length = 5,
                Position = 67
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ExchangeIdentity"
                    }
                },
                Length = 15,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "RecordNumber",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 2,
                Position = 87
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffClass"
                    }
                },
                Length = 3,
                Position = 89
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffSwitchingIndicator"
                    }
                },
                Length = 1,
                Position = 92
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OriginForCharging"
                    }
                },
                Length = 4,
                Position = 93
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OutgoingRoute"
                    }
                },
                Length = 7,
                Position = 97
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "IncomingRoute"
                    }
                },
                Length = 7,
                Position = 104
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "Reserved"
                    }
                },
                Length = 4,
                Position = 111
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_08_Ogero_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ANumber"
                    }
                },
                Length = 20,
                Position = 4
            });
            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BNumber"
                    }
                },
                Length = 20,
                Position = 24
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CallStatus"
                    }
                },
                Length = 1,
                Position = 2

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CauseForOutput"
                    }
                },
                Length = 1,
                Position = 3
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ACategory"
                    }
                },
                Length = 2,
                Position = 44
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BCategory"
                    }
                },
                Length = 2,
                Position = 46
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ChargedParty"
                    }
                },
                Length = 1,
                Position = 48
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new DateFromTextParser
                        {
                            FieldName = "DateForStartCharging",
                            DateFormat = "yyMMdd"
                        }
                    }
                },
                Length = 6,
                Position = 49
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new TimeFromTextParser
                        {
                            FieldName = "TimeForStartCharging",
                            TimeFormat = "hhmmss"
                        }
                    }
                },
                Length = 6,
                Position = 55
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "ChargeableDuration",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 6,
                Position = 61
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "FaultCode"
                    }
                },
                Length = 5,
                Position = 67
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ExchangeIdentity"
                    }
                },
                Length = 15,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "RecordNumber",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 2,
                Position = 87
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffClass"
                    }
                },
                Length = 3,
                Position = 89
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffSwitchingIndicator"
                    }
                },
                Length = 1,
                Position = 92
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OriginForCharging"
                    }
                },
                Length = 4,
                Position = 93
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OutgoingRoute"
                    }
                },
                Length = 7,
                Position = 97
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "IncomingRoute"
                    }
                },
                Length = 7,
                Position = 104
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "Reserved"
                    }
                },
                Length = 4,
                Position = 111
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_09_Ogero_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ANumber"
                    }
                },
                Length = 20,
                Position = 4
            });
            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BNumber"
                    }
                },
                Length = 20,
                Position = 24
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CallStatus"
                    }
                },
                Length = 1,
                Position = 2

            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "CauseForOutput"
                    }
                },
                Length = 1,
                Position = 3
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ACategory"
                    }
                },
                Length = 2,
                Position = 44
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "BCategory"
                    }
                },
                Length = 2,
                Position = 46
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ChargedParty"
                    }
                },
                Length = 1,
                Position = 48
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new DateFromTextParser
                        {
                            FieldName = "DateForStartCharging",
                            DateFormat = "yyMMdd"
                        }
                    }
                },
                Length = 6,
                Position = 49
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new TimeFromTextParser
                        {
                            FieldName = "TimeForStartCharging",
                            TimeFormat = "hhmmss"
                        }
                    }
                },
                Length = 6,
                Position = 55
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "ChargeableDuration",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 6,
                Position = 61
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "FaultCode"
                    }
                },
                Length = 5,
                Position = 67
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "ExchangeIdentity"
                    }
                },
                Length = 15,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        Parser = new NumberFromTextParser
                        {
                            FieldName = "RecordNumber",
                            NumberType = NumberType.Int
                        }
                    }
                },
                Length = 2,
                Position = 87
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffClass"
                    }
                },
                Length = 3,
                Position = 89
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "TariffSwitchingIndicator"
                    }
                },
                Length = 1,
                Position = 92
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OriginForCharging"
                    }
                },
                Length = 4,
                Position = 93
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "OutgoingRoute"
                    }
                },
                Length = 7,
                Position = 97
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "IncomingRoute"
                    }
                },
                Length = 7,
                Position = 104
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new StringParser
                    {
                        FieldName = "Reserved"
                    }
                },
                Length = 4,
                Position = 111
            });

            return fieldParsers;
        }

        #endregion

        #region Huawei 

        #region Huawei IMS 

        public string GetHuaweiIMSOgero_ParserSettings()
        {
            BinaryParserType hexParser = new BinaryParserType
            {
                RecordParser = new Vanrise.DataParser.MainExtensions.BinaryParsers.HuaweiParser.RecordParsers.HeaderRecordParser
                {
                    HeaderLengthPosition = 4,
                    HeaderBytesLength = 4,
                    RecordParser = new BinaryRecordParser
                    {
                        Settings = new Vanrise.DataParser.MainExtensions.BinaryParsers.HuaweiParser.RecordParsers.HuaweiRecordParser
                        {
                            HeaderLength = 9,
                            RecordLengthPosition = 0,
                            RecordByteLength = 2,
                            RecordTypePosition = 4,
                            RecordTypeByteLength = 2,
                            SubRecordsParsersByRecordType = GetHuaweiIMSOgero_SubRecordsParsers()
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
        private Dictionary<string, BinaryRecordParser> GetHuaweiIMSOgero_SubRecordsParsers()
        {
            Dictionary<string, BinaryRecordParser> parsers = new Dictionary<string, BinaryRecordParser>();

            parsers.Add("BF45", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    RecordType = "Ogero_HuaweiIMS_CDR",
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = GetHuaweiIMSOgero_ATS9900_FieldParsers()
                    },
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    {
                        new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 10}
                    },
                    CompositeFieldsParsers = GetHuaweiOgeroCompositeFieldsParsers()
                }
            });

            parsers.Add("BF3F", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    RecordType = "Ogero_HuaweiIMS_CDR",
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = GetHuaweiIMSOgero_SCSCF_FieldParsers()
                    },
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    {
                        new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 10}
                    },
                    CompositeFieldsParsers = GetHuaweiOgeroCompositeFieldsParsers()
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> GetHuaweiIMSOgero_ATS9900_FieldParsers()
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

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "SipMethod"
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "NodeRole",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("A4", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new StringParser
                                        {
                                             FieldName = "DomainName"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "SessionId"
                }
            });

            parsers.Add("A6", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new StringParser
                                        {
                                             FieldName = "CallingPartyAddress"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("A7", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new StringParser
                                        {
                                             FieldName = "CalledPartyAddress"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    IsBCD = true,
                    FieldName = "ServiceRequestTime",
                    DateTimeParsingType = DateTimeParsingType.DateTime,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2,
                    HoursIndex = 3,
                    MinutesIndex = 4,
                    SecondsIndex = 5,
                    TimeShiftIndicatorIndex = 6,
                    HoursTimeShiftIndex = 7,
                    MinutesTimeShiftIndex = 8

                }
            });

            parsers.Add("8A", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    IsBCD = true,
                    FieldName = "ServiceDeliveryStartTime",
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
                }
            });

            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    IsBCD = true,
                    FieldName = "ServiceDeliveryEndTime",
                    DateTimeParsingType = DateTimeParsingType.DateTime,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2,
                    HoursIndex = 3,
                    MinutesIndex = 4,
                    SecondsIndex = 5,
                    TimeShiftIndicatorIndex = 6,
                    HoursTimeShiftIndex = 7,
                    MinutesTimeShiftIndex = 8
                }
            });

            parsers.Add("8C", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    IsBCD = true,
                    FieldName = "RecordOpeningTime",
                    DateTimeParsingType = DateTimeParsingType.DateTime,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2,
                    HoursIndex = 3,
                    MinutesIndex = 4,
                    SecondsIndex = 5,
                    TimeShiftIndicatorIndex = 6,
                    HoursTimeShiftIndex = 7,
                    MinutesTimeShiftIndex = 8
                }
            });

            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    IsBCD = true,
                    FieldName = "RecordClosureTime",
                    DateTimeParsingType = DateTimeParsingType.DateTime,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2,
                    HoursIndex = 3,
                    MinutesIndex = 4,
                    SecondsIndex = 5,
                    TimeShiftIndicatorIndex = 6,
                    HoursTimeShiftIndex = 7,
                    MinutesTimeShiftIndex = 8

                }
            });

            parsers.Add("AE", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"30", new BinaryFieldParser
                                   {
                                        Settings = new SequenceFieldParser
                                        {
                                            FieldParsers = new BinaryFieldParserCollection
                                            {
                                                FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                                                {
                                                    {"80", new BinaryFieldParser
                                                           {
                                                                Settings = new StringParser
                                                                {
                                                                     FieldName = "OriginatingIOI"
                                                                }
                                                           }
                                                    },
                                                    {"81", new BinaryFieldParser
                                                           {
                                                                Settings = new StringParser
                                                                {
                                                                     FieldName = "TerminatingIOI"
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

            parsers.Add("8F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocalRecordSequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("91", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForRecordClosing",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("B2", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
                                   {
                                        Settings = new BoolFieldParser
                                        {
                                             FieldName = "ACRStartLost"

                                        }
                                   }
                            },
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new BoolFieldParser
                                        {
                                             FieldName = "ACRInterimLost"
                                        }
                                   }
                            },
                            {"82", new BinaryFieldParser
                                   {
                                        Settings = new BoolFieldParser
                                        {
                                             FieldName = "ACRStopLost"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("93", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "IMSChargingIdentifier"
                }
            });

            parsers.Add("B5", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"30", new BinaryFieldParser
                                   {
                                         Settings = new SequenceFieldParser
                                         {
                                             FieldParsers = new BinaryFieldParserCollection
                                             {
                                                 FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                                                 {
                                                     {"80", new BinaryFieldParser
                                                            {
                                                                 Settings = new DateTimeParser
                                                                 {
                                                                    IsBCD = true,
                                                                    FieldName = "SIPRequestTime",
                                                                    DateTimeParsingType = DateTimeParsingType.DateTime,
                                                                    YearIndex = 0,
                                                                    MonthIndex = 1,
                                                                    DayIndex = 2,
                                                                    HoursIndex = 3,
                                                                    MinutesIndex = 4,
                                                                    SecondsIndex = 5,
                                                                    TimeShiftIndicatorIndex = 6,
                                                                    HoursTimeShiftIndex = 7,
                                                                    MinutesTimeShiftIndex = 8
                                                                 }
                                                            }
                                                     },
                                                     {"81", new BinaryFieldParser
                                                            {
                                                                 Settings = new DateTimeParser
                                                                 {
                                                                    IsBCD = true,
                                                                    FieldName = "SIPResponseTime",
                                                                    DateTimeParsingType = DateTimeParsingType.DateTime,
                                                                    YearIndex = 0,
                                                                    MonthIndex = 1,
                                                                    DayIndex = 2,
                                                                    HoursIndex = 3,
                                                                    MinutesIndex = 4,
                                                                    SecondsIndex = 5,
                                                                    TimeShiftIndicatorIndex = 6,
                                                                    HoursTimeShiftIndex = 7,
                                                                    MinutesTimeShiftIndex = 8
                                                                 }
                                                            }
                                                     },
                                                     {"A2", new BinaryFieldParser
                                                            {
                                                                Settings = new SequenceFieldParser
                                                                {
                                                                    FieldParsers = new BinaryFieldParserCollection
                                                                    {
                                                                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser> {
                                                                            {"30", new BinaryFieldParser
                                                                                   {
                                                                                        Settings = new SequenceFieldParser
                                                                                           {
                                                                                               FieldParsers = new BinaryFieldParserCollection
                                                                                               {
                                                                                                   FieldParsersByTag = new Dictionary<string, BinaryFieldParser> {
                                                                                                       {"80", new BinaryFieldParser
                                                                                                              {
                                                                                                                   Settings = new StringParser
                                                                                                                   {
                                                                                                                        FieldName = "SDPMediaName"
                                                                                                                   }
                                                                                                              }
                                                                                                       },
                                                                                                       {"A1", new BinaryFieldParser
                                                                                                              {
                                                                                                                   Settings = new SequenceFieldParser
                                                                                                                   {
                                                                                                                       FieldParsers = new BinaryFieldParserCollection
                                                                                                                       {
                                                                                                                           FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                                                                                                                           {
                                                                                                                               {"19", new BinaryFieldParser
                                                                                                                                      {
                                                                                                                                           Settings = new StringParser
                                                                                                                                           {
                                                                                                                                                FieldName = "SDPMediaDescription"
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
                                                                                   }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                      },
                                                      {"A4", new BinaryFieldParser
                                                             {
                                                                Settings = new SequenceFieldParser
                                                                {
                                                                    FieldParsers = new BinaryFieldParserCollection
                                                                    {
                                                                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                                                                        {
                                                                            {"19", new BinaryFieldParser
                                                                                   {
                                                                                       Settings = new StringParser
                                                                                       {
                                                                                           FieldName = "SDPSessionDescription"

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
                               }
                            }
                        }
                    }
                }
            });

            parsers.Add("B8", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"30", new BinaryFieldParser
                                   {
                                         Settings = new SequenceFieldParser
                                         {
                                              FieldParsers = new BinaryFieldParserCollection
                                              {
                                                  FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                                                  {
                                                      {"80", new BinaryFieldParser
                                                             {
                                                                  Settings = new StringParser
                                                                  {
                                                                       FieldName = "ContentType"
                                                                  }
                                                             }
                                                      },
                                                      {"82", new BinaryFieldParser
                                                             {
                                                                  Settings = new NumberFieldParser
                                                                  {
                                                                       FieldName = "ContentLength",
                                                                       NumberType = NumberType.Int
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

            parsers.Add("97", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ServiceReasonReturnCode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9D", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "AccessNetworkInformation"
                }
            });

            parsers.Add("9E", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "ServiceContextID"
                }
            });

            parsers.Add("BF66", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser> {
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new StringParser
                                        {
                                             FieldName = "CalledAssertedIdentity"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F8148", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "Duration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("BF814B", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new StringParser
                                        {
                                             FieldName = "DialledPartyAddress"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F814C", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RingingDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("BF8155", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"02", new BinaryFieldParser
                                   {
                                        Settings = new NumberFieldParser
                                        {
                                             FieldName = "ServiceIdentifier",
                                             NumberType = NumberType.Int
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F8158", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargingCategory",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F815E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallProperty",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8163", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "AccountingRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8116", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "OnlineChargingFlag",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8237", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "VisitedNetworkId"
                }
            });

            parsers.Add("BF833E", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"83", new BinaryFieldParser
                                   {
                                        Settings = new StringParser
                                        {
                                             FieldName = "OutgoingRoute"
                                        }
                                   }
                            },
                            {"84", new BinaryFieldParser
                                   {
                                        Settings = new StringParser
                                        {
                                             FieldName = "IncomingRoute"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> GetHuaweiIMSOgero_SCSCF_FieldParsers()
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

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "SipMethod"
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "NodeRole",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("A4", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new StringParser
                                        {
                                             FieldName = "DomainName"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "SessionId"
                }
            });

            parsers.Add("A6", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new StringParser
                                        {
                                             FieldName = "CallingPartyAddress"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("A7", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new StringParser
                                        {
                                             FieldName = "CalledPartyAddress"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    IsBCD = true,
                    FieldName = "ServiceRequestTime",
                    DateTimeParsingType = DateTimeParsingType.DateTime,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2,
                    HoursIndex = 3,
                    MinutesIndex = 4,
                    SecondsIndex = 5,
                    TimeShiftIndicatorIndex = 6,
                    HoursTimeShiftIndex = 7,
                    MinutesTimeShiftIndex = 8
                }
            });

            parsers.Add("8A", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    IsBCD = true,
                    FieldName = "ServiceDeliveryStartTime",
                    DateTimeParsingType = DateTimeParsingType.DateTime,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2,
                    HoursIndex = 3,
                    MinutesIndex = 4,
                    SecondsIndex = 5,
                    TimeShiftIndicatorIndex = 6,
                    HoursTimeShiftIndex = 7,
                    MinutesTimeShiftIndex = 8
                }
            });

            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    IsBCD = true,
                    FieldName = "ServiceDeliveryEndTime",
                    DateTimeParsingType = DateTimeParsingType.DateTime,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2,
                    HoursIndex = 3,
                    MinutesIndex = 4,
                    SecondsIndex = 5,
                    TimeShiftIndicatorIndex = 6,
                    HoursTimeShiftIndex = 7,
                    MinutesTimeShiftIndex = 8
                }
            });

            parsers.Add("8C", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    IsBCD = true,
                    FieldName = "RecordOpeningTime",
                    DateTimeParsingType = DateTimeParsingType.DateTime,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2,
                    HoursIndex = 3,
                    MinutesIndex = 4,
                    SecondsIndex = 5,
                    TimeShiftIndicatorIndex = 6,
                    HoursTimeShiftIndex = 7,
                    MinutesTimeShiftIndex = 8
                }
            });

            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    IsBCD = true,
                    FieldName = "RecordClosureTime",
                    DateTimeParsingType = DateTimeParsingType.DateTime,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2,
                    HoursIndex = 3,
                    MinutesIndex = 4,
                    SecondsIndex = 5,
                    TimeShiftIndicatorIndex = 6,
                    HoursTimeShiftIndex = 7,
                    MinutesTimeShiftIndex = 8
                }
            });

            parsers.Add("AE", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"30", new BinaryFieldParser
                                   {
                                        Settings = new SequenceFieldParser
                                        {
                                            FieldParsers = new BinaryFieldParserCollection
                                            {
                                                FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                                                {
                                                    {"80", new BinaryFieldParser
                                                           {
                                                                Settings = new StringParser
                                                                {
                                                                     FieldName = "OriginatingIOI"
                                                                }
                                                           }
                                                    },
                                                    {"81", new BinaryFieldParser
                                                           {
                                                                Settings = new StringParser
                                                                {
                                                                     FieldName = "TerminatingIOI"
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

            parsers.Add("8F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocalRecordSequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("91", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForRecordClosing",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("B2", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
                                   {
                                        Settings = new BoolFieldParser
                                        {
                                             FieldName = "ACRStartLost"

                                        }
                                   }
                            },
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new BoolFieldParser
                                        {
                                             FieldName = "ACRInterimLost"
                                        }
                                   }
                            },
                            {"82", new BinaryFieldParser
                                   {
                                        Settings = new BoolFieldParser
                                        {
                                             FieldName = "ACRStopLost"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("93", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "IMSChargingIdentifier"
                }
            });

            parsers.Add("B5", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"30", new BinaryFieldParser
                                   {
                                         Settings = new SequenceFieldParser
                                         {
                                             FieldParsers = new BinaryFieldParserCollection
                                             {
                                                 FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                                                 {
                                                     {"80", new BinaryFieldParser
                                                            {
                                                                 Settings = new DateTimeParser
                                                                 {
                                                                    IsBCD = true,
                                                                    FieldName = "SIPRequestTime",
                                                                    DateTimeParsingType = DateTimeParsingType.DateTime,
                                                                    YearIndex = 0,
                                                                    MonthIndex = 1,
                                                                    DayIndex = 2,
                                                                    HoursIndex = 3,
                                                                    MinutesIndex = 4,
                                                                    SecondsIndex = 5,
                                                                    TimeShiftIndicatorIndex = 6,
                                                                    HoursTimeShiftIndex = 7,
                                                                    MinutesTimeShiftIndex = 8
                                                                 }
                                                            }
                                                     },
                                                     {"81", new BinaryFieldParser
                                                            {
                                                                 Settings = new DateTimeParser
                                                                 {
                                                                    IsBCD = true,
                                                                    FieldName = "SIPResponseTime",
                                                                    DateTimeParsingType = DateTimeParsingType.DateTime,
                                                                    YearIndex = 0,
                                                                    MonthIndex = 1,
                                                                    DayIndex = 2,
                                                                    HoursIndex = 3,
                                                                    MinutesIndex = 4,
                                                                    SecondsIndex = 5,
                                                                    TimeShiftIndicatorIndex = 6,
                                                                    HoursTimeShiftIndex = 7,
                                                                    MinutesTimeShiftIndex = 8
                                                                 }
                                                            }
                                                     },
                                                     {"A2", new BinaryFieldParser
                                                            {
                                                                Settings = new SequenceFieldParser
                                                                {
                                                                    FieldParsers = new BinaryFieldParserCollection
                                                                    {
                                                                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                                                                        {
                                                                            {"30", new BinaryFieldParser
                                                                                   {
                                                                                        Settings = new SequenceFieldParser
                                                                                           {
                                                                                               FieldParsers = new BinaryFieldParserCollection
                                                                                               {
                                                                                                   FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                                                                                                   {
                                                                                                       {"80", new BinaryFieldParser
                                                                                                              {
                                                                                                                   Settings = new StringParser
                                                                                                                   {
                                                                                                                        FieldName = "SDPMediaName"
                                                                                                                   }
                                                                                                              }
                                                                                                       },
                                                                                                       {"A1", new BinaryFieldParser
                                                                                                              {
                                                                                                                   Settings = new SequenceFieldParser
                                                                                                                   {
                                                                                                                       FieldParsers = new BinaryFieldParserCollection
                                                                                                                       {
                                                                                                                           FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                                                                                                                           {
                                                                                                                               {"19", new BinaryFieldParser
                                                                                                                                      {
                                                                                                                                           Settings = new StringParser
                                                                                                                                           {
                                                                                                                                                FieldName = "SDPMediaDescription"
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
                                                                                   }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                      },
                                                      {"A4", new BinaryFieldParser
                                                             {
                                                                Settings = new SequenceFieldParser
                                                                {
                                                                    FieldParsers = new BinaryFieldParserCollection
                                                                    {
                                                                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                                                                        {
                                                                            {"19", new BinaryFieldParser
                                                                                   {
                                                                                       Settings = new StringParser
                                                                                       {
                                                                                           FieldName = "SDPSessionDescription"

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
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("B8", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"30", new BinaryFieldParser
                                   {
                                         Settings = new SequenceFieldParser
                                         {
                                              FieldParsers = new BinaryFieldParserCollection
                                              {
                                                  FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                                                  {
                                                      {"80", new BinaryFieldParser
                                                             {
                                                                  Settings = new StringParser
                                                                  {
                                                                      FieldName = "ContentType"
                                                                  }
                                                             }
                                                      },
                                                      {"82", new BinaryFieldParser
                                                             {
                                                                  Settings = new NumberFieldParser
                                                                  {
                                                                       FieldName = "ContentLength",
                                                                       NumberType = NumberType.Int
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

            parsers.Add("97", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ServiceReasonReturnCode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9D", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "AccessNetworkInformation"
                }
            });

            parsers.Add("9E", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "ServiceContextID"
                }
            });

            parsers.Add("BF66", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new StringParser
                                        {
                                             FieldName = "CalledAssertedIdentity"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F8148", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "Duration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("BF814B", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new StringParser
                                        {
                                             FieldName = "DialledPartyAddress"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F814C", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RingingDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("BF8155", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"02", new BinaryFieldParser
                                   {
                                        Settings = new NumberFieldParser
                                        {
                                             FieldName = "ServiceIdentifier",
                                             NumberType = NumberType.Int
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F8158", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargingCategory",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F815E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallProperty",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8163", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "AccountingRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8116", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "OnlineChargingFlag",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8237", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "VisitedNetworkId"
                }
            });

            parsers.Add("9F814D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SessionPriority",
                    NumberType = NumberType.Int
                }
            });

            return parsers;
        }
        private List<CompositeFieldsParser> GetHuaweiOgeroCompositeFieldsParsers()
        {
            List<CompositeFieldsParser> compositeParsers = new List<CompositeFieldsParser>();
            compositeParsers.Add(new FileNameCompositeParser() { FieldName = "FileName" });
            compositeParsers.Add(new DataSourceCompositeParser() { DataSourceFieldName = "DataSourceId" });
            return compositeParsers;
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
                Settings = GetDateTimeParser("RecordOpeningTime")
            });

            parsers.Add("9F26", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("StartTime")
            });

            parsers.Add("9F27", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("StopTime")
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

        #endregion

        #region  Huawei MGCF 

        public string GetHuaweiMGCFOgero_ParserSettings()
        {
            BinaryParserType hexParser = new BinaryParserType
            {
                RecordParser = new Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.RecordParsers.SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = new Dictionary<string, BinaryRecordParser>()
                    {
                        {"30", new BinaryRecordParser()
                               {
                                   Settings = new Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.RecordParsers.SplitByTagRecordParser()
                                   {
                                       SubRecordsParsersByTag = GetHuaweiMGCFOgero_SubRecordsParsers()
                                   }
                               }
                        }
                    }
                }
            };

            ParserType parserType = new ParserType
            {
                Name = "Huawei MGCF Ogero Parser",
                ParserTypeId = new Guid("D6896CC8-22EF-4FAA-BEF4-4644EE5323F9"),
                Settings = new ParserTypeSettings
                {
                    ExtendedSettings = hexParser
                }
            };

            return Serializer.Serialize(parserType.Settings);
        }
        private Dictionary<string, BinaryRecordParser> GetHuaweiMGCFOgero_SubRecordsParsers()
        {
            Dictionary<string, BinaryRecordParser> secondLevelSubRecordsParsersByTag = new Dictionary<string, BinaryRecordParser>();

            secondLevelSubRecordsParsersByTag.Add("A1", new BinaryRecordParser()
            {
                Settings = new Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.RecordParsers.SplitByTagRecordParser()
                {
                    SubRecordsParsersByTag = GetHuaweiMGCFOgero_RecordTypeParsers()
                }
            });

            return secondLevelSubRecordsParsersByTag;
        }
        private Dictionary<string, BinaryRecordParser> GetHuaweiMGCFOgero_RecordTypeParsers()
        {
            Dictionary<string, BinaryRecordParser> parsers = new Dictionary<string, BinaryRecordParser>();

            //Incoming Record Type
            parsers.Add("A3", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    RecordType = "Ogero_HuaweiMGCF_CDR",
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = GetHuaweiMGCFOgero_IncomingType_FieldParsers()
                    },
                    CompositeFieldsParsers = GetHuaweiMGCFOgero_CompositeFieldsParsers()
                }
            });

            //Outgiong Record Type
            parsers.Add("A4", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    RecordType = "Ogero_HuaweiMGCF_CDR",
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = GetHuaweiMGCFOgero_OutgoingType_FieldParsers()
                    },
                    CompositeFieldsParsers = GetHuaweiMGCFOgero_CompositeFieldsParsers()
                }
            });

            //Transit Record Type
            parsers.Add("A5", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    RecordType = "Ogero_HuaweiMGCF_CDR",
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = GetHuaweiMGCFOgero_TransitType_FieldParsers()
                    },
                    CompositeFieldsParsers = GetHuaweiMGCFOgero_CompositeFieldsParsers()
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> GetHuaweiMGCFOgero_IncomingType_FieldParsers()
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

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RecordingEntity",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            parsers.Add("A4", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
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

            parsers.Add("A5", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
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

            parsers.Add("86", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("SeizureTime")

            });

            parsers.Add("87", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("AnswerTime")

            });

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("ReleaseTime")
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForTerm",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AC", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
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

            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CallReference"
                }
            });

            parsers.Add("8E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("96", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("BF8102", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"83", new BinaryFieldParser
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

            parsers.Add("BF8105", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
                                   {
                                       Settings = new NumberFieldParser
                                       {
                                           FieldName = "AdditionalChgInfo",
                                           NumberType = NumberType.Int
                                       }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F810E", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8120", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RoamingNumber",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            //parsers.Add("9F8126", new BinaryFieldParser
            //{
            //    Settings = new NumberFieldParser
            //    {
            //        FieldName = "MSCOutgoingCircuit",
            //        NumberType = NumberType.Int
            //    }
            //});

            parsers.Add("9F8127", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCIncomingCircuit",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F812A", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallEmlppPriority",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8134", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallerPortedFlag",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F813E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SubscriberCategory",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8143", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CUGOutgoingAccessIndicator",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8146", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCIncomingRouteAttribute",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8147", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCOutgoingRouteAttribute",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8148", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "NetworkCallReference"
                }
            });

            parsers.Add("9F8149", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("SetupTime")
            });

            parsers.Add("9F814A", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("AlertingTime")
            });

            parsers.Add("9F814B", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "VoiceIndicator",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F814C", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "BCategory",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F814D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F815A", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "DisconnectParty",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8161", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "AudioDataType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargeLevel",
                    NumberType = NumberType.Int
                }
            });

            //parsers.Add("9F817B", new BinaryFieldParser
            //{
            //    Settings = new NumberFieldParser
            //    {
            //        FieldName = "CMNFlag",
            //        NumberType = NumberType.Int
            //    }
            //});

            parsers.Add("9F817B", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "ICIDValue"
                }
            });

            parsers.Add("9F817C", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OrigIOI"
                }
            });

            parsers.Add("9F817D", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "TermIOI"
                }
            });

            parsers.Add("9F817F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CalledPortedFlag",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8202", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "IntermediateChargingInd",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8205", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCOutgoingRouteNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8206", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCIncomingRouteNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8233", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RingingDuration",
                    NumberType = NumberType.Int
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> GetHuaweiMGCFOgero_OutgoingType_FieldParsers()
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

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RecordingEntity",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            parsers.Add("A4", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
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

            parsers.Add("A5", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
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

            parsers.Add("86", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("SeizureTime")

            });

            parsers.Add("87", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("AnswerTime")

            });

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("ReleaseTime")
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForTerm",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AC", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
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

            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CallReference"
                }
            });

            parsers.Add("8E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("96", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("BF8102", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"83", new BinaryFieldParser
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

            parsers.Add("BF8105", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
                                   {
                                       Settings = new NumberFieldParser
                                       {
                                           FieldName = "AdditionalChgInfo",
                                           NumberType = NumberType.Int
                                       }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F810E", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8120", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RoamingNumber",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            parsers.Add("9F8126", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCOutgoingCircuit",
                    NumberType = NumberType.Int
                }
            });

            //parsers.Add("9F8127", new BinaryFieldParser
            //{
            //    Settings = new NumberFieldParser
            //    {
            //        FieldName = "MSCIncomingCircuit",
            //        NumberType = NumberType.Int
            //    }
            //});

            parsers.Add("9F812A", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallEmlppPriority",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8134", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallerPortedFlag",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F813E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SubscriberCategory",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8143", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CUGOutgoingAccessIndicator",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8146", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCIncomingRouteAttribute",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8147", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCOutgoingRouteAttribute",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8148", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "NetworkCallReference"
                }
            });

            parsers.Add("9F8149", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("SetupTime")
            });

            parsers.Add("9F814A", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("AlertingTime")
            });

            parsers.Add("9F814B", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "VoiceIndicator",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F814C", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "BCategory",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F814D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F815A", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "DisconnectParty",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8161", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "AudioDataType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargeLevel",
                    NumberType = NumberType.Int
                }
            });

            //parsers.Add("9F817B", new BinaryFieldParser
            //{
            //    Settings = new NumberFieldParser
            //    {
            //        FieldName = "CMNFlag",
            //        NumberType = NumberType.Int
            //    }
            //});

            parsers.Add("9F817B", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "ICIDValue"
                }
            });

            parsers.Add("9F817C", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OrigIOI"
                }
            });

            parsers.Add("9F817D", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "TermIOI"
                }
            });

            parsers.Add("9F817F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CalledPortedFlag",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8202", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "IntermediateChargingInd",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8205", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCOutgoingRouteNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8206", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCIncomingRouteNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8233", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RingingDuration",
                    NumberType = NumberType.Int
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> GetHuaweiMGCFOgero_TransitType_FieldParsers()
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

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RecordingEntity",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            parsers.Add("A2", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
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

            parsers.Add("A3", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
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

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            parsers.Add("87", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("SeizureTime")

            });

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("AnswerTime")

            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("ReleaseTime")
            });

            parsers.Add("8A", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8C", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForTerm",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AD", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
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

            parsers.Add("8E", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CallReference"
                }
            });

            parsers.Add("8F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("97", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("BF8102", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"83", new BinaryFieldParser
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

            parsers.Add("BF8105", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
                                   {
                                       Settings = new NumberFieldParser
                                       {
                                           FieldName = "AdditionalChgInfo",
                                           NumberType = NumberType.Int
                                       }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F810E", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8120", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RoamingNumber",
                    AIsZero = false,
                    RemoveHexa = true,
                    ByteOffset = 1
                }
            });

            parsers.Add("9F8126", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCOutgoingCircuit",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8127", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCIncomingCircuit",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F812A", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallEmlppPriority",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8134", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallerPortedFlag",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F813E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SubscriberCategory",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8143", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CUGOutgoingAccessIndicator",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8146", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCIncomingRouteAttribute",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8147", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCOutgoingRouteAttribute",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8148", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "NetworkCallReference"
                }
            });

            parsers.Add("9F8149", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("SetupTime")
            });

            parsers.Add("9F814A", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("AlertingTime")
            });

            parsers.Add("9F814B", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "VoiceIndicator",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F814C", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "BCategory",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F814D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F815A", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "DisconnectParty",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8161", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "AudioDataType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargeLevel",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817B", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CMNFlag",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817C", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "ICIDValue"
                }
            });

            parsers.Add("9F817D", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OrigIOI"
                }
            });

            parsers.Add("9F817E", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "TermIOI"
                }
            });

            parsers.Add("9F817F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CalledPortedFlag",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8202", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "IntermediateChargingInd",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8205", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCOutgoingRouteNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8206", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCIncomingRouteNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8233", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RingingDuration",
                    NumberType = NumberType.Int
                }
            });

            return parsers;
        }
        private List<CompositeFieldsParser> GetHuaweiMGCFOgero_CompositeFieldsParsers()
        {
            List<CompositeFieldsParser> compositeParsers = new List<CompositeFieldsParser>();
            compositeParsers.Add(new FileNameCompositeParser() { FieldName = "FileName" });
            compositeParsers.Add(new DataSourceCompositeParser() { DataSourceFieldName = "DataSourceId" });
            return compositeParsers;
        }

        #endregion

        #region  Huawei EPC 

        public string GetHuaweiEPCOgero_ParserSettings()
        {
            BinaryParserType hexParser = new BinaryParserType
            {
                RecordParser = new Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.RecordParsers.SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = new Dictionary<string, BinaryRecordParser>()
                    {
                        {"BF4F", new BinaryRecordParser()
                            {
                                Settings = new Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.RecordParsers.CreateRecordRecordParser()
                                {
                                    RecordType = "Ogero_HuaweiEPC_CDR",
                                    FieldParsers = new BinaryFieldParserCollection()
                                        {
                                            FieldParsersByTag = GetHuaweiEPCOgero_FieldParsers(),
                                        },
                                    CompositeFieldsParsers = GetHuaweiEPCOgero_CompositeFieldsParsers()
                                }
                            }
                        }
                    }
                }
            };

            ParserType parserType = new ParserType
            {
                Name = "Huawei EPC Ogero Parser",
                ParserTypeId = new Guid("95037A1B-EF0C-4F2B-8B22-F51EE65ACD45"),
                Settings = new ParserTypeSettings
                {
                    ExtendedSettings = hexParser
                }
            };

            return Serializer.Serialize(parserType.Settings);
        }
        private Dictionary<string, BinaryFieldParser> GetHuaweiEPCOgero_FieldParsers()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            //int
            parsers.Add("80", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordType",
                    NumberType = NumberType.Int
                }
            });

            //TBCDNumberParser
            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSI",
                    AIsZero = false,
                    RemoveHexa = true,
                }
            });

            //seq IPv4Parser
            parsers.Add("A4", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
                                {
                                    Settings = new IPv4Parser
                                    {
                                        FieldName = "PGWAddress"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            //BigInt
            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargingID",
                    NumberType = NumberType.BigInt
                }
            });

            //seq IPv4Parser
            parsers.Add("A6", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
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
                            }
                        }
                    }
                }
            });

            //string
            parsers.Add("87", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "AccessPointNameNI"
                }
            });

            //seq seq IPv4Parser
            parsers.Add("A9", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"A0", new BinaryFieldParser
                                {
                                    Settings = new SequenceFieldParser
                                    {
                                        FieldParsers = new BinaryFieldParserCollection
                                        {
                                            FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                                            {
                                                {"80", new BinaryFieldParser
                                                    {
                                                        Settings = new IPv4Parser
                                                        {
                                                                FieldName = "ServedPDPPDNAddress"
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

            //hexa
            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "DynamicAddressFlag"
                }
            });

            //change of char
            parsers.Add("AC", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"30", new BinaryFieldParser
                                {
                                    Settings =new SequenceFieldParser
                                    {
                                        FieldParsers = new BinaryFieldParserCollection()
                                        {
                                            FieldParsersByTag= GetChangeOfChar_FieldParsers()
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });


            //dateTime
            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("RecordOpeningTime")
            });

            //int
            parsers.Add("8E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "Duration",
                    NumberType = NumberType.Int
                }
            });

            //int
            parsers.Add("8F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForRecClosing",
                    NumberType = NumberType.Int
                }
            });

            //int
            parsers.Add("91", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordSequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            //string
            parsers.Add("92", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "NodeID"
                }
            });

            //contact info
            //parsers.Add("B3", new BinaryFieldParser
            //{
            //	Settings = new Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.RecordParsers.SplitByTagRecordParser()
            //	{
            //		SubRecordsParsersByTag = GetContactInfo_FieldParsers()
            //	}
            //});

            //int
            parsers.Add("95", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "APNSelectionMode",
                    NumberType = NumberType.Int
                }
            });

            //TBCDNumberParser
            parsers.Add("96", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    AIsZero = false,
                    RemoveHexa = true,
                }
            });

            //hexa
            parsers.Add("97", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargingCharacteristics"
                }
            });

            //int
            parsers.Add("98", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChSelectionMode",
                    NumberType = NumberType.Int
                }
            });

            //TBCDNumberParser
            parsers.Add("9B", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServingNodePLMNIdentifier",
                    AIsZero = false,
                    RemoveHexa = true,
                }
            });

            //TBCDNumberParser
            parsers.Add("9D", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEISV",
                    AIsZero = false,
                    RemoveHexa = true,
                }
            });

            //int
            parsers.Add("9E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RATType",
                    NumberType = NumberType.Int
                }
            });

            //change of services
            parsers.Add("BF22", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"30", new BinaryFieldParser
                                {
                                    Settings =new SequenceFieldParser
                                    {
                                        FieldParsers = new BinaryFieldParserCollection()
                                        {
                                            FieldParsersByTag= GetChangeOfServiceConditions_FieldParsers()
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });


            //seq int
            parsers.Add("BF23", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"0A", new BinaryFieldParser
                                   {
                                        Settings = new NumberFieldParser
                                        {
                                            FieldName = "ServingNodeType",
                                            NumberType = NumberType.Int
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> GetChangeOfChar_FieldParsers()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            //BigInt
            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "DataVolumeGPRSUplink",
                    NumberType = NumberType.BigInt
                }
            });

            //BigInt
            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "DataVolumeGPRSDownlink",
                    NumberType = NumberType.BigInt
                }
            });

            //int
            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChangeCondition",
                    NumberType = NumberType.Int
                }
            });

            //dateTime
            parsers.Add("86", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("ChangeTime")
            });

            //hexa
            parsers.Add("87", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "FailureHandlingContinue"
                }
            });

            //hexa
            parsers.Add("93", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CPCIoTEPSOptimisationIndicator"
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> GetContactInfo_FieldParsers()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            //int
            parsers.Add("91", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocalTimeZone",
                    NumberType = NumberType.Int
                }
            });

            //int
            parsers.Add("8A", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "Roaming",
                    NumberType = NumberType.Int
                }
            });

            //bool
            parsers.Add("8A", new BinaryFieldParser
            {
                Settings = new BoolFieldParser
                {
                    FieldName = "Roaming"
                }
            });

            //hexa
            parsers.Add("8A", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Roaming"
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> GetChangeOfServiceConditions_FieldParsers()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            //int
            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RatingGroup",
                    NumberType = NumberType.Int
                }
            });

            //BigInt
            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocalSequenceNumber",
                    NumberType = NumberType.BigInt
                }
            });

            //dateTime
            parsers.Add("85", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("TimeOfFirstUsage")
            });

            //dateTime
            parsers.Add("86", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("TimeOfLastUsage")
            });

            //int
            parsers.Add("87", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TimeUsage",
                    NumberType = NumberType.Int
                }
            });

            //hexa
            parsers.Add("88", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ServiceConditionChange"
                }
            });

            //BigInt
            parsers.Add("8C", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "DataVolumeFBCUplink",
                    NumberType = NumberType.BigInt
                }
            });

            //BigInt
            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "DataVolumeFBCDownlink",
                    NumberType = NumberType.BigInt
                }
            });

            //dateTime
            parsers.Add("8E", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("TimeOfReport")
            });

            return parsers;
        }
        private List<CompositeFieldsParser> GetHuaweiEPCOgero_CompositeFieldsParsers()
        {
            List<CompositeFieldsParser> compositeParsers = new List<CompositeFieldsParser>();
            compositeParsers.Add(new FileNameCompositeParser() { FieldName = "FileName" });
            compositeParsers.Add(new DataSourceCompositeParser() { DataSourceFieldName = "DataSourceId" });
            return compositeParsers;
        }

        #endregion

        #endregion

        #region Nokia Siemens 

        public string GetICXNokiaSiemens_ParserSettings()
        {
            BinaryParserType hexParser = new BinaryParserType
            {
                RecordParser = new Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.RecordParsers.HeaderRecordParser
                {
                    HeaderLength = 8,
                    HeaderTagLength = 1,
                    RecordLengthByteLength = 2,
                    PackageRecordParser = new BinaryRecordParser
                    {
                        Settings = new PackageRecordParser
                        {
                            PackageTagLength = 1,
                            PackageLengthByteLength = 1,
                            RecordType = "Ogero_ICX_NokiaSiemens_CDR",
                            Packages = GetNokiaSiemensPackages(),
                            CompositeFieldsParsers = GetNokiaSiemensCompositeParsers()
                        }
                    }
                }
            };

            ParserType parserType = new ParserType
            {
                ParserTypeId = new Guid("202C8508-A24C-4664-B769-BE71C86FCD75"),
                Settings = new ParserTypeSettings { ExtendedSettings = hexParser }
            };

            return Serializer.Serialize(parserType.Settings);
        }
        private Dictionary<int, PackageFieldParser> GetNokiaSiemensPackages()
        {
            Dictionary<int, PackageFieldParser> packages = new Dictionary<int, PackageFieldParser>();

            PackageFieldParser dateTimePackage_100 = new FixedLengthPackageFieldParser()
            {
                PackageLength = 11,
                FieldParser = new DateTimePackageParser()
                {
                    BeginDateTimeFieldName = "BeginDate",
                    EndDateTimeFieldName = "EndDate",
                    DurationFieldName = "DurationInSeconds",
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2,
                    HoursIndex = 3,
                    MinutesIndex = 4,
                    SecondsIndex = 5,
                    FlagsMillisecondsIndex = 6,
                    DurationIndex = 7
                }
            };
            packages.Add(100, dateTimePackage_100);

            PackageFieldParser trunkIdentificationIncommingPackage_105 = new FixedLengthPackageFieldParser()
            {
                PackageLength = 9,
                FieldParser = new TrunkIdentificationPackageParser()
                {
                    TrunkGroupNumberFieldName = "IncomingTrunkGroupNumber",
                    TrunkNumberFieldName = "IncomingTrunkNumber"
                }
            };
            packages.Add(105, trunkIdentificationIncommingPackage_105);

            PackageFieldParser trunkIdentificationOutgoingPackage_106 = new FixedLengthPackageFieldParser()
            {
                PackageLength = 9,
                FieldParser = new TrunkIdentificationPackageParser()
                {
                    TrunkGroupNumberFieldName = "OutgoingTrunkGroupNumber",
                    TrunkNumberFieldName = "OutgoingTrunkNumber"
                }
            };
            packages.Add(106, trunkIdentificationOutgoingPackage_106);

            PackageFieldParser trunkIdentificationIncommingCICPackage_107 = new FixedLengthPackageFieldParser()
            {
                PackageLength = 10,
                FieldParser = new TrunkIdentificationCICPackageParser()
                {
                    TrunkGroupNumberCICFieldName = "IncomingTrunkGroupNumberCIC"
                }
            };
            packages.Add(107, trunkIdentificationIncommingCICPackage_107);

            PackageFieldParser trunkIdentificationOutgoingCICPackage_108 = new FixedLengthPackageFieldParser()
            {
                PackageLength = 10,
                FieldParser = new TrunkIdentificationCICPackageParser()
                {
                    TrunkGroupNumberCICFieldName = "OutgoingTrunkGroupNumberCIC"
                }
            };
            packages.Add(108, trunkIdentificationOutgoingCICPackage_108);

            PackageFieldParser connectionIdentificationPackage_110 = new DirectLengthPackageFieldParser()
            {
                FieldParser = new ConnectionIdentificationPackageParser()
                {
                    ConnectionIdentificationFieldName = "ConnectionIdentification"
                }
            };
            packages.Add(110, connectionIdentificationPackage_110);

            PackageFieldParser zonePackage_122 = new FixedLengthPackageFieldParser()
            {
                PackageLength = 3,
                FieldParser = new ZonePackageParser()
                {
                    ZoneFieldName = "Zone"
                }
            };
            packages.Add(122, zonePackage_122);

            PackageFieldParser callingPartyNumberPackage_142 = new DirectLengthPackageFieldParser()
            {
                FieldParser = new PartyNumberPackageParser()
                {
                    PartyNumberFieldName = "CallingPartyNumber",
                    NADIFieldName = "CallingNADI",
                    NPIFieldName = "CallingNPI"
                }
            };
            packages.Add(142, callingPartyNumberPackage_142);

            PackageFieldParser calledPartyNumberPackage_168 = new DirectLengthPackageFieldParser()
            {
                FieldParser = new PartyNumberPackageParser()
                {
                    PartyNumberFieldName = "CalledPartyNumber",
                    NADIFieldName = "CalledNADI",
                    NPIFieldName = "CalledNPI"
                }
            };
            packages.Add(168, calledPartyNumberPackage_168);

            PackageFieldParser trafficQualityPackage_130 = new DirectLengthPackageFieldParser()
            {
                FieldParser = new TrafficQualityDataPackageParser()
                {
                    CauseValueFieldName = "CauseValue",
                    CodingStandardFieldName = "CodingStandard",
                    LocationFieldName = "Location"
                }
            };
            packages.Add(130, trafficQualityPackage_130);

            PackageFieldParser serviceInfoPackage_102 = new FixedLengthPackageFieldParser() { PackageLength = 4, FieldParser = new SkipBlockParser() };
            packages.Add(102, serviceInfoPackage_102);

            PackageFieldParser chargeUnitsForConnectionPackage_103 = new FixedLengthPackageFieldParser() { PackageLength = 4, FieldParser = new SkipBlockParser() };
            packages.Add(103, chargeUnitsForConnectionPackage_103);

            PackageFieldParser chargeUnitsForFacilityUsagePackage_104 = new FixedLengthPackageFieldParser() { PackageLength = 3, FieldParser = new SkipBlockParser() };
            packages.Add(104, chargeUnitsForFacilityUsagePackage_104);

            PackageFieldParser dateTimePackage_116 = new FixedLengthPackageFieldParser() { PackageLength = 8, FieldParser = new SkipBlockParser() };
            packages.Add(116, dateTimePackage_116);

            PackageFieldParser digitStringPackage_118 = new DigitStringPackageFieldParser() { FieldParser = new SkipBlockParser() };
            packages.Add(118, digitStringPackage_118);

            PackageFieldParser transmissionMediumRequirementPackage_119 = new FixedLengthPackageFieldParser() { PackageLength = 2, FieldParser = new SkipBlockParser() };
            packages.Add(119, transmissionMediumRequirementPackage_119);

            PackageFieldParser callingPartyCategoryPackage_120 = new FixedLengthPackageFieldParser() { PackageLength = 2, FieldParser = new SkipBlockParser() };
            packages.Add(120, callingPartyCategoryPackage_120);

            PackageFieldParser zeroBytePackage_0 = new FixedLengthPackageFieldParser() { PackageLength = 1, FieldParser = new SkipBlockParser() };
            packages.Add(0, zeroBytePackage_0);

            return packages;
        }
        private List<CompositeFieldsParser> GetNokiaSiemensCompositeParsers()
        {
            List<CompositeFieldsParser> compositeParsers = new List<CompositeFieldsParser>();
            compositeParsers.Add(new FileNameCompositeParser() { FieldName = "FileName" });
            compositeParsers.Add(new DataSourceCompositeParser() { DataSourceFieldName = "DataSourceId" });
            return compositeParsers;
        }

        #endregion

        #endregion

        #region Iraq

        #region Ericsson 

        #region Ericsson 

        public string GetEricssonIraqParserSettings()
        {
            BinaryParserType hexParser = new BinaryParserType
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
        private Dictionary<Guid, BinaryRecordParser> GetTemplates()
        {
            Dictionary<Guid, BinaryRecordParser> templates = new Dictionary<Guid, BinaryRecordParser>();

            templates.Add(new Guid("8861E765-B865-43AB-AAF7-39DFB1079E04"), new BinaryRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = GetTemplateParsers()
                }
            });

            return templates;
        }
        private Dictionary<string, BinaryRecordParser> GetTemplateParsers()
        {
            Dictionary<string, BinaryRecordParser> parsers = new Dictionary<string, BinaryRecordParser>();

            parsers.Add("A0", new BinaryRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_Template_CreateRecordsParsersByTag()
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryRecordParser> Get_A0_SubRecordsParsersByTag()
        {
            Dictionary<string, BinaryRecordParser> subParser = new Dictionary<string, BinaryRecordParser>();

            subParser.Add("A0", new BinaryRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_Template_CreateRecordsParsersByTag()
                }
            });

            subParser.Add("A1", new BinaryRecordParser
            {
                Settings = new ExecuteTemplateRecordParser
                {
                    RecordParserTemplateId = new Guid("8861E765-B865-43AB-AAF7-39DFB1079E04")
                }
            });

            return subParser;
        }
        private Dictionary<string, BinaryRecordParser> Get_Template_CreateRecordsParsersByTag()
        {
            Dictionary<string, BinaryRecordParser> subParser = new Dictionary<string, BinaryRecordParser>();

            subParser.Add("A0", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
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
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordTypeName",
                            Value = "Transit"
                        }
                    },
                    RecordType = "Iraq_Mobile_CDR",
                    TempFieldsNames = GetTempFieldsName(),
                    CompositeFieldsParsers = GetCompositeFieldParsers()
                }
            });

            subParser.Add("A1", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
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
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordTypeName",
                            Value = "MobileOriginated"
                        }
                    },
                    RecordType = "Iraq_Mobile_CDR",
                    TempFieldsNames = GetTempFieldsName(),
                    CompositeFieldsParsers = GetCompositeFieldParsers()
                }
            });

            subParser.Add("A2", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
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
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordTypeName",
                            Value = "Roaming"
                        }
                    },
                    RecordType = "Iraq_Mobile_CDR",
                    TempFieldsNames = GetTempFieldsName(),
                    CompositeFieldsParsers = GetCompositeFieldParsers()
                }
            });

            subParser.Add("A3", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
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
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordTypeName",
                            Value = "CallForwarding"
                        }
                    },
                    RecordType = "Iraq_Mobile_CDR",
                    TempFieldsNames = GetTempFieldsName(),
                    CompositeFieldsParsers = GetCompositeFieldParsers()
                }
            });

            subParser.Add("A4", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
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
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordTypeName",
                            Value = "MobileTerminated"
                        }
                    },
                    RecordType = "Iraq_Mobile_CDR",
                    TempFieldsNames = GetTempFieldsName(),
                    CompositeFieldsParsers = GetCompositeFieldParsers()
                }
            });

            subParser.Add("A5", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
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
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordTypeName",
                            Value = "SMSOriginated"
                        }
                    },
                    RecordType = "Iraq_SMS",
                    CompositeFieldsParsers = GetCompositeFieldParsers_SMS()
                }
            });

            subParser.Add("A7", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
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
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordTypeName",
                            Value = "SMSTerminated"
                        }
                    },
                    RecordType = "Iraq_SMS",
                    CompositeFieldsParsers = GetCompositeFieldParsers_SMS()
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
                       new FileNameCompositeParser { FieldName = "FileName" },
                       new GuidCompositeParser { FieldName = "UniqueIdentifier" },
                       new DateTimeCompositeParser { FieldName = "ConnectDateTime", DateFieldName = "Date", TimeFieldName = "StartTime" },
                       new DateTimeCompositeParser { FieldName = "DisconnectDateTime", DateFieldName = "Date",TimeFieldName = "StopTime" },
                       new DateTimeCompositeParser { FieldName = "SetupTime", DateFieldName = "ConnectDateTime", TimeFieldName = "TimeFromRegisterSeizureToStartOfCharging", SubtractTime = true },
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
                       },
                       new TimestampDateTimeCompositeParser
                       {
                          FieldName = "SetupTimestamp",
                          DateTimeFieldName = "SetupTime",
                          DateTimeShift = new DateTime(1970, 01, 01)
                       }
                   };
        }
        private List<CompositeFieldsParser> GetCompositeFieldParsers_SMS()
        {
            return new List<CompositeFieldsParser>
                   {
                        new TimestampDateTimeCompositeParser
                        {
                            FieldName = "MessageTimestamp",
                            DateTimeFieldName = "MessageTime",
                            DateTimeShift = new DateTime(1970, 01, 01)
                        },
                        new GuidCompositeParser { FieldName="UniqueIdentifier" },
                        new FileNameCompositeParser { FieldName = "FileName" }
                   };
        }
        private Dictionary<string, BinaryFieldParser> Get_A0_Transit_FieldParsers()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            parsers.Add("88", new BinaryFieldParser
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

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StartTime"
                }
            });

            parsers.Add("8A", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StopTime"
                }
            });

            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "CallDuration"
                }
            });

            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "TimeFromRegisterSeizureToStartOfCharging"
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9D", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F50", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F54", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F2E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "GlobalCallReference",
                    NumberType = NumberType.BigInt,
                    ConvertOutputToString = true
                }
            });

            parsers.Add("9F26", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    NumberType = NumberType.Int,
                    FieldName = "IntermediateChargingIndicator"
                }
            });

            parsers.Add("9F27", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    NumberType = NumberType.Int,
                    FieldName = "IntermediateRecordNumber"
                }
            });

            parsers.Add("95", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OutgoingRoute"
                }
            });

            parsers.Add("96", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "IncomingRoute"
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A1_MSOriginating_FieldParsers()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            parsers.Add("89", new BinaryFieldParser
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

            parsers.Add("8A", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StartTime"
                }
            });

            parsers.Add("9F44", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "GlobalCallReference",
                    NumberType = NumberType.BigInt,
                    ConvertOutputToString = true
                }
            });

            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StopTime"
                }
            });

            parsers.Add("8C", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "CallDuration"
                }
            });

            parsers.Add("8E", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "TimeFromRegisterSeizureToStartOfCharging"
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("87", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F7E", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8102", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9B", new BinaryFieldParser
            {
                Settings = new CallLocationInformationParser
                {
                    FieldName = "Calling_First_CI"
                }
            });

            parsers.Add("9C", new BinaryFieldParser
            {
                Settings = new CallLocationInformationParser
                {
                    FieldName = "Calling_Last_CI"
                }
            });

            parsers.Add("99", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F30", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    NumberType = NumberType.Int,
                    FieldName = "IntermediateChargingIndicator"
                }
            });

            parsers.Add("9F31", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    NumberType = NumberType.Int,
                    FieldName = "IntermediateRecordNumber"
                }
            });

            parsers.Add("96", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OutgoingRoute"
                }
            });

            parsers.Add("97", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "IncomingRoute"
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A2_Roaming_FieldParsers()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            parsers.Add("89", new BinaryFieldParser
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

            parsers.Add("8A", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StartTime"
                }
            });

            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StopTime"
                }
            });

            parsers.Add("8C", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "CallDuration"
                }
            });

            parsers.Add("8E", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "TimeFromRegisterSeizureToStartOfCharging"
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9C", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F3A", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F3D", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F31", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "GlobalCallReference",
                    NumberType = NumberType.BigInt,
                    ConvertOutputToString = true
                }
            });

            parsers.Add("9F27", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    NumberType = NumberType.Int,
                    FieldName = "IntermediateChargingIndicator"
                }
            });

            parsers.Add("9F28", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    NumberType = NumberType.Int,
                    FieldName = "IntermediateRecordNumber"
                }
            });

            parsers.Add("95", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OutgoingRoute"
                }
            });

            parsers.Add("96", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "IncomingRoute"
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A3_CallForwarding_FieldParsers()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            parsers.Add("8D", new BinaryFieldParser
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

            parsers.Add("8E", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StartTime"
                }
            });

            parsers.Add("8F", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StopTime"
                }
            });

            parsers.Add("90", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "CallDuration"
                }
            });

            parsers.Add("92", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "TimeFromRegisterSeizureToStartOfCharging"
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F53", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F56", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9C", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F39", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "GlobalCallReference",
                    NumberType = NumberType.BigInt,
                    ConvertOutputToString = true
                }
            });

            parsers.Add("9F29", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    NumberType = NumberType.Int,
                    FieldName = "IntermediateChargingIndicator"
                }
            });

            parsers.Add("9F2A", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    NumberType = NumberType.Int,
                    FieldName = "IntermediateRecordNumber"
                }
            });

            parsers.Add("99", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OutgoingRoute"
                }
            });

            parsers.Add("9A", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "IncomingRoute"
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A4_MSTerminating_FieldParsers()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            parsers.Add("8A", new BinaryFieldParser
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

            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StartTime"
                }
            });

            parsers.Add("8C", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "StopTime"
                }
            });

            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "CallDuration"
                }
            });

            parsers.Add("8F", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "TimeFromRegisterSeizureToStartOfCharging"
                }
            });

            parsers.Add("86", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("87", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F70", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F72", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F24", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9B", new BinaryFieldParser
            {
                Settings = new CallLocationInformationParser
                {
                    FieldName = "Called_First_CI"
                }
            });

            parsers.Add("9C", new BinaryFieldParser
            {
                Settings = new CallLocationInformationParser
                {
                    FieldName = "Called_Last_CI"
                }
            });

            parsers.Add("99", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F43", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "GlobalCallReference",
                    NumberType = NumberType.BigInt,
                    ConvertOutputToString = true
                }
            });

            parsers.Add("9F2E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    NumberType = NumberType.Int,
                    FieldName = "IntermediateChargingIndicator"
                }
            });

            parsers.Add("9F2F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    NumberType = NumberType.Int,
                    FieldName = "IntermediateRecordNumber"
                }
            });

            parsers.Add("96", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OutgoingRoute"
                }
            });

            parsers.Add("97", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "IncomingRoute"
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A5_MSOriginatingSMS_FieldParsers()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            parsers.Add("87", new BinaryFieldParser
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

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "MessageTime",
                    DateTimeParsingType = DateTimeParsingType.Time,
                    WithOffset = false,
                    HoursIndex = 0,
                    MinutesIndex = 1,
                    SecondsIndex = 2
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A7_MSTerminatingSMS_FieldParsers()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            parsers.Add("86", new BinaryFieldParser
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

            parsers.Add("87", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "MessageTime",
                    DateTimeParsingType = DateTimeParsingType.Time,
                    WithOffset = false,
                    HoursIndex = 0,
                    MinutesIndex = 1,
                    SecondsIndex = 2
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            return parsers;
        }

        #endregion

        #region Ericsson GPRS

        public string GetEricssonParserSettings_GPRS()
        {
            BinaryParserType hexParser = new BinaryParserType
            {
                RecordParser = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = GetTemplateParsers_Ericsson_GPRS()
                }
            };

            ParserType parserType = new ParserType
            {
                Name = "Huawei Iraq GPRS Parser",
                ParserTypeId = new Guid("B9648105-8914-4C70-8550-F63D946F5B0C"),
                Settings = new ParserTypeSettings
                {
                    ExtendedSettings = hexParser
                }
            };

            return Serializer.Serialize(parserType.Settings);
        }
        private Dictionary<string, BinaryRecordParser> GetTemplateParsers_Ericsson_GPRS()
        {
            Dictionary<string, BinaryRecordParser> parsers = new Dictionary<string, BinaryRecordParser>();

            parsers.Add("BF4F", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_B4_FieldParsers_Ericsson()
                    },
                    RecordType = "Iraq_GPRS",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> {
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 2}
                    },
                    CompositeFieldsParsers = GetEricssonCompositeFieldsParser_GPRS()
                }

            });

            return parsers;
        }
        private List<CompositeFieldsParser> GetEricssonCompositeFieldsParser_GPRS()
        {
            List<CompositeFieldsParser> fieldParsers = new List<CompositeFieldsParser>
            {

            };

            return fieldParsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_B4_FieldParsers_Ericsson()
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
                Settings = GetDateTimeParser("RecordOpeningTime")
            });

            parsers.Add("9F26", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("StartTime")
            });

            parsers.Add("9F27", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("StopTime")
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

        #endregion

        #endregion

        #region Huawei 

        #region Huawei 

        public string GetHuaweiIraqParserSettings()
        {
            BinaryParserType hexParser = new BinaryParserType
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
        private Dictionary<string, BinaryRecordParser> Get_30_SubRecordsParsersByTag_Iraq()
        {
            Dictionary<string, BinaryRecordParser> subParser = new Dictionary<string, BinaryRecordParser>();

            subParser.Add("30", new BinaryRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_30_SplitRecordsParsersByTag_Iraq()
                }
            });

            return subParser;
        }
        private Dictionary<string, BinaryRecordParser> Get_30_SplitRecordsParsersByTag_Iraq()
        {
            Dictionary<string, BinaryRecordParser> subParser = new Dictionary<string, BinaryRecordParser>();

            subParser.Add("A1", new BinaryRecordParser
            {
                Settings = new ExecuteTemplateRecordParser
                {
                    RecordParserTemplateId = new Guid("E9FD1F21-4D0A-467B-8DC9-885543BE1E76")
                }
            });

            return subParser;
        }
        private Dictionary<Guid, BinaryRecordParser> GetTemplates_Huawei_Iraq()
        {
            Dictionary<Guid, BinaryRecordParser> templates = new Dictionary<Guid, BinaryRecordParser>();

            templates.Add(new Guid("E9FD1F21-4D0A-467B-8DC9-885543BE1E76"), new BinaryRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = GetTemplateParsers_Huawei_Iraq()
                }
            });

            return templates;
        }
        private Dictionary<string, BinaryRecordParser> GetTemplateParsers_Huawei_Iraq()
        {
            Dictionary<string, BinaryRecordParser> parsers = new Dictionary<string, BinaryRecordParser>();

            parsers.Add("A0", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_A0_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "Iraq_Mobile_CDR",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    {
                        new ParsedRecordFieldConstantValue { FieldName = "SwitchId", Value = 5},
                        new ParsedRecordFieldConstantValue { FieldName = "RecordTypeName", Value = "MobileOriginated" }
                    },
                    CompositeFieldsParsers = GetHuaweiIraqCompositeFieldsParser()
                }
            });

            parsers.Add("A1", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_A1_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "Iraq_Mobile_CDR",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    {
                        new ParsedRecordFieldConstantValue { FieldName = "SwitchId", Value = 5},
                        new ParsedRecordFieldConstantValue { FieldName = "RecordTypeName", Value = "MobileTerminated" }
                    },
                    CompositeFieldsParsers = GetHuaweiIraqCompositeFieldsParser()
                }
            });

            //parsers.Add("A3", new BinaryRecordParser
            //{
            //    Settings = new CreateRecordRecordParser
            //    {
            //        FieldParsers = new BinaryFieldParserCollection
            //        {
            //            FieldParsersByTag = Get_A3_FieldParsers_Huawei_Iraq()
            //        },
            //        RecordType = "Iraq_Mobile_CDR",
            //        FieldConstantValues = new List<ParsedRecordFieldConstantValue>
            //        {
            //            new ParsedRecordFieldConstantValue { FieldName = "SwitchId", Value = 5},
            //            new ParsedRecordFieldConstantValue { FieldName = "RecordTypeName", Value = "IncomingGateway" }
            //        },
            //        CompositeFieldsParsers = GetHuaweiIraqCompositeFieldsParser()
            //    }
            //});

            //parsers.Add("A4", new BinaryRecordParser
            //{
            //    Settings = new CreateRecordRecordParser
            //    {
            //        FieldParsers = new BinaryFieldParserCollection
            //        {
            //            FieldParsersByTag = Get_A4_FieldParsers_Huawei_Iraq()
            //        },
            //        RecordType = "Iraq_Mobile_CDR",
            //        FieldConstantValues = new List<ParsedRecordFieldConstantValue>
            //        {
            //            new ParsedRecordFieldConstantValue { FieldName = "SwitchId", Value = 5},
            //            new ParsedRecordFieldConstantValue { FieldName = "RecordTypeName", Value = "OutgoingGateway" }
            //        },
            //        CompositeFieldsParsers = GetHuaweiIraqCompositeFieldsParser()
            //    }
            //});

            parsers.Add("A5", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_A5_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "Iraq_Mobile_CDR",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    {
                        new ParsedRecordFieldConstantValue { FieldName = "SwitchId", Value = 5},
                        new ParsedRecordFieldConstantValue { FieldName = "RecordTypeName", Value = "Transit" }
                    },
                    CompositeFieldsParsers = GetHuaweiIraqCompositeFieldsParser()
                }
            });

            parsers.Add("BF64", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_A100_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "Iraq_Mobile_CDR",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    {
                        new ParsedRecordFieldConstantValue { FieldName = "SwitchId", Value = 5},
                        new ParsedRecordFieldConstantValue { FieldName = "RecordTypeName", Value = "CallForwarding" }
                    },
                    CompositeFieldsParsers = GetHuaweiIraqCompositeFieldsParser()
                }
            });

            parsers.Add("A6", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_A6_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "Iraq_SMS",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    {
                        new ParsedRecordFieldConstantValue { FieldName = "SwitchId", Value = 5},
                        new ParsedRecordFieldConstantValue {  FieldName = "RecordTypeName", Value = "SMSOriginated" }
                    },
                    CompositeFieldsParsers = GetHuaweiIraqCompositeFieldsParser_SMS()
                }
            });

            parsers.Add("A7", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_A7_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "Iraq_SMS",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    {
                        new ParsedRecordFieldConstantValue { FieldName = "SwitchId", Value = 5},
                        new ParsedRecordFieldConstantValue { FieldName = "RecordTypeName", Value = "SMSTerminated" }
                    },
                    CompositeFieldsParsers = GetHuaweiIraqCompositeFieldsParser_SMS()
                }
            });

            return parsers;
        }
        private List<CompositeFieldsParser> GetHuaweiIraqCompositeFieldsParser()
        {
            List<CompositeFieldsParser> fieldParsers = new List<CompositeFieldsParser>
            {
                new GuidCompositeParser() { FieldName = "UniqueIdentifier" },
                new FileNameCompositeParser() { FieldName = "FileName" },
                new TimestampDateTimeCompositeParser()
                {
                    FieldName = "ConnectTimestamp",
                    DateTimeFieldName = "ConnectDateTime",
                    DateTimeShift = new DateTime(1970, 01, 01)
                },
                new TimestampDateTimeCompositeParser()
                {
                    FieldName = "DisconnectTimestamp",
                    DateTimeFieldName = "DisconnectDateTime",
                    DateTimeShift = new DateTime(1970, 01, 01)
                },
                new CopyFieldValueCompositeParser()
                {
                    SourceFieldName = "ConnectDateTime",
                    TargetFieldName = "SetupTime",
                    OverrideOnlyIfDefault = true
                }
            };

            return fieldParsers;
        }
        private List<CompositeFieldsParser> GetHuaweiIraqCompositeFieldsParser_SMS()
        {
            List<CompositeFieldsParser> fieldParsers = new List<CompositeFieldsParser> {
            new TimestampDateTimeCompositeParser
                    {
                        FieldName = "MessageTimestamp",
                        DateTimeFieldName = "MessageTime",
                        DateTimeShift = new DateTime(1970, 01, 01)
                    },
                        new GuidCompositeParser
                    {
                        FieldName = "UniqueIdentifier"
                    },
                     new FileNameCompositeParser{
                         FieldName = "FileName"
                        }
            };

            return fieldParsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A0_FieldParsers_Huawei_Iraq()
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

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("AC", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new NumberFieldParser
                                        {
                                             FieldName = "Calling_First_CI",
                                             NumberType = NumberType.Int,
                                             ConvertOutputToString = true
                                        }
                                   }
                            },
                            {"80", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "LocationAreaCode",
                                          NumberType = NumberType.Int,
                                          ConvertOutputToString = true
                                     }
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("AD", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new NumberFieldParser
                                        {
                                             FieldName = "Calling_Last_CI",
                                             NumberType = NumberType.Int,
                                             ConvertOutputToString = true
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("99", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F21", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F45", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F810E", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8112", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F813C", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("97", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("ConnectDateTime")
            });

            parsers.Add("98", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("DisconnectDateTime")
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

            parsers.Add("9F814D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8170", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ZoneCode"
                }
            });

            parsers.Add("9F8171", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817E", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F8168", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8135", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8104", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F26", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "GlobalCallReference",
                    NumberType = NumberType.BigInt,
                    ConvertOutputToString = true
                }
            });

            parsers.Add("9F20", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("9E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForTermination",
                    NumberType = NumberType.Int
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A1_FieldParsers_Huawei_Iraq()
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

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F814D", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("A9", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                   {
                                        Settings = new NumberFieldParser
                                        {
                                             FieldName = "Called_First_CI",
                                             NumberType = NumberType.Int,
                                             ConvertOutputToString = true
                                        }
                                   }
                            },
                            {"80", new BinaryFieldParser
                                   {
                                        Settings = new NumberFieldParser
                                        {
                                             FieldName = "LocationAreaCode",
                                             NumberType = NumberType.Int,
                                             ConvertOutputToString = true
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("AA", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "Called_Last_CI",
                                          NumberType = NumberType.Int,
                                          ConvertOutputToString = true
                                     }
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("96", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F36", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "ChargeAreaCode",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8112", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F813C", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("94", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("ConnectDateTime")
            });

            parsers.Add("95", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("DisconnectDateTime")
            });

            parsers.Add("9F8150", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8170", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ZoneCode"
                }
            });

            parsers.Add("9F8171", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817E", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F814E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "VoiceIndicator",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F21", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "GlobalCallReference",
                    NumberType = NumberType.BigInt,
                    ConvertOutputToString = true
                }
            });

            parsers.Add("9D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("9B", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForTermination",
                    NumberType = NumberType.Int
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A3_FieldParsers_Huawei_Iraq()
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

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("96", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8112", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F813C", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("87", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("ConnectDateTime")
            });

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("DisconnectDateTime")
            });

            parsers.Add("9F814D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8171", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817C", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Origioi"
                }
            });


            parsers.Add("9F816F", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A4_FieldParsers_Huawei_Iraq()
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

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("96", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8112", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F813C", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("87", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("ConnectDateTime")
            });

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("DisconnectDateTime")
            });

            parsers.Add("9F814D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8171", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817C", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F815B", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A5_FieldParsers_Huawei_Iraq()
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

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("8A", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("97", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "PartialRecordType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F810E", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("ConnectDateTime")
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("DisconnectDateTime")
            });

            parsers.Add("9F814D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8171", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817C", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F827A", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "InputCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8148", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "GlobalCallReference",
                    NumberType = NumberType.BigInt,
                    ConvertOutputToString = true
                }
            });

            parsers.Add("8E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("8C", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForTermination",
                    NumberType = NumberType.Int
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A6_FieldParsers_Huawei_Iraq()
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

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            parsers.Add("A7", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "SAC",
                                          NumberType = NumberType.Int
                                     }
                                }
                            },
                            {"80", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "LocationAreaCode",
                                          NumberType = NumberType.Int,
                                          ConvertOutputToString = true
                                     }
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F821F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SMSType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8160", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TypeOfSubscribers",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F813C", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F815F", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "NetworkOperatorId"
                }
            });

            parsers.Add("9F8241", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallOrigin",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "MessageReference"
                }
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("MessageTime")
            });

            parsers.Add("8C", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "DestinationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("8F", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "LocationExtension"
                }
            });

            parsers.Add("9F8135", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8163", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "UserType",
                    NumberType = NumberType.Int
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A7_FieldParsers_Huawei_Iraq()
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

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            parsers.Add("A7", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "SAC",
                                          NumberType = NumberType.Int
                                     }
                                }
                            },
                            {"80", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "LocationAreaCode",
                                          NumberType = NumberType.Int,
                                          ConvertOutputToString = true
                                     }
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("9F821F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SMSType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8160", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TypeOfSubscribers",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F813C", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F815F", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "NetworkOperatorId"
                }
            });

            parsers.Add("9F8241", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallOrigin",
                    NumberType = NumberType.Int
                }
            });


            parsers.Add("88", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("MessageTime")
            });

            parsers.Add("9F8127", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "OriginatingRNCorBSCId"
                }
            });

            parsers.Add("9F8128", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "OriginatingMSCId"
                }
            });

            parsers.Add("9F8149", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "Origination",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F814A", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("9F814B", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TariffCode",
                    NumberType = NumberType.Int
                }
            });
            parsers.Add("9F816D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargeLevel",
                    NumberType = NumberType.Int
                }
            });
            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A100_FieldParsers_Huawei_Iraq()
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

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("AC", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "Called_First_CI",
                                          NumberType = NumberType.Int,
                                          ConvertOutputToString = true
                                     }
                                }
                            },
                            {"80", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "LocationAreaCode",
                                          NumberType = NumberType.Int,
                                          ConvertOutputToString = true
                                     }
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("AD", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "Called_Last_CI",
                                          NumberType = NumberType.Int,
                                          ConvertOutputToString = true
                                     }
                                }
                            }
                        }
                    }
                }
            });

            parsers.Add("99", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallDuration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F21", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8112", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CalledChargeAreaCode"
                }
            });

            parsers.Add("9F8135", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8104", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F813C", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("97", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("ConnectDateTime")
            });

            parsers.Add("98", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("DisconnectDateTime")
            });

            parsers.Add("9F814D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816E", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "LocationNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8170", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ZoneCode"
                }
            });

            parsers.Add("9F8171", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocationNumberNai",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F817D", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Termioi"
                }
            });

            parsers.Add("9F817E", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "Origioi"
                }
            });

            parsers.Add("9F827A", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "InputCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F810E", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginalCalledNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F26", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "GlobalCallReference",
                    NumberType = NumberType.BigInt,
                    ConvertOutputToString = true
                }
            });

            parsers.Add("9F20", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("9E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForTermination",
                    NumberType = NumberType.Int
                }
            });

            return parsers;
        }

        #endregion

        #region Huawei GPRS

        public string GetHuaweiIraqParserSettings_GPRS()
        {
            BinaryParserType hexParser = new BinaryParserType
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
        private Dictionary<string, BinaryRecordParser> GetTemplateParsers_Huawei_Iraq_GPRS()
        {
            Dictionary<string, BinaryRecordParser> parsers = new Dictionary<string, BinaryRecordParser>();

            parsers.Add("B4", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_B4_FieldParsers_Huawei_Iraq()
                    },
                    RecordType = "Iraq_GPRS",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    {
                        new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    },
                    CompositeFieldsParsers = GetHuaweiIraqCompositeFieldsParser_GPRS()
                }
            });

            return parsers;
        }
        private List<CompositeFieldsParser> GetHuaweiIraqCompositeFieldsParser_GPRS()
        {
            List<CompositeFieldsParser> fieldParsers = new List<CompositeFieldsParser>
            {
                new TimestampDateTimeCompositeParser
                {
                    FieldName = "RecordOpeningTimestamp",
                    DateTimeFieldName = "RecordOpeningTime",
                    DateTimeShift = new DateTime(1970, 01, 01)
                },
                new TimestampDateTimeCompositeParser
                {
                    FieldName = "ChangeTimestamp",
                    DateTimeFieldName = "ChangeTime",
                    DateTimeShift = new DateTime(1970, 01, 01)
                },
                new GuidCompositeParser { FieldName = "UniqueIdentifier" },
                new FileNameCompositeParser { FieldName = "FileName" }
            };

            return fieldParsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_B4_FieldParsers_Huawei_Iraq()
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

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new BoolFieldParser
                {
                    FieldName = "NetworkInitiation"
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

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("A5", new BinaryFieldParser
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
                                          FieldName = "SGSN_Address"
                                     }
                                }
                            },
                            {"82", new BinaryFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "SGSN_Address"
                                     }
                                }
                            },
                            {"81", new BinaryFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "SGSN_Address"
                                     }
                                }
                            },
                            {"83", new BinaryFieldParser
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

            parsers.Add("86", new BinaryFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "MSNetworkCapability",
                    RemoveHexa = true
                }
            });

            parsers.Add("87", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RoutingArea",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocationAreaCode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CellIdentifier",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8A", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CellIdentifier",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AB", new BinaryFieldParser
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
                                          FieldName = "GGSN_Address"
                                     }
                                }
                            },
                            {"82", new BinaryFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "GGSN_Address"
                                     }
                                }
                            },
                            {"81", new BinaryFieldParser
                                {
                                     Settings = new IPv4Parser
                                     {
                                          FieldName = "GGSN_Address"
                                     }
                                }
                            },
                            {"83", new BinaryFieldParser
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

            parsers.Add("8C", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "AccessPointNameNI"
                }
            });

            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "PDPType"
                }
            });

            parsers.Add("90", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("RecordOpeningTime")
            });

            parsers.Add("91", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "Duration",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("92", new BinaryFieldParser
            {
                Settings = new BoolFieldParser
                {
                    FieldName = "SGSN_Change"
                }
            });

            parsers.Add("93", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForRecClosing",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("B4", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"84", new BinaryFieldParser
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

            parsers.Add("95", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordSequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("96", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "NodeID"
                }
            });

            parsers.Add("98", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "LocalSequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("99", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "APNSelectionMode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9A", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "AccessPointNameOI"
                }
            });

            parsers.Add("9B", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    RemoveHexa = true
                }
            });

            parsers.Add("9C", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargingCharacteristics",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SystemType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F1F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RNC_UnsentDownlinkVolume",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F20", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChSelectionMode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F21", new BinaryFieldParser
            {
                Settings = new BoolFieldParser
                {
                    FieldName = "DynamicAddressFlag"
                }
            });

            parsers.Add("AF", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"30", new BinaryFieldParser
                                {
                                      Settings = new SequenceFieldParser
                                        {
                                            FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                                            {
                                                FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                                                {
                                                    {"81", new BinaryFieldParser
                                                        {
                                                             Settings = new HexaParser
                                                             {
                                                                  FieldName = "QosRequested"
                                                             }
                                                        }
                                                    },
                                                    {"82", new BinaryFieldParser
                                                        {
                                                             Settings = new HexaParser
                                                             {
                                                                  FieldName = "QosNegotiated"
                                                             }
                                                        }
                                                    },
                                                     {"83", new BinaryFieldParser
                                                        {
                                                             Settings = new NumberFieldParser
                                                             {
                                                                  FieldName = "DataVolumeGPRSUplink",
                                                                  NumberType = NumberType.Int
                                                             }
                                                        }
                                                    },
                                                    {"84", new BinaryFieldParser
                                                        {
                                                             Settings = new NumberFieldParser
                                                             {
                                                                  FieldName = "DataVolumeGPRSDownlink",
                                                                  NumberType = NumberType.Int
                                                             }
                                                        }
                                                    },
                                                    {"85", new BinaryFieldParser
                                                        {
                                                             Settings = new NumberFieldParser
                                                             {
                                                                  FieldName = "ChangeCondition",
                                                                  NumberType = NumberType.Int
                                                             }
                                                        }
                                                    },
                                                    {"86", new BinaryFieldParser
                                                        {
                                                             Settings = new DateTimeParser
                                                             {
                                                                  FieldName = "ChangeTime",
                                                                  DateTimeParsingType = DateTimeParsingType.DateTime,
                                                                  WithOffset = true,
                                                                  IsBCD = true
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

            parsers.Add("BE", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                {
                                     Settings = new HexaParser
                                     {
                                          FieldName = "SCFAddress"
                                     }
                                }
                            },
                            {"82", new BinaryFieldParser
                                {
                                     Settings = new HexaParser
                                     {
                                          FieldName = "ServiceKey"
                                     }
                                }
                            },
                            {"83", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "DefaultTransactionHandling",
                                          NumberType = NumberType.Int
                                     }
                                }
                            },
                            {"84", new BinaryFieldParser
                                {
                                     Settings = new StringParser
                                     {
                                          FieldName = "CamelAccessPointNameNI"
                                     }
                                }
                            },
                            {"85", new BinaryFieldParser
                                {
                                     Settings = new StringParser
                                     {
                                          FieldName = "CamelAccessPointNameOI"
                                     }
                                }
                            },
                            {"86", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "NumberOfDPEncountered",
                                          NumberType = NumberType.Int
                                     }
                                }
                            },
                            {"87", new BinaryFieldParser
                                {
                                     Settings = new StringParser
                                     {
                                          FieldName = "LevelOfCamelService"
                                     }
                                }
                            },
                            {"88", new BinaryFieldParser
                                {
                                     Settings = new HexaParser
                                     {
                                          FieldName = "FreeFormatData"
                                     }
                                }
                            },
                            {"89", new BinaryFieldParser
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

        #endregion

        #region Nokia 

        public string GetNokiaParserSettings()
        {
            BinaryParserType hexParser = new BinaryParserType
            {
                RecordParser = new SplitByBlockRecordParser
                {
                    RecordParser = new BinaryRecordParser
                    {
                        Settings = new SplitByOffSetRecordParser
                        {
                            LengthNbOfBytes = 2,
                            RecordParser = new BinaryRecordParser
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
        private Dictionary<string, BinaryRecordParser> GetNokiaSubRecordsParsers()
        {
            Dictionary<string, BinaryRecordParser> recordParsers = new Dictionary<string, BinaryRecordParser>();

            recordParsers.Add("1", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_MOC_Call(),
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
                            Value = 3
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordTypeName",
                            Value = "MobileOriginated"
                        }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
                    RecordType = "Iraq_Mobile_CDR"
                }
            });

            recordParsers.Add("2", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_MOT_Call(),
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
                            Value = 3
                        },
                        new ParsedRecordFieldConstantValue
                        {
                             FieldName = "RecordTypeName",
                             Value = "MobileTerminated"
                        }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
                    RecordType = "Iraq_Mobile_CDR"
                }
            });

            recordParsers.Add("3", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_Forward_Call(),
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
                            Value = 3
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordTypeName",
                            Value = "CallForwarding"
                        }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
                    RecordType = "Iraq_Mobile_CDR"
                }
            });

            recordParsers.Add("4", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_Roaming_Call(),
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
                            Value = 3
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordTypeName",
                            Value = "Roaming"
                        }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
                    RecordType = "Iraq_Mobile_CDR"
                }
            });

            //recordParsers.Add("17", new BinaryRecordParser
            //{
            //    Settings = new PositionedFieldsRecordParser
            //    {
            //        FieldParsers = Get_PositionedFieldParsers_PSTN_Originated_Call(),
            //        FieldConstantValues = new List<ParsedRecordFieldConstantValue>
            //        {
            //            new ParsedRecordFieldConstantValue
            //            {
            //                FieldName = "RecordType",
            //                Value = 17
            //            },
            //            new ParsedRecordFieldConstantValue
            //            {
            //                FieldName = "SwitchId",
            //                Value = 3
            //            },
            //            new ParsedRecordFieldConstantValue
            //            {
            //                FieldName = "RecordTypeName",
            //                Value = "PSTN_Originated"
            //            }
            //        },
            //        CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
            //        RecordType = "Iraq_Mobile_CDR"
            //    }
            //});

            //recordParsers.Add("18", new BinaryRecordParser
            //{
            //    Settings = new PositionedFieldsRecordParser
            //    {
            //        FieldParsers = Get_PositionedFieldParsers_PSTN_Terminated_Call(),
            //        FieldConstantValues = new List<ParsedRecordFieldConstantValue>
            //        {
            //            new ParsedRecordFieldConstantValue
            //            {
            //                FieldName = "RecordType",
            //                Value = 18
            //            },
            //            new ParsedRecordFieldConstantValue
            //            {
            //                FieldName = "SwitchId",
            //                Value = 3
            //            },
            //            new ParsedRecordFieldConstantValue
            //            {
            //                FieldName = "RecordTypeName",
            //                Value = "PSTN_Terminated"
            //            }
            //        },
            //        CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
            //        RecordType = "Iraq_Mobile_CDR"
            //    }
            //});

            recordParsers.Add("19", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_PBX_Originated_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    {
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordType",
                            Value = 19
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "SwitchId",
                            Value = 3
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordTypeName",
                            Value = "PBX_Originated"
                        }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
                    RecordType = "Iraq_Mobile_CDR"
                }
            });

            recordParsers.Add("20", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_PBX_Terminated_Call(),
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue>
                    {
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordType",
                            Value = 21
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "SwitchId",
                            Value = 3
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordTypeName",
                            Value = "PBX_Terminated"
                        }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_Nokia(),
                    RecordType = "Iraq_Mobile_CDR"
                }
            });

            recordParsers.Add("8", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_SMMO(),
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
                            Value = 3
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordTypeName",
                            Value = "SMSOriginated"
                        }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_SMS_Nokia(),
                    RecordType = "Iraq_SMS"
                }
            });

            recordParsers.Add("9", new BinaryRecordParser
            {
                Settings = new PositionedFieldsRecordParser
                {
                    FieldParsers = Get_PositionedFieldParsers_SMMT(),
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
                            Value = 3
                        },
                        new ParsedRecordFieldConstantValue
                        {
                            FieldName = "RecordTypeName",
                            Value = "SMSTerminated"
                        }
                    },
                    CompositeFieldsParsers = GetCompositeFieldParsers_SMS_Nokia(),
                    RecordType = "Iraq_SMS"
                }
            });

            return recordParsers;
        }
        private List<CompositeFieldsParser> GetCompositeFieldParsers_Nokia()
        {
            return new List<CompositeFieldsParser>()
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
                       },
                       new TimestampDateTimeCompositeParser
                       {
                            FieldName = "SetupTimestamp",
                            DateTimeFieldName = "SetupTime",
                            DateTimeShift = new DateTime(1970, 01, 01)
                       },
                       new GuidCompositeParser
                       {
                            FieldName = "UniqueIdentifier"
                       },
                       new FileNameCompositeParser
                       {
                            FieldName = "FileName"
                       }
                   };
        }
        private List<CompositeFieldsParser> GetCompositeFieldParsers_SMS_Nokia()
        {
            return new List<CompositeFieldsParser>()
                   {
                       new TimestampDateTimeCompositeParser
                       {
                           FieldName = "MessageTimestamp",
                           DateTimeFieldName = "MessageTime",
                           DateTimeShift = new DateTime(1970, 01, 01)
                       },
                       new GuidCompositeParser
                       {
                           FieldName = "UniqueIdentifier"
                       },
                       new FileNameCompositeParser
                       {
                           FieldName = "FileName"
                       }
                   };
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_MOC_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Sum(s => (decimal)s.Position);

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 204
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 136
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 143
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "CallDuration"
                    }
                },
                Length = 3,
                Position = 156
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "Calling_First_LAC",
                        Reverse = true
                    }
                },
                Length = 2,
                Position = 99
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "Calling_Last_LAC",
                        Reverse = true
                    }
                },
                Length = 2,
                Position = 113
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "Calling_First_CI",
                        Reverse = true,
                        ConvertOutputToString = true
                    }
                },
                Length = 2,
                Position = 101
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "Calling_Last_CI",
                        Reverse = true,
                        ConvertOutputToString = true
                    }
                },
                Length = 2,
                Position = 115
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "CallType"
                    }
                },
                Length = 1,
                Position = 154
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateChargingIndicator"
                    }
                },
                Length = 1,
                Position = 26
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CallReference",
                        NumberType = NumberType.BigInt
                    }
                },
                Length = 5,
                Position = 10
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateRecordNumber"
                    }
                },
                Length = 1,
                Position = 25
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new HexaParser
                    {
                        FieldName = "GlobalCallReference"
                    }
                },
                Length = 21,
                Position = 252
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_MOT_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 166
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 105
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 112
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "CallDuration"
                    }
                },
                Length = 3,
                Position = 125
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "Called_First_LAC",
                        Reverse = true
                    }
                },
                Length = 2,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "Called_Last_LAC",
                        Reverse = true
                    }
                },
                Length = 2,
                Position = 86
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "Called_First_CI",
                        Reverse = true,
                        ConvertOutputToString = true
                    }
                },
                Length = 2,
                Position = 74
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "Called_Last_CI",
                        Reverse = true,
                        ConvertOutputToString = true
                    }
                },
                Length = 2,
                Position = 88
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "CallType"
                    }
                },
                Length = 1,
                Position = 123
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateChargingIndicator"
                    }
                },
                Length = 1,
                Position = 26
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CallReference",
                        NumberType = NumberType.BigInt
                    }
                },
                Length = 5,
                Position = 10
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateRecordNumber"
                    }
                },
                Length = 1,
                Position = 25
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new HexaParser
                    {
                        FieldName = "GlobalCallReference"
                    }
                },
                Length = 21,
                Position = 214
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_Forward_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
                {
                    Settings = new TBCDNumberParser
                    {
                        RemoveHexa = true,
                        FieldName = "CalledNumber"
                    }
                },
                Length = 12,
                Position = 65
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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

            //fieldParsers.Add(new PositionedFieldParser
            //{
            //    FieldParser = new HexTLVFieldParser
            //    {
            //        Settings = new TBCDNumberParser
            //        {
            //            RemoveHexa = true,
            //            FieldName = "CallingIMSI"
            //        }
            //    },
            //    Length = 8,
            //    Position = 29
            //});

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 164
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 111
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 118
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "CallDuration"
                    }
                },
                Length = 3,
                Position = 132
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "CallType"
                    }
                },
                Length = 1,
                Position = 129
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateChargingIndicator"
                    }
                },
                Length = 1,
                Position = 26
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CallReference",
                        NumberType = NumberType.BigInt
                    }
                },
                Length = 5,
                Position = 10
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateRecordNumber"
                    }
                },
                Length = 1,
                Position = 25
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new HexaParser
                    {
                        FieldName = "GlobalCallReference"
                    }
                },
                Length = 21,
                Position = 201
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_Roaming_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 136
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 91
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 98
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "CallDuration"
                    }
                },
                Length = 3,
                Position = 111
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "CallType"
                    }
                },
                Length = 1,
                Position = 109
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateChargingIndicator"
                    }
                },
                Length = 1,
                Position = 26
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CallReference",
                        NumberType = NumberType.BigInt
                    }
                },
                Length = 5,
                Position = 10
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateRecordNumber"
                    }
                },
                Length = 1,
                Position = 25
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new HexaParser
                    {
                        FieldName = "GlobalCallReference"
                    }
                },
                Length = 21,
                Position = 173
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_PSTN_Originated_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 118
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 65
            });
            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 72
            });
            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "CallDuration"
                    }
                },
                Length = 3,
                Position = 86
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "CallType"
                    }
                },
                Length = 1,
                Position = 83
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateChargingIndicator"
                    }
                },
                Length = 1,
                Position = 26
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CallReference",
                        NumberType = NumberType.BigInt
                    }
                },
                Length = 5,
                Position = 10
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateRecordNumber"
                    }
                },
                Length = 1,
                Position = 25
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new HexaParser
                    {
                        FieldName = "GlobalCallReference"
                    }
                },
                Length = 21,
                Position = 146
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_PSTN_Terminated_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 123
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 65
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "CallDuration"
                    }
                },
                Length = 3,
                Position = 86
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "CallType"
                    }
                },
                Length = 1,
                Position = 83
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateChargingIndicator"
                    }
                },
                Length = 1,
                Position = 26
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CallReference",
                        NumberType = NumberType.BigInt
                    }
                },
                Length = 5,
                Position = 10
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateRecordNumber"
                    }
                },
                Length = 1,
                Position = 25
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new HexaParser
                    {
                        FieldName = "GlobalCallReference"
                    }
                },
                Length = 21,
                Position = 133
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_PBX_Originated_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 115
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 65
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "CallDuration"
                    }
                },
                Length = 3,
                Position = 85
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "CallType"
                    }
                },
                Length = 1,
                Position = 83
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateChargingIndicator"
                    }
                },
                Length = 1,
                Position = 26
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CallReference",
                        NumberType = NumberType.BigInt
                    }
                },
                Length = 5,
                Position = 10
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateRecordNumber"
                    }
                },
                Length = 1,
                Position = 25
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new HexaParser
                    {
                        FieldName = "GlobalCallReference"
                    }
                },
                Length = 21,
                Position = 143
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_PBX_Terminated_Call()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 100
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 65
            });


            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 72
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        RemoveHexa = true,
                        Reverse = true,
                        FieldName = "CallDuration"
                    }
                },
                Length = 3,
                Position = 85
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "CallType"
                    }
                },
                Length = 1,
                Position = 83
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateChargingIndicator"
                    }
                },
                Length = 1,
                Position = 26
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CallReference",
                        NumberType = NumberType.BigInt
                    }
                },
                Length = 5,
                Position = 10
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IntermediateRecordNumber"
                    }
                },
                Length = 1,
                Position = 25
            });

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
                {
                    Settings = new HexaParser
                    {
                        FieldName = "GlobalCallReference"
                    }
                },
                Length = 21,
                Position = 110
            });

            return fieldParsers;
        }
        private List<PositionedFieldParser> Get_PositionedFieldParsers_SMMO()
        {
            List<PositionedFieldParser> fieldParsers = new List<PositionedFieldParser>();

            fieldParsers.Add(new PositionedFieldParser
            {
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                FieldParser = new BinaryFieldParser
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
                        WithOffset = false,
                        IsBCD = true
                    }
                },
                Length = 7,
                Position = 105
            });


            return fieldParsers;
        }

        #endregion

        #endregion

        #region Mobilis

        #region Ericsson

        public string GetEricssonMobilisParserSettings()
        {
            BinaryParserType hexParser = new BinaryParserType
            {
                RecordParser = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_A0_SubRecordsParsersByTag_EricssonMobilis()
                },
                RecordParserTemplates = GetTemplates_EricssonMobilis()
            };

            ParserType parserType = new ParserType
            {
                ParserTypeId = new Guid("57E3E68E-9403-440D-A67D-CC5896D6BAD5"),
                Settings = new ParserTypeSettings
                {
                    ExtendedSettings = hexParser
                }
            };

            return Serializer.Serialize(parserType.Settings);
        }

        private Dictionary<Guid, BinaryRecordParser> GetTemplates_EricssonMobilis()
        {
            Dictionary<Guid, BinaryRecordParser> templates = new Dictionary<Guid, BinaryRecordParser>();

            templates.Add(new Guid("6BF125E7-BC7B-4C98-8B23-B8992569A1E1"), new BinaryRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = GetTemplateParsers_EricssonMobilis()
                }
            });

            return templates;
        }
        private Dictionary<string, BinaryRecordParser> GetTemplateParsers_EricssonMobilis()
        {
            Dictionary<string, BinaryRecordParser> parsers = new Dictionary<string, BinaryRecordParser>();

            parsers.Add("A0", new BinaryRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_Template_CreateRecordsParsersByTag_EricssonMobilis()
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryRecordParser> Get_A0_SubRecordsParsersByTag_EricssonMobilis()
        {
            Dictionary<string, BinaryRecordParser> subParser = new Dictionary<string, BinaryRecordParser>();

            subParser.Add("A0", new BinaryRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_Template_CreateRecordsParsersByTag_EricssonMobilis()
                }
            });

            subParser.Add("A1", new BinaryRecordParser
            {
                Settings = new ExecuteTemplateRecordParser
                {
                    RecordParserTemplateId = new Guid("6BF125E7-BC7B-4C98-8B23-B8992569A1E1")
                }
            });

            return subParser;
        }
        private Dictionary<string, BinaryRecordParser> Get_Template_CreateRecordsParsersByTag_EricssonMobilis()
        {
            Dictionary<string, BinaryRecordParser> subParser = new Dictionary<string, BinaryRecordParser>();

            subParser.Add("A1", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_A1_MSOriginating_FieldParsers_EricssonMobilis()
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
                            FieldName = "RecordTypeName",
                            Value = "MSOriginating"
                        }
                    },
                    RecordType = "Mobilis_Ericsson_CDR",
                    TempFieldsNames = GetTempFieldsName_EricssonMobilis(),
                    CompositeFieldsParsers = GetCompositeFieldParsers_EricssonMobilis()
                }
            });

            subParser.Add("A4", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_A4_MSTerminating_FieldParsers_EricssonMobilis()
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
                            FieldName = "RecordTypeName",
                            Value = "MSTerminating"
                        }
                    },
                    RecordType = "Mobilis_Ericsson_CDR",
                    TempFieldsNames = GetTempFieldsName_EricssonMobilis(),
                    CompositeFieldsParsers = GetCompositeFieldParsers_EricssonMobilis()
                }
            });

            subParser.Add("A5", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_A5_MSOriginatingSMS_FieldParsers_EricssonMobilis()
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
                            FieldName = "RecordTypeName",
                            Value = "MSOriginatingSMSinMSC"
                        }
                    },
                    RecordType = "Mobilis_Ericsson_SMS",
                    CompositeFieldsParsers = GetCompositeFieldParsers_SMS_EricssonMobilis()
                }
            });

            subParser.Add("A7", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_A7_MSTerminatingSMS_FieldParsers_EricssonMobilis()
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
                            FieldName = "RecordTypeName",
                            Value = "MSTerminatingSMSinMSC"
                        }
                    },
                    RecordType = "Mobilis_Ericsson_SMS",
                    CompositeFieldsParsers = GetCompositeFieldParsers_SMS_EricssonMobilis()
                }
            });

            return subParser;
        }

        private HashSet<string> GetTempFieldsName_EricssonMobilis()
        {
            return new HashSet<string> { "DateForStartOfCharge", "TimeForStartOfCharge", "TimeForStopOfCharge", "TimeFromRegisterSeizureToStartOfCharging" };
        }
        private List<CompositeFieldsParser> GetCompositeFieldParsers_EricssonMobilis()
        {
            return new List<CompositeFieldsParser>
            {
                new FileNameCompositeParser { FieldName = "FileName" },
                new DataSourceCompositeParser { DataSourceFieldName = "DataSourceId" },
                new DateTimeCompositeParser { FieldName = "ConnectDateTime", DateFieldName = "DateForStartOfCharge", TimeFieldName = "TimeForStartOfCharge" },
                new DateTimeCompositeParser { FieldName = "DisconnectDateTime", DateFieldName = "DateForStartOfCharge", TimeFieldName = "TimeForStopOfCharge"},
                new DateTimeCompositeParser { FieldName = "SetupTime", DateFieldName = "ConnectDateTime", TimeFieldName = "TimeFromRegisterSeizureToStartOfCharging", SubtractTime = true },
            };
        }
        private List<CompositeFieldsParser> GetCompositeFieldParsers_SMS_EricssonMobilis()
        {
            return new List<CompositeFieldsParser>
            {
                 new FileNameCompositeParser { FieldName = "FileName" },
                 new DataSourceCompositeParser { DataSourceFieldName = "DataSourceId" }
            };
        }

        private Dictionary<string, BinaryFieldParser> Get_A1_MSOriginating_FieldParsers_EricssonMobilis()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallIdentificationNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingPartyNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingSubscriberIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingSubscriberIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("87", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledPartyNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "DisconnectingParty",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "DateForStartOfCharge",
                    DateTimeParsingType = DateTimeParsingType.Date,
                    WithOffset = false,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2
                }
            });

            parsers.Add("8A", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "TimeForStartOfCharge"
                }
            });

            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "TimeForStopOfCharge"
                }
            });

            parsers.Add("8C", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "ChargeableDurationInSeconds"
                }
            });

            parsers.Add("8E", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "TimeFromRegisterSeizureToStartOfCharging"
                }
            });

            parsers.Add("96", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OutgoingRoute"
                }
            });

            parsers.Add("97", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "IncomingRoute"
                }
            });

            parsers.Add("9A", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "TimeForTCSeizureCalling"
                }
            });

            parsers.Add("9F44", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "NetworkCallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("9F810F", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingSubscriberIMEISV",
                    RemoveHexa = true
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A4_MSTerminating_FieldParsers_EricssonMobilis()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallIdentificationNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordSequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingPartyNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledPartyNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledSubscriberIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("87", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledSubscriberIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "MobileStationRoamingNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "DisconnectingParty",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8A", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "DateForStartOfCharge",
                    DateTimeParsingType = DateTimeParsingType.Date,
                    WithOffset = false,
                    YearIndex = 0,
                    MonthIndex = 1,
                    DayIndex = 2
                }
            });

            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "TimeForStartOfCharge"
                }
            });

            parsers.Add("8C", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "TimeForStopOfCharge"
                }
            });

            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "ChargeableDurationInSeconds"
                }
            });

            parsers.Add("8F", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "TimeFromRegisterSeizureToStartOfCharging"
                }
            });

            parsers.Add("96", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OutgoingRoute"
                }
            });

            parsers.Add("97", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "IncomingRoute"
                }
            });

            parsers.Add("9A", new BinaryFieldParser
            {
                Settings = new TimeParser
                {
                    FieldName = "TimeForTCSeizureCalled"
                }
            });

            parsers.Add("9F74", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledSubscriberIMEISV",
                    RemoveHexa = true
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A5_MSOriginatingSMS_FieldParsers_EricssonMobilis()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallIdentificationNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordSequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingPartyNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingSubscriberIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingSubscriberIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("87", new BinaryFieldParser
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

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "MessageTime",
                    DateTimeParsingType = DateTimeParsingType.Time,
                    WithOffset = false,
                    HoursIndex = 0,
                    MinutesIndex = 1,
                    SecondsIndex = 2
                }
            });

            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "IncomingRoute"
                }
            });

            parsers.Add("91", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CallingSubscriberIMEISV",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F20", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "DestinationAddress",
                    RemoveHexa = true
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A7_MSTerminatingSMS_FieldParsers_EricssonMobilis()
        {
            Dictionary<string, BinaryFieldParser> parsers = new Dictionary<string, BinaryFieldParser>();

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallIdentificationNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordSequenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledPartyNumber",
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledSubscriberIMSI",
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledSubscriberIMEI",
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new BinaryFieldParser
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

            parsers.Add("87", new BinaryFieldParser
            {
                Settings = new DateTimeParser
                {
                    FieldName = "MessageTime",
                    DateTimeParsingType = DateTimeParsingType.Time,
                    WithOffset = false,
                    HoursIndex = 0,
                    MinutesIndex = 1,
                    SecondsIndex = 2
                }
            });

            parsers.Add("8C", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OutgoingRoute"
                }
            });

            parsers.Add("9F21", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "OriginatingAddress",
                    RemoveHexa = true
                }
            });

            parsers.Add("9F2B", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CalledSubscriberIMEISV",
                    RemoveHexa = true
                }
            });

            return parsers;
        }

        #endregion

        #region Huawei 

        public string GetHuaweiMobilis_ParserSettings()
        {
            BinaryParserType hexParser = new BinaryParserType
            {
                RecordParser = new PositionedBlockRecordParser()
                {
                    BlockSize = 154,
                    RecordParser = new BinaryRecordParser()
                    {
                        Settings = new PositionedFieldsRecordParser()
                        {
                            RecordType = "Mobilis_Huawei_CDR",
                            FieldParsers = GetMobilisHuaweiPositionedFieldParsers(),
                            CompositeFieldsParsers = GetMobilisHuaweiCompositeParsers(),
                            ZeroBytesBlockAction = ZeroBytesBlockAction.Skip
                        }
                    }
                }
            };

            ParserType parserType = new ParserType
            {
                ParserTypeId = new Guid("9B613038-9F64-47C7-B9FA-2F41ABE85286"),
                Settings = new ParserTypeSettings { ExtendedSettings = hexParser }
            };

            return Serializer.Serialize(parserType.Settings);
        }
        private List<PositionedFieldParser> GetMobilisHuaweiPositionedFieldParsers()
        {
            List<PositionedFieldParser> positionedFieldParsers = new List<PositionedFieldParser>();

            PositionedFieldParser serialNumber = new PositionedFieldParser
            {
                Position = 0,
                Length = 4,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "SerialNumber",
                        NumberType = NumberType.BigInt
                    }
                }
            };
            positionedFieldParsers.Add(serialNumber);

            PositionedFieldParser ticketType = new PositionedFieldParser
            {
                Position = 4,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "TicketType",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(ticketType);

            PositionedFieldParser checkSum = new PositionedFieldParser
            {
                Position = 5,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CheckSum",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(checkSum);

            PositionedFieldParser partialRecordIndicator = new PositionedFieldParser
            {
                Position = 6,
                Length = 2,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new PositionedBitsBinaryFieldParser
                    {
                        BitParsers = new List<PositionedBitFieldParser>
                        {
                             new PositionedBitFieldParser{
                                Position = 0,
                                Length = 2,
                                FieldParser = new BitFieldParser
                                {
                                    Settings = new NumberBitFieldParser
                                    {
                                        FieldName = "PartialRecordIndicator"
                                    }
                                }
                             },
                             new PositionedBitFieldParser {
                                Position = 2,
                                Length = 1,
                                FieldParser = new BitFieldParser
                                {
                                    Settings = new BoolBitFieldParser
                                    {
                                        FieldName = "ClockChangedFlag"
                                    }
                                }
                             },
                             new PositionedBitFieldParser{
                                 Position = 3,
                                 Length = 1,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new NumberBitFieldParser
                                     {
                                         FieldName = "FreeFlag"
                                     }
                                 }
                             },
                             new PositionedBitFieldParser{
                                 Position = 4,
                                 Length = 1,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new NumberBitFieldParser
                                     {
                                         FieldName = "Validity"
                                     }
                                 }
                             },
                             new PositionedBitFieldParser {
                                 Position = 5,
                                 Length = 1,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new NumberBitFieldParser
                                     {
                                         FieldName = "CallAttemptFlag"
                                     }
                                 }
                             },
                             new PositionedBitFieldParser{
                                 Position = 6,
                                 Length = 1,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new NumberBitFieldParser
                                     {
                                         FieldName = "ComplaintFlag"
                                     }
                                 }
                             },
                             new PositionedBitFieldParser{
                                 Position = 7,
                                 Length = 1,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new BoolBitFieldParser
                                     {
                                         FieldName = "CentralizedChargingFlag"
                                     }
                                 }
                             },
                             new PositionedBitFieldParser{
                                 Position = 8,
                                 Length = 1,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new BoolBitFieldParser
                                     {
                                         FieldName = "PPSFlag"
                                     }
                                 }
                             },
                             new PositionedBitFieldParser{
                                 Position =9,
                                 Length = 2,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new NumberBitFieldParser
                                     {
                                         FieldName = "ChargingMethod"
                                     }
                                 }
                             },
                             new PositionedBitFieldParser{
                                 Position = 11,
                                 Length = 1,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new BoolBitFieldParser
                                     {
                                         FieldName = "NPCallFlag"
                                     }
                                 }
                             },
                             new PositionedBitFieldParser{
                                 Position = 12,
                                 Length = 4,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new NumberBitFieldParser
                                     {
                                         FieldName = "Payer"
                                     }
                                 }
                             }
                        }
                    }
                }
            };
            positionedFieldParsers.Add(partialRecordIndicator);

            PositionedFieldParser conversationEndTime = new PositionedFieldParser
            {
                Position = 8,
                Length = 6,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new DateTimeParser
                    {
                        DateTimeParsingType = DateTimeParsingType.DateTime,
                        FieldName = "ConversationEndTime",
                        YearIndex = 0,
                        MonthIndex = 1,
                        DayIndex = 2,
                        HoursIndex = 3,
                        MinutesIndex = 4,
                        SecondsIndex = 5
                    }
                }
            };
            positionedFieldParsers.Add(conversationEndTime);

            PositionedFieldParser conversationDuration = new PositionedFieldParser
            {
                Position = 14,
                Length = 4,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.BigInt,
                        FieldName = "ConversationDuration"
                    }
                }
            };
            positionedFieldParsers.Add(conversationDuration);

            PositionedFieldParser callerSeizureDuration = new PositionedFieldParser
            {
                Position = 18,
                Length = 4,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.BigInt,
                        FieldName = "CallerSeizureDuration"
                    }
                }
            };
            positionedFieldParsers.Add(callerSeizureDuration);

            PositionedFieldParser calledSeizureDuration = new PositionedFieldParser
            {
                Position = 22,
                Length = 4,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.BigInt,
                        FieldName = "CalledSeizureDuration"
                    }
                }
            };
            positionedFieldParsers.Add(calledSeizureDuration);

            PositionedFieldParser incompleteCallWatch = new PositionedFieldParser
            {
                Position = 26,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new PositionedBitsBinaryFieldParser
                    {
                        BitParsers = new List<PositionedBitFieldParser>
                        {
                             new PositionedBitFieldParser{
                                 Position = 0,
                                 Length = 2,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new NumberBitFieldParser
                                     {
                                         FieldName = "IncompleteCallWatch"
                                     }
                                 }
                             },
                             new PositionedBitFieldParser{
                                 Position = 2,
                                 Length = 1,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new BoolBitFieldParser
                                     {
                                         FieldName = "CallerISDNAccess"
                                     }
                                 }
                             },
                             new PositionedBitFieldParser{
                                 Position = 3,
                                 Length = 1,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new BoolBitFieldParser
                                     {
                                         FieldName = "CalledISDNAccess"
                                     }
                                 }
                             },
                             new PositionedBitFieldParser{
                                 Position =4,
                                 Length = 1,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new BoolBitFieldParser
                                     {
                                         FieldName = "ISUPIndication"
                                     }
                                 }
                             }
                        }
                    }
                }
            };
            positionedFieldParsers.Add(incompleteCallWatch);

            PositionedFieldParser numberAddressNature = new PositionedFieldParser
            {
                Position = 27,
                Length = 2,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new PositionedBitsBinaryFieldParser
                    {
                        BitParsers = new List<PositionedBitFieldParser>
                        {
                            new PositionedBitFieldParser{
                                Position = 0,
                                Length = 4,
                                FieldParser = new BitFieldParser
                                {
                                    Settings = new NumberBitFieldParser
                                    {
                                        FieldName = "ChargingNumberAddressNature"
                                    }
                                }
                            },
                            new PositionedBitFieldParser{
                                Position = 4,
                                Length = 4,
                                FieldParser = new BitFieldParser
                                {
                                    Settings = new NumberBitFieldParser
                                    {
                                        FieldName = "CallerNumberAddressNature"
                                    }
                                }
                            },
                            new PositionedBitFieldParser {
                                Position = 8,
                                Length = 4,
                                FieldParser = new BitFieldParser
                                {
                                    Settings = new NumberBitFieldParser
                                    {
                                        FieldName = "ConnectedNumberAddressNature"
                                    }
                                }
                            },
                            new PositionedBitFieldParser{
                                Position = 12,
                                Length = 4,
                                FieldParser = new BitFieldParser
                                {
                                    Settings = new NumberBitFieldParser
                                    {
                                        FieldName = "CalledNumberAddressNature"
                                    }
                                }
                            }
                        }
                    }
                }
            };
            positionedFieldParsers.Add(numberAddressNature);

            PositionedFieldParser chargingNumberDNSet = new PositionedFieldParser
            {
                Position = 29,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "ChargingNumberDNSet",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(chargingNumberDNSet);

            PositionedFieldParser chargingNumber = new PositionedFieldParser
            {
                Position = 30,
                Length = 10,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        FieldName = "ChargingNumber",
                        RemoveHexa = true
                    }
                }
            };
            positionedFieldParsers.Add(chargingNumber);

            PositionedFieldParser callerNumberDNSet = new PositionedFieldParser
            {
                Position = 40,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CallerNumberDNSet",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(callerNumberDNSet);

            PositionedFieldParser callerNumber = new PositionedFieldParser
            {
                Position = 41,
                Length = 10,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        FieldName = "CallerNumber",
                        RemoveHexa = true
                    }
                }
            };
            positionedFieldParsers.Add(callerNumber);

            PositionedFieldParser connectedNumberDNSet = new PositionedFieldParser
            {
                Position = 51,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "ConnectedNumberDNSet",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(connectedNumberDNSet);

            PositionedFieldParser connectedNumber = new PositionedFieldParser
            {
                Position = 52,
                Length = 10,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        FieldName = "ConnectedNumber",
                        RemoveHexa = true
                    }
                }
            };
            positionedFieldParsers.Add(connectedNumber);

            PositionedFieldParser calledNumberDNSet = new PositionedFieldParser
            {
                Position = 62,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CalledNumberDNSet",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(calledNumberDNSet);

            PositionedFieldParser calledNumber = new PositionedFieldParser
            {
                Position = 63,
                Length = 10,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        FieldName = "CalledNumber",
                        RemoveHexa = true
                    }
                }
            };
            positionedFieldParsers.Add(calledNumber);

            PositionedFieldParser dialedNumber = new PositionedFieldParser
            {
                Position = 73,
                Length = 12,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new BCDNumberParser
                    {
                        FieldName = "DialedNumber",
                        RemoveHexa = true
                    }
                }
            };
            positionedFieldParsers.Add(dialedNumber);

            PositionedFieldParser centrexGroupNumber = new PositionedFieldParser
            {
                Position = 85,
                Length = 2,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CentrexGroupNumber",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(centrexGroupNumber);

            PositionedFieldParser callerCentrexShortNumber = new PositionedFieldParser
            {
                Position = 87,
                Length = 4,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CallerCentrexShortNumber",
                        NumberType = NumberType.BigInt
                    }
                }
            };
            positionedFieldParsers.Add(callerCentrexShortNumber);

            PositionedFieldParser calledCentrexShortNumber = new PositionedFieldParser
            {
                Position = 91,
                Length = 4,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CalledCentrexShortNumber",
                        NumberType = NumberType.BigInt
                    }
                }
            };
            positionedFieldParsers.Add(calledCentrexShortNumber);

            PositionedFieldParser callerModuleNumber = new PositionedFieldParser
            {
                Position = 95,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CallerModuleNumber",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(callerModuleNumber);

            PositionedFieldParser calledModuleNumber = new PositionedFieldParser
            {
                Position = 96,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CalledModuleNumber",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(calledModuleNumber);

            PositionedFieldParser incomingTrunkGroupNumber = new PositionedFieldParser
            {
                Position = 97,
                Length = 2,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "IncomingTrunkGroupNumber"
                    }
                }
            };
            positionedFieldParsers.Add(incomingTrunkGroupNumber);

            PositionedFieldParser outgoingTrunkGroupNumber = new PositionedFieldParser
            {
                Position = 99,
                Length = 2,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        NumberType = NumberType.Int,
                        FieldName = "OutgoingTrunkGroupNumber"
                    }
                }
            };
            positionedFieldParsers.Add(outgoingTrunkGroupNumber);

            PositionedFieldParser incomingSubrouteNumber = new PositionedFieldParser
            {
                Position = 101,
                Length = 2,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "IncomingSubrouteNumber",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(incomingSubrouteNumber);

            PositionedFieldParser outgoingSubrouteNumber = new PositionedFieldParser
            {
                Position = 103,
                Length = 2,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "OutgoingSubrouteNumber",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(outgoingSubrouteNumber);

            PositionedFieldParser callerDeviceType = new PositionedFieldParser
            {
                Position = 105,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CallerDeviceType",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(callerDeviceType);

            PositionedFieldParser calledDeviceType = new PositionedFieldParser
            {
                Position = 106,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CalledDeviceType",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(calledDeviceType);

            PositionedFieldParser callerPortNumber = new PositionedFieldParser
            {
                Position = 107,
                Length = 2,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CallerPortNumber",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(callerPortNumber);

            PositionedFieldParser calledPortNumber = new PositionedFieldParser
            {
                Position = 109,
                Length = 2,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CalledPortNumber",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(calledPortNumber);

            PositionedFieldParser callerCategory = new PositionedFieldParser
            {
                Position = 111,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CallerCategory",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(callerCategory);

            PositionedFieldParser calledCategory = new PositionedFieldParser
            {
                Position = 112,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "CalledCategory",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(calledCategory);

            PositionedFieldParser callType = new PositionedFieldParser
            {
                Position = 113,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new PositionedBitsBinaryFieldParser
                    {
                        BitParsers = new List<PositionedBitFieldParser>
                        {
                            new PositionedBitFieldParser{
                                Position = 0,
                                Length = 4,
                                FieldParser = new BitFieldParser
                                {
                                    Settings = new NumberBitFieldParser
                                    {
                                        FieldName = "CallType"
                                    }
                                }
                            },
                            new PositionedBitFieldParser{
                                Position = 4,
                                Length = 4,
                                FieldParser = new BitFieldParser
                                {
                                    Settings = new NumberBitFieldParser
                                    {
                                        FieldName = "ServiceType"
                                    }
                                }
                            }
                        }
                    }
                }
            };
            positionedFieldParsers.Add(callType);

            PositionedFieldParser supplementaryServiceType = new PositionedFieldParser
            {
                Position = 114,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "SupplementaryServiceType",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(supplementaryServiceType);

            PositionedFieldParser chargingCase = new PositionedFieldParser
            {
                Position = 115,
                Length = 2,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "ChargingCase",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(chargingCase);

            PositionedFieldParser tariff = new PositionedFieldParser
            {
                Position = 117,
                Length = 2,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "Tariff",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(tariff);

            PositionedFieldParser chargingPulse = new PositionedFieldParser
            {
                Position = 119,
                Length = 4,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "ChargingPulse",
                        NumberType = NumberType.BigInt
                    }
                }
            };
            positionedFieldParsers.Add(chargingPulse);

            PositionedFieldParser fee = new PositionedFieldParser
            {
                Position = 123,
                Length = 4,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "Fee",
                        NumberType = NumberType.BigInt
                    }
                }
            };
            positionedFieldParsers.Add(fee);

            PositionedFieldParser balance = new PositionedFieldParser
            {
                Position = 127,
                Length = 4,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "Balance",
                        NumberType = NumberType.BigInt
                    }
                }
            };
            positionedFieldParsers.Add(balance);

            PositionedFieldParser bearerService = new PositionedFieldParser
            {
                Position = 131,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "BearerService",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(bearerService);

            PositionedFieldParser teleservice = new PositionedFieldParser
            {
                Position = 132,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new PositionedBitsBinaryFieldParser
                    {
                        BitParsers = new List<PositionedBitFieldParser>
                        {
                             new PositionedBitFieldParser{
                                 Position = 0,
                                 Length = 4,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new NumberBitFieldParser
                                     {
                                         FieldName = "Teleservice"
                                     }
                                 }
                             },
                             new PositionedBitFieldParser{
                                 Position = 4,
                                 Length = 3,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new NumberBitFieldParser
                                     {
                                         FieldName = "ReleaseParty"
                                     }
                                 }
                             },
                             new PositionedBitFieldParser{
                                 Position = 7,
                                 Length = 1,
                                 FieldParser = new BitFieldParser
                                 {
                                     Settings = new NumberBitFieldParser
                                     {
                                         FieldName = "ReleaseIndex"
                                     }
                                 }
                             }
                        }
                    }
                }
            };
            positionedFieldParsers.Add(teleservice);

            PositionedFieldParser releaseCauseValue = new PositionedFieldParser
            {
                Position = 133,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "ReleaseCauseValue",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(releaseCauseValue);

            PositionedFieldParser uUS1Count = new PositionedFieldParser
            {
                Position = 134,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "UUS1Count",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(uUS1Count);

            PositionedFieldParser uUS2Count = new PositionedFieldParser
            {
                Position = 135,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "UUS2Count",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(uUS2Count);

            PositionedFieldParser uUS3Count = new PositionedFieldParser
            {
                Position = 136,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "UUS3Count",
                        NumberType = NumberType.Int
                    }
                }
            };
            positionedFieldParsers.Add(uUS3Count);

            PositionedFieldParser opc = new PositionedFieldParser
            {
                Position = 137,
                Length = 4,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "OPC",
                        NumberType = NumberType.BigInt
                    }
                }
            };
            positionedFieldParsers.Add(opc);

            PositionedFieldParser dpc = new PositionedFieldParser
            {
                Position = 141,
                Length = 4,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new NumberFieldParser
                    {
                        FieldName = "DPC",
                        NumberType = NumberType.BigInt
                    }
                }
            };
            positionedFieldParsers.Add(dpc);

            PositionedFieldParser bNum = new PositionedFieldParser
            {
                Position = 145,
                Length = 1,
                FieldParser = new BinaryFieldParser
                {
                    Settings = new PositionedBitsBinaryFieldParser
                    {
                        BitParsers = new List<PositionedBitFieldParser>
                        {
                            new PositionedBitFieldParser{
                                Position = 0,
                                Length = 5,
                                FieldParser = new BitFieldParser
                                {
                                    Settings = new NumberBitFieldParser
                                    {
                                        FieldName = "B_Num"
                                    }
                                }
                            }
                        }
                    }
                }
            };
            positionedFieldParsers.Add(bNum);

            return positionedFieldParsers;
        }
        private List<CompositeFieldsParser> GetMobilisHuaweiCompositeParsers()
        {
            List<CompositeFieldsParser> compositeParsers = new List<CompositeFieldsParser>();
            compositeParsers.Add(new FileNameCompositeParser() { FieldName = "FileName" });
            compositeParsers.Add(new DataSourceCompositeParser() { DataSourceFieldName = "DataSourceId" });
            compositeParsers.Add(new DateTimeCompositeParser()
            {
                FieldName = "ConversationBeginTime",
                DateFieldName = "ConversationEndTime",
                TimeFieldName = "ConversationDuration",
                TimeFieldUnit = TimeFieldUnit.Seconds,
                SubtractTime = true
            });

            return compositeParsers;
        }

        #endregion

        #endregion

        #region Namibia

        #region Huawei 

        public string GetHuaweiNamibiaParserSettings()
        {
            BinaryParserType hexParser = new BinaryParserType
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
        private Dictionary<string, BinaryRecordParser> Get_30_SubRecordsParsersByTag()
        {
            Dictionary<string, BinaryRecordParser> subParser = new Dictionary<string, BinaryRecordParser>();

            subParser.Add("30", new BinaryRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = Get_30_SplitRecordsParsersByTag()
                }
            });

            return subParser;
        }
        private Dictionary<string, BinaryRecordParser> Get_30_SplitRecordsParsersByTag()
        {
            Dictionary<string, BinaryRecordParser> subParser = new Dictionary<string, BinaryRecordParser>();

            subParser.Add("A1", new BinaryRecordParser
            {
                Settings = new ExecuteTemplateRecordParser
                {
                    RecordParserTemplateId = new Guid("9AB5792A-BB25-412C-BFCD-791515F7C893")
                }

            });

            return subParser;
        }
        private Dictionary<Guid, BinaryRecordParser> GetTemplates_Huawei()
        {
            Dictionary<Guid, BinaryRecordParser> templates = new Dictionary<Guid, BinaryRecordParser>();

            templates.Add(new Guid("9AB5792A-BB25-412C-BFCD-791515F7C893"), new BinaryRecordParser
            {
                Settings = new SplitByTagRecordParser
                {
                    SubRecordsParsersByTag = GetTemplateParsers_Huawei()
                }
            });

            return templates;
        }
        private Dictionary<string, BinaryRecordParser> GetTemplateParsers_Huawei()
        {
            Dictionary<string, BinaryRecordParser> parsers = new Dictionary<string, BinaryRecordParser>();

            parsers.Add("A3", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_A3_FieldParsers_Huawei()
                    },
                    RecordType = "Namibia_WHS_CDR",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> {
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    },
                    CompositeFieldsParsers = GetHuaweiNamibiaCompositeParsers()
                }

            });

            parsers.Add("A4", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_A4_FieldParsers_Huawei()
                    },
                    RecordType = "Namibia_WHS_CDR",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> {
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    },
                    CompositeFieldsParsers = GetHuaweiNamibiaCompositeParsers()
                }
            });

            parsers.Add("A6", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_A6_SMSMO_FieldParsers_Huawei()
                    },
                    RecordType = "Namibia_SMS",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> {
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    },
                    CompositeFieldsParsers = GetHuaweiNamibiaCompositeParsers()

                }
            });

            parsers.Add("A7", new BinaryRecordParser
            {
                Settings = new CreateRecordRecordParser
                {
                    FieldParsers = new BinaryFieldParserCollection
                    {
                        FieldParsersByTag = Get_A7_SMSMT_FieldParsers_Huawei()
                    },
                    RecordType = "Namibia_SMS",
                    FieldConstantValues = new List<ParsedRecordFieldConstantValue> {
                     new ParsedRecordFieldConstantValue{ FieldName = "SwitchId", Value = 5}
                    },
                    CompositeFieldsParsers = GetHuaweiNamibiaCompositeParsers()
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A3_FieldParsers_Huawei()
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

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CGPN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CDPN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RecordEntity",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("A4", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
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

            parsers.Add("A5", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
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

            parsers.Add("87", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("ConnectDateTime")
            });

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("DisconnectDateTime")
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "DurationInSeconds",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForTermination",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AC", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
                                   {
                                        Settings = new DiagnosticsParser
                                        {
                                             FieldName = "Diagnostics"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("99", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "HLC"
                }
            });

            parsers.Add("BF8102", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"83", new BinaryFieldParser
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

            parsers.Add("BF8105", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
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

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChangeAreaCode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8120", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RoamingNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8127", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "MSCIncomingCircuit"
                }
            });

            parsers.Add("9F8128", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "MSCID"
                }
            });

            parsers.Add("9F812A", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CallEmplPriority"
                }
            });

            parsers.Add("9F8143", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CUGIndicatorAccess"
                }
            });

            parsers.Add("9F8146", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "MSCIncomingRouteAttribute"
                }
            });

            parsers.Add("9F8148", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "NetworkCallReference",
                    NumberType = NumberType.BigInt,
                    ConvertOutputToString = true
                }
            });

            parsers.Add("9F815A", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "DisconnectParty"
                }
            });

            parsers.Add("9F8161", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "AudioDataType"
                }
            });

            parsers.Add("9F8168", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("9F8175", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "TranslatedNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("BF8177", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "LAC",
                                          NumberType = NumberType.Int
                                     }
                                }
                            },
                            {"81", new BinaryFieldParser
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

            parsers.Add("9F8179", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "FirstMSC"
                }
            });

            parsers.Add("9F817A", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "LastMSC"
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A4_FieldParsers_Huawei()
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


            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CGPN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "CDPN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RecordEntity",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("A4", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
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
            parsers.Add("A5", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
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


            parsers.Add("87", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("ConnectDateTime")
            });

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("DisconnectDateTime")
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "DurationInSeconds",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CauseForTermination",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AC", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
                                   {
                                        Settings = new DiagnosticsParser
                                        {
                                             FieldName = "Diagnostics"
                                        }
                                   }
                            }
                        }
                    }
                }
            });

            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("99", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "HLC"
                }
            });

            parsers.Add("BF8102", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"83", new BinaryFieldParser
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

            parsers.Add("BF8105", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
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

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChangeAreaCode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8120", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RoamingNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8127", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "MSCIncomingCircuit"
                }
            });

            parsers.Add("9F8128", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "MSCID"
                }
            });

            parsers.Add("9F812A", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CallEmplPriority"
                }
            });

            parsers.Add("9F8143", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "CUGIndicatorAccess"
                }
            });

            parsers.Add("9F8146", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "MSCIncomingRouteAttribute"
                }
            });

            parsers.Add("9F8148", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "NetworkCallReference",
                    NumberType = NumberType.BigInt,
                    ConvertOutputToString = true
                }
            });

            parsers.Add("9F815A", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "DisconnectParty"
                }
            });

            parsers.Add("9F8161", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "AudioDataType"
                }
            });


            parsers.Add("9F8168", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.BigInt

                }
            });

            parsers.Add("9F8175", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "TranslatedNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("BF8177", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "LAC",
                                          NumberType = NumberType.Int
                                     }
                                }
                            },
                            {"81", new BinaryFieldParser
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

            parsers.Add("9F8179", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "FirstMSC"
                }
            });

            parsers.Add("9F817A", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "LastMSC"
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A6_SMSMO_FieldParsers_Huawei()
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

            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "MSClassmark",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServiceCenter",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RecordingEntity",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("A7", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "SAC",
                                          NumberType = NumberType.Int
                                     }
                                }
                            },
                            {"80", new BinaryFieldParser
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

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "MessageReference",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("89", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("MessageTime")
            });

            parsers.Add("AA", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SMSResult",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AB", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "RecordExtensions"
                }
            });

            parsers.Add("8C", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "DestinationNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("AD", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                {
                                     Settings = new BCDNumberParser
                                     {
                                          FieldName = "GsmSCFAddress",
                                          AIsZero = false,
                                          RemoveHexa = true
                                     }
                                }
                            },
                            {"82", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "ServiceKey",
                                          NumberType = NumberType.Int
                                     }
                                }
                            },
                            {"83", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "DefaultSMSHandling",
                                          NumberType = NumberType.Int
                                     }
                                }
                            },
                            {"84", new BinaryFieldParser
                                {
                                     Settings = new HexaParser
                                     {
                                          FieldName = "FreeFormatData"
                                     }
                                }
                            },
                            {"85", new BinaryFieldParser
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

            parsers.Add("8E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SystemType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8F", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "LocationExtension"
                }
            });

            parsers.Add("8F8102", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "BasicService",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("BF8105", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "ChargeIndicator",
                                          NumberType = NumberType.Int
                                     }
                                }
                            },
                            {"81", new BinaryFieldParser
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

            parsers.Add("9F810C", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ClassMark"
                }
            });

            parsers.Add("9F810D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargedParty",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8127", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "OriginatingRNCorBSCId"
                }
            });

            parsers.Add("9F8128", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "OriginatingMSCId"
                }
            });

            parsers.Add("9F813C", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F813E", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "SubscriberCategory"
                }
            });

            parsers.Add("9F8140", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "FirstMCC_MNC"
                }
            });

            parsers.Add("9F8143", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "SMSUserDataType"
                }
            });

            parsers.Add("9F8144", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "SMSText"
                }
            });

            parsers.Add("9F8145", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MaxNumberOfSMSInConcatenatedSMS",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8146", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ConcatenatedSMSReferenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8147", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumberOfCurrentSMS",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8148", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "HotBillingTag",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8149", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("9F814A", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TariffCode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F815F", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "NetworkOperatorId"
                }
            });

            parsers.Add("9F8160", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TypeOfSubscribers",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8163", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "UserType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816B", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "OSS_ServicesUsed"
                }
            });

            parsers.Add("9F816D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargeLevel",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8170", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ZoneCode"
                }
            });

            parsers.Add("9F8201", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "RoutingCategory"
                }
            });

            parsers.Add("9F8116", new BinaryFieldParser
            {
                Settings = new BoolFieldParser
                {
                    FieldName = "SMMODirect"
                }
            });

            parsers.Add("9F821D", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OfficeName"
                }
            });

            parsers.Add("9F821E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F821F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SMSType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8221", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "SMMOCommandType"
                }
            });

            parsers.Add("9F8222", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SwitchMode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F823D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "AdditionalRoutingCategory",
                    NumberType = NumberType.Int
                }
            });

            return parsers;
        }
        private Dictionary<string, BinaryFieldParser> Get_A7_SMSMT_FieldParsers_Huawei()
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


            parsers.Add("81", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServiceCenter",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("82", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMSI",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("83", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedIMEI",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("84", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "DestinationNumber",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("9F8149", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "ServedMSISDN",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("85", new BinaryFieldParser
            {
                Settings = new BCDNumberParser
                {
                    FieldName = "MSClassmark",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("86", new BinaryFieldParser
            {
                Settings = new TBCDNumberParser
                {
                    FieldName = "RecordingEntity",
                    AIsZero = false,
                    RemoveHexa = true
                }
            });

            parsers.Add("A7", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "SAC",
                                          NumberType = NumberType.Int
                                     }
                                }
                            },
                            {"80", new BinaryFieldParser
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

            parsers.Add("88", new BinaryFieldParser
            {
                Settings = GetDateTimeParser("MessageTime")
            });

            parsers.Add("A9", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SMSResult",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("AA", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "RecordExtensions"
                }
            });

            parsers.Add("8B", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SystemType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("8D", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "LocationExtension"
                }
            });

            parsers.Add("AC", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"81", new BinaryFieldParser
                                {
                                     Settings = new BCDNumberParser
                                     {
                                          FieldName = "GsmSCFAddress",
                                          AIsZero = false,
                                          RemoveHexa = true
                                     }
                                }
                            },
                            {"82", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "ServiceKey",
                                          NumberType = NumberType.Int
                                     }
                                }
                            },
                            {"83", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "DefaultSMSHandling",
                                          NumberType = NumberType.Int
                                     }
                                }
                            },
                            {"84", new BinaryFieldParser
                                {
                                     Settings = new HexaParser
                                     {
                                          FieldName = "FreeFormatData"
                                     }
                                }
                            },
                            {"85", new BinaryFieldParser
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

            parsers.Add("BF8102", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "BasicService",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("BF8105", new BinaryFieldParser
            {
                Settings = new SequenceFieldParser
                {
                    FieldParsers = new Vanrise.DataParser.Entities.BinaryFieldParserCollection
                    {
                        FieldParsersByTag = new Dictionary<string, BinaryFieldParser>
                        {
                            {"80", new BinaryFieldParser
                                {
                                     Settings = new NumberFieldParser
                                     {
                                          FieldName = "ChargeIndicator",
                                          NumberType = NumberType.Int
                                     }
                                }
                            },
                            {"81", new BinaryFieldParser
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

            parsers.Add("9F810C", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ClassMark"
                }
            });

            parsers.Add("9F810D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargedParty",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8111", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ChargeAreaCode"
                }
            });

            parsers.Add("9F8127", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "OriginatingRNCorBSCId"
                }
            });

            parsers.Add("9F8128", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "OriginatingMSCId"
                }
            });

            parsers.Add("9F813C", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "GlobalAreaID"
                }
            });

            parsers.Add("9F813E", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "SubscriberCategory"
                }
            });

            parsers.Add("9F8140", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "FirstMCC_MNC"
                }
            });

            parsers.Add("9F8143", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "SMSUserDataType"
                }
            });

            parsers.Add("9F8144", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "SMSText"
                }
            });

            parsers.Add("9F8145", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MaxNumberOfSMSInConcatenatedSMS",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8146", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ConcatenatedSMSReferenceNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8147", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SequenceNumberOfCurrentSMS",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8148", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "HotBillingTag",
                    NumberType = NumberType.Int
                }
            });

            //parsers.Add("9F8149", new HexTLVFieldParser
            //{
            //    Settings = new BCDNumberParser
            //    {
            //        FieldName = "Origination",
            //        AIsZero = false,
            //        RemoveHexa = true
            //    }
            //});

            parsers.Add("9F814A", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "CallReference",
                    NumberType = NumberType.BigInt
                }
            });

            parsers.Add("9F814B", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TariffCode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F815F", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "NetworkOperatorId"
                }
            });

            parsers.Add("9F8160", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "TypeOfSubscribers",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8168", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "RecordNumber",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F816D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "ChargeLevel",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8170", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "ZoneCode"
                }
            });

            parsers.Add("9F8201", new BinaryFieldParser
            {
                Settings = new HexaParser
                {
                    FieldName = "RoutingCategory"
                }
            });

            parsers.Add("9F821D", new BinaryFieldParser
            {
                Settings = new StringParser
                {
                    FieldName = "OfficeName"
                }
            });

            parsers.Add("9F821E", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "MSCType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F821F", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SMSType",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F8222", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "SwitchMode",
                    NumberType = NumberType.Int
                }
            });

            parsers.Add("9F823D", new BinaryFieldParser
            {
                Settings = new NumberFieldParser
                {
                    FieldName = "AdditionalRoutingCategory",
                    NumberType = NumberType.Int
                }
            });


            return parsers;
        }
        private List<CompositeFieldsParser> GetHuaweiNamibiaCompositeParsers()
        {
            List<CompositeFieldsParser> fieldParsers = new List<CompositeFieldsParser> {
            new FileNameCompositeParser{
             FieldName = "FileName"
            }
            };

            return fieldParsers;
        }

        #endregion



        #endregion

    }
}