CREATE TYPE [Mediation_WHS].[EricssonBadCDRType] AS TABLE (
    [Id]                       BIGINT        NULL,
    [RecordType]               VARCHAR (5)   NULL,
    [CallStatus]               VARCHAR (20)  NULL,
    [CauseForOutput]           VARCHAR (20)  NULL,
    [ANumber]                  VARCHAR (20)  NULL,
    [BNumber]                  VARCHAR (20)  NULL,
    [ACategory]                VARCHAR (20)  NULL,
    [BCategory]                VARCHAR (20)  NULL,
    [ChargedParty]             VARCHAR (20)  NULL,
    [DateForStartCharging]     DATE          NULL,
    [TimeForStartCharging]     TIME (3)      NULL,
    [ChargeableDuration]       INT           NULL,
    [FaultCode]                VARCHAR (20)  NULL,
    [ExchangeIdentity]         VARCHAR (20)  NULL,
    [RecordNumber]             INT           NULL,
    [TariffClass]              VARCHAR (5)   NULL,
    [TariffSwitchingIndicator] VARCHAR (5)   NULL,
    [OriginForCharging]        VARCHAR (5)   NULL,
    [OutgoingRoute]            VARCHAR (20)  NULL,
    [IncomingRoute]            VARCHAR (20)  NULL,
    [Reserved]                 VARCHAR (10)  NULL,
    [FileName]                 VARCHAR (255) NULL);



