﻿CREATE TYPE [Mobile_EDR].[MobileCDRType] AS TABLE (
    [RecordType]                       INT              NULL,
    [ServedIMSI]                       VARCHAR (100)    NULL,
    [ServedIMEI]                       VARCHAR (100)    NULL,
    [CallingNumber]                    VARCHAR (100)    NULL,
    [CalledNumber]                     VARCHAR (100)    NULL,
    [SAC]                              VARCHAR (100)    NULL,
    [LocationAreaCode]                 VARCHAR (100)    NULL,
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
    [CallingLocationInformation_First] INT              NULL,
    [CallingLocationInformation_Last]  INT              NULL,
    [CalledLocationInformation_First]  INT              NULL,
    [CalledLocationInformation_Last]   INT              NULL,
    [ConnectTimestamp]                 BIGINT           NULL,
    [DisconnectTimestamp]              BIGINT           NULL,
    [UniqueIdentifier]                 UNIQUEIDENTIFIER NULL);













