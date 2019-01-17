CREATE TABLE [Mediation_WHS].[MobilisEricsson111CDR] (
    [Id]                   BIGINT           NOT NULL,
    [RecordType]           VARCHAR (5)      NULL,
    [CauseForOutput]       VARCHAR (5)      NULL,
    [RecordNumber]         VARCHAR (5)      NULL,
    [ANumber]              VARCHAR (25)     NULL,
    [BNumber]              VARCHAR (25)     NULL,
    [RedirectingNumber]    VARCHAR (25)     NULL,
    [DateForStartCharging] DATE             NULL,
    [TimeForStartCharging] TIME (3)         NULL,
    [TimeForStopCharging]  TIME (3)         NULL,
    [ChargeableDuration]   INT              NULL,
    [NumberOfMeterPulses]  VARCHAR (10)     NULL,
    [OutgoingRoute]        VARCHAR (10)     NULL,
    [IncomingRoute]        VARCHAR (10)     NULL,
    [ANumberLength]        VARCHAR (5)      NULL,
    [BNumberLength]        VARCHAR (5)      NULL,
    [OriginatingCode]      VARCHAR (5)      NULL,
    [FileName]             VARCHAR (255)    NULL,
    [DataSourceId]         UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_MobilisEricsson111CDR] PRIMARY KEY CLUSTERED ([Id] ASC)
);

