CREATE TABLE [Mobile_EDR].[SMS] (
    [Id]                              BIGINT        IDENTITY (1, 1) NOT NULL,
    [RecordType]                      INT           NULL,
    [ServedIMSI]                      VARCHAR (100) NULL,
    [ServedIMEI]                      VARCHAR (100) NULL,
    [ServedMSISDN]                    VARCHAR (100) NULL,
    [MSClassmark]                     VARCHAR (100) NULL,
    [ServiceCenter]                   VARCHAR (100) NULL,
    [RecordingEntity]                 VARCHAR (100) NULL,
    [MessageReference]                VARCHAR (100) NULL,
    [MessageTime]                     DATETIME      NULL,
    [SMSResult]                       INT           NULL,
    [RecordExtensions]                VARCHAR (100) NULL,
    [DestinationNumber]               VARCHAR (100) NULL,
    [CamelSMSInformation]             VARCHAR (100) NULL,
    [SystemType]                      INT           NULL,
    [LocationExtension]               VARCHAR (100) NULL,
    [BasicService]                    INT           NULL,
    [AdditionalChangeInfo]            VARCHAR (100) NULL,
    [ClassMark]                       VARCHAR (100) NULL,
    [ChargedParty]                    INT           NULL,
    [ChargeAreaCode]                  VARCHAR (100) NULL,
    [OriginatingRNCorBSCId]           VARCHAR (100) NULL,
    [OriginatingMSCId]                VARCHAR (100) NULL,
    [CalledIMSI]                      VARCHAR (100) NULL,
    [GlobalAreaID]                    VARCHAR (100) NULL,
    [SubscriberCategory]              VARCHAR (100) NULL,
    [FirstMCC_MNC]                    VARCHAR (100) NULL,
    [SMSUserDataType]                 VARCHAR (100) NULL,
    [SMSText]                         VARCHAR (100) NULL,
    [MaxNumberOfSMSInConcatenatedSMS] INT           NULL,
    [ConcatenatedSMSReferenceNumber]  INT           NULL,
    [SequenceNumberOfCurrentSMS]      INT           NULL,
    [HotBillingTag]                   INT           NULL,
    [CallReference]                   VARCHAR (100) NULL,
    [TariffCode]                      INT           NULL,
    [NetworkOperatorId]               VARCHAR (100) NULL,
    [TypeOfSubscribers]               INT           NULL,
    [UserType]                        INT           NULL,
    [RecordNumber]                    INT           NULL,
    [OSS_ServicesUsed]                VARCHAR (100) NULL,
    [ChargeLevel]                     INT           NULL,
    [ZoneCode]                        VARCHAR (100) NULL,
    [RoutingCategory]                 VARCHAR (100) NULL,
    [VOBBUserFlag]                    VARCHAR (100) NULL,
    [SMMODirect]                      BIT           NULL,
    [OfficeName]                      VARCHAR (100) NULL,
    [MSCType]                         INT           NULL,
    [SMSType]                         INT           NULL,
    [SMMOCommandType]                 VARCHAR (100) NULL,
    [SwitchMode]                      INT           NULL,
    [AdditionalRoutingCategory]       INT           NULL,
    [CallOrigin]                      INT           NULL,
    [SMSDataCodingSchema]             INT           NULL,
    [SMSUserDataLength]               INT           NULL,
    [Origination]                     VARCHAR (100) NULL,
    [MTSMSBlackListFlag]              BIT           NULL,
    [SAC]                             VARCHAR (100) NULL,
    [LocationAreaCode]                VARCHAR (100) NULL,
    [SwitchID]                        INT           NULL,
    [CallingNumber]                   VARCHAR (50)  NULL,
    [CalledNumber]                    VARCHAR (50)  NULL,
    CONSTRAINT [PK_SMS] PRIMARY KEY CLUSTERED ([Id] ASC)
);












GO


