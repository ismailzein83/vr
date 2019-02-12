﻿CREATE TYPE [Mediation_WHS].[MobilisEricssonSMSType] AS TABLE (
    [Id]                       BIGINT           NULL,
    [RecordType]               INT              NULL,
    [RecordTypeName]           VARCHAR (50)     NULL,
    [CallIdentificationNumber] INT              NULL,
    [RecordSequenceNumber]     INT              NULL,
    [MessageTime]              DATETIME         NULL,
    [CallingPartyNumber]       VARCHAR (50)     NULL,
    [CalledPartyNumber]        VARCHAR (50)     NULL,
    [CallingSubscriberIMSI]    VARCHAR (50)     NULL,
    [CalledSubscriberIMSI]     VARCHAR (50)     NULL,
    [CallingSubscriberIMEI]    VARCHAR (50)     NULL,
    [CalledSubscriberIMEI]     VARCHAR (50)     NULL,
    [CallingSubscriberIMEISV]  VARCHAR (50)     NULL,
    [CalledSubscriberIMEISV]   VARCHAR (50)     NULL,
    [IncomingRoute]            VARCHAR (20)     NULL,
    [OutgoingRoute]            VARCHAR (20)     NULL,
    [OriginatingAddress]       VARCHAR (50)     NULL,
    [DestinationAddress]       VARCHAR (50)     NULL,
    [FileName]                 VARCHAR (255)    NULL,
    [DataSourceId]             UNIQUEIDENTIFIER NULL);



