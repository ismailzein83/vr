﻿CREATE TABLE [Mediation_WHS].[MobilisEricssonBadSMS] (
    [Id]                       BIGINT           NULL,
    [RecordType]               INT              NULL,
    [RecordTypeName]           NVARCHAR (100)   NULL,
    [CallIdentificationNumber] INT              NULL,
    [RecordSequenceNumber]     INT              NULL,
    [CallingPartyNumber]       VARCHAR (50)     NULL,
    [CalledPartyNumber]        VARCHAR (50)     NULL,
    [CallingSubscriberIMSI]    VARCHAR (50)     NULL,
    [CalledSubscriberIMSI]     VARCHAR (50)     NULL,
    [CallingSubscriberIMEI]    VARCHAR (50)     NULL,
    [CalledSubscriberIMEI]     VARCHAR (50)     NULL,
    [CallingSubscriberIMEISV]  VARCHAR (50)     NULL,
    [CalledSubscriberIMEISV]   VARCHAR (50)     NULL,
    [MessageTime]              DATETIME         NULL,
    [IncomingRoute]            VARCHAR (20)     NULL,
    [OutgoingRoute]            VARCHAR (20)     NULL,
    [OriginatingAddress]       VARCHAR (100)    NULL,
    [DestinationAddress]       VARCHAR (100)    NULL,
    [FileName]                 VARCHAR (255)    NULL,
    [DataSourceId]             UNIQUEIDENTIFIER NULL
);

