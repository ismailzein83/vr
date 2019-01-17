CREATE TYPE [Mediation_WHS].[MobilisEricsson102BadCDRType] AS TABLE (
    [Id]                   BIGINT           NULL,
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
    [FileName]             VARCHAR (255)    NULL,
    [DataSourceId]         UNIQUEIDENTIFIER NULL);

