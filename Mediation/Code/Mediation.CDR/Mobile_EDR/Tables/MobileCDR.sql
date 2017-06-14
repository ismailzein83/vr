CREATE TABLE [Mobile_EDR].[MobileCDR] (
    [Id]                         BIGINT        IDENTITY (1, 1) NOT NULL,
    [RecordType]                 INT           NULL,
    [ServedIMSI]                 VARCHAR (100) NULL,
    [ServedIMEI]                 VARCHAR (100) NULL,
    [CallingNumber]              VARCHAR (100) NULL,
    [CalledNumber]               VARCHAR (100) NULL,
    [SAC]                        VARCHAR (100) NULL,
    [LocationAreaCode]           VARCHAR (100) NULL,
    [CallDuration]               INT           NULL,
    [SequenceNumber]             INT           NULL,
    [PartialRecordType]          INT           NULL,
    [ChargeAreaCode]             VARCHAR (100) NULL,
    [CalledChargeAreaCode]       VARCHAR (100) NULL,
    [GlobalAreaID]               VARCHAR (100) NULL,
    [SetupTime]                  DATETIME      NULL,
    [CallType]                   INT           NULL,
    [LocationNumber]             VARCHAR (100) NULL,
    [ZoneCode]                   VARCHAR (100) NULL,
    [LocationNumberNai]          INT           NULL,
    [Termioi]                    VARCHAR (100) NULL,
    [Origioi]                    VARCHAR (100) NULL,
    [RecordNumber]               INT           NULL,
    [CalledIMSI]                 VARCHAR (100) NULL,
    [CalledIMEI]                 VARCHAR (100) NULL,
    [OriginalCalledNumber]       VARCHAR (100) NULL,
    [VoiceIndicator]             INT           NULL,
    [ServedMSISDN]               VARCHAR (100) NULL,
    [InputCalledNumber]          VARCHAR (100) NULL,
    [ConnectDateTime]            DATETIME      NULL,
    [DisconnectDateTime]         DATETIME      NULL,
    [Longitude]                  VARCHAR (50)  NULL,
    [Latitude]                   VARCHAR (50)  NULL,
    [CallingLocationInformation] VARCHAR (50)  NULL,
    [CalledLocationInformation]  VARCHAR (50)  NULL,
    CONSTRAINT [PK_MobileCDR_1] PRIMARY KEY CLUSTERED ([Id] ASC)
);








GO


