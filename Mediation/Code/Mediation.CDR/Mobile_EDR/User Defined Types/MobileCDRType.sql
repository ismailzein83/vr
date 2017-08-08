CREATE TYPE [Mobile_EDR].[MobileCDRType] AS TABLE (
    [RecordType]                       INT              NULL,
    [ServedIMSI]                       VARCHAR (100)    NULL,
    [ServedIMEI]                       VARCHAR (100)    NULL,
    [CallingNumber]                    VARCHAR (100)    NULL,
    [CalledNumber]                     VARCHAR (100)    NULL,
    [SAC]                              INT              NULL,
    [LocationAreaCode]                 INT              NULL,
    [CallDuration]                     INT              NULL,
    [SequenceNumber]                   INT              NULL,
    [PartialRecordType]                INT              NULL,
    [ChargeAreaCode]                   VARCHAR (100)    NULL,
    [CalledChargeAreaCode]             VARCHAR (100)    NULL,
    [GlobalAreaID]                     VARCHAR (100)    NULL,
    [SetupTime]                        DATETIME         NULL,
    [CallType]                         INT              NULL,
    [LocationNumber]                   VARCHAR (100)    NULL,
    [ZoneCode]                         VARCHAR (100)    NULL,
    [LocationNumberNai]                INT              NULL,
    [Termioi]                          VARCHAR (100)    NULL,
    [Origioi]                          VARCHAR (100)    NULL,
    [RecordNumber]                     INT              NULL,
    [CalledIMSI]                       VARCHAR (100)    NULL,
    [CalledIMEI]                       VARCHAR (100)    NULL,
    [OriginalCalledNumber]             VARCHAR (100)    NULL,
    [VoiceIndicator]                   INT              NULL,
    [ServedMSISDN]                     VARCHAR (100)    NULL,
    [InputCalledNumber]                VARCHAR (100)    NULL,
    [ConnectDateTime]                  DATETIME         NULL,
    [DisconnectDateTime]               DATETIME         NULL,
    [Longitude]                        VARCHAR (50)     NULL,
    [Latitude]                         VARCHAR (50)     NULL,
    [SwitchID]                         INT              NULL,
    [CallingIMSI]                      VARCHAR (50)     NULL,
    [CallingIMEI]                      VARCHAR (50)     NULL,
    [FisrtSiteId]                      INT              NULL,
    [LastSiteId]                       INT              NULL,
    [CallingLocationInformation_First] VARCHAR (100)    NULL,
    [CallingLocationInformation_Last]  VARCHAR (100)    NULL,
    [CalledLocationInformation_First]  VARCHAR (100)    NULL,
    [CalledLocationInformation_Last]   VARCHAR (100)    NULL,
    [ConnectTimestamp]                 BIGINT           NULL,
    [DisconnectTimestamp]              BIGINT           NULL,
    [UniqueIdentifier]                 UNIQUEIDENTIFIER NULL,
    [Subs_First_LAC]                   INT              NULL,
    [Subs_Last_LAC]                    INT              NULL,
    [FileName]                         NVARCHAR (200)   NULL,
    [Called_First_CI]                  INT              NULL,
    [Called_Last_CI]                   INT              NULL,
    [Calling_First_CI]                 INT              NULL,
    [Calling_Last_CI]                  INT              NULL,
    [RecordTypeName]                   VARCHAR (100)    NULL);



















