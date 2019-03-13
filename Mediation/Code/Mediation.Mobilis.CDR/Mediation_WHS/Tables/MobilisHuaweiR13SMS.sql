CREATE TABLE [Mediation_WHS].[MobilisHuaweiR13SMS] (
    [Id]                       BIGINT           IDENTITY (1, 1) NOT NULL,
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
    [DataSourceId]             UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_MobilisHuaweiR13SMS] PRIMARY KEY CLUSTERED ([Id] ASC)
);

