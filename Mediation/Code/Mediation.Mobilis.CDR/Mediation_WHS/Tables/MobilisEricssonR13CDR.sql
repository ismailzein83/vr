﻿CREATE TABLE [Mediation_WHS].[MobilisEricssonR13CDR] (
    [Id]                          BIGINT           IDENTITY (1, 1) NOT NULL,
    [RecordType]                  INT              NULL,
    [RecordTypeName]              VARCHAR (50)     NULL,
    [CallIdentificationNumber]    INT              NULL,
    [SetupTime]                   DATETIME         NULL,
    [ConnectDateTime]             DATETIME         NULL,
    [DisconnectDateTime]          DATETIME         NULL,
    [ChargeableDurationInSeconds] INT              NULL,
    [CallingPartyNumber]          VARCHAR (50)     NULL,
    [CalledPartyNumber]           VARCHAR (50)     NULL,
    [CallingSubscriberIMSI]       VARCHAR (50)     NULL,
    [CalledSubscriberIMSI]        VARCHAR (50)     NULL,
    [CallingSubscriberIMEI]       VARCHAR (50)     NULL,
    [CalledSubscriberIMEI]        VARCHAR (50)     NULL,
    [CallingSubscriberIMEISV]     VARCHAR (50)     NULL,
    [CalledSubscriberIMEISV]      VARCHAR (50)     NULL,
    [DisconnectingParty]          INT              NULL,
    [OriginalCalledNumber]        VARCHAR (50)     NULL,
    [RedirectingNumber]           VARCHAR (50)     NULL,
    [RedirectionCounter]          INT              NULL,
    [IncomingRoute]               VARCHAR (20)     NULL,
    [OutgoingRoute]               VARCHAR (20)     NULL,
    [TimeForTCSeizureCalling]     INT              NULL,
    [TimeForTCSeizureCalled]      INT              NULL,
    [NetworkCallReference]        BIGINT           NULL,
    [RecordSequenceNumber]        INT              NULL,
    [MobileStationRoamingNumber]  VARCHAR (50)     NULL,
    [FaultCode]                   INT              NULL,
    [FileName]                    VARCHAR (255)    NULL,
    [DataSourceId]                UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_MobilisEricssonCDR] PRIMARY KEY CLUSTERED ([Id] ASC)
);



