﻿CREATE TYPE [Mediation_WHS].[OgeroHuaweiCDRType] AS TABLE (
    [Id]                        BIGINT        NULL,
    [RecordType]                INT           NULL,
    [SipMethod]                 VARCHAR (100) NULL,
    [NodeRole]                  INT           NULL,
    [DomainName]                VARCHAR (100) NULL,
    [SessionId]                 VARCHAR (100) NULL,
    [CallingPartyAddress]       VARCHAR (100) NULL,
    [CalledPartyAddress]        VARCHAR (100) NULL,
    [ServiceRequestTime]        DATETIME      NULL,
    [ServiceDeliveryStartTime]  DATETIME      NULL,
    [ServiceDeliveryEndTime]    DATETIME      NULL,
    [RecordOpeningTime]         DATETIME      NULL,
    [RecordClosureTime]         DATETIME      NULL,
    [OriginatingIOI]            VARCHAR (100) NULL,
    [TerminatingIOI]            VARCHAR (100) NULL,
    [LocalRecordSequenceNumber] INT           NULL,
    [CauseForRecordClosing]     INT           NULL,
    [ACRStartLost]              BIT           NULL,
    [ACRInterimLost]            BIT           NULL,
    [ACRStopLost]               BIT           NULL,
    [IMSChargingIdentifier]     VARCHAR (100) NULL,
    [SIPRequestTime]            DATETIME      NULL,
    [SIPResponseTime]           DATETIME      NULL,
    [SDPMediaName]              VARCHAR (100) NULL,
    [SDPMediaDescription]       VARCHAR (100) NULL,
    [SDPSessionDescription]     VARCHAR (MAX) NULL,
    [ServiceReasonReturnCode]   INT           NULL,
    [ContentType]               VARCHAR (100) NULL,
    [ContentLength]             INT           NULL,
    [AccessNetworkInformation]  VARCHAR (200) NULL,
    [ServiceContextID]          VARCHAR (100) NULL,
    [CalledAssertedIdentity]    VARCHAR (100) NULL,
    [Duration]                  INT           NULL,
    [DialledPartyAddress]       VARCHAR (100) NULL,
    [RingingDuration]           INT           NULL,
    [ServiceIdentifier]         INT           NULL,
    [ChargingCategory]          INT           NULL,
    [CallProperty]              INT           NULL,
    [AccountingRecordType]      INT           NULL,
    [OnlineChargingFlag]        INT           NULL,
    [VisitedNetworkId]          VARCHAR (500) NULL,
    [FileName]                  VARCHAR (100) NULL);





