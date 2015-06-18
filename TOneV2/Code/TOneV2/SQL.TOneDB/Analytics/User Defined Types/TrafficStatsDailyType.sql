CREATE TYPE [Analytics].[TrafficStatsDailyType] AS TABLE (
    [ID]                     BIGINT          NOT NULL,
    [Attempts]               INT             NOT NULL,
    [DeliveredAttempts]      INT             NOT NULL,
    [SuccessfulAttempts]     INT             NOT NULL,
    [DurationsInSeconds]     NUMERIC (19, 5) NOT NULL,
    [PDDInSeconds]           NUMERIC (19, 5) NULL,
    [UtilizationInSeconds]   NUMERIC (19, 5) NULL,
    [NumberOfCalls]          INT             NULL,
    [DeliveredNumberOfCalls] INT             NULL,
    [PGAD]                   NUMERIC (19, 5) NULL,
    [CeiledDuration]         BIGINT          NULL,
    [MaxDurationInSeconds]   NUMERIC (19, 5) NULL,
    [ReleaseSourceAParty]    INT             NULL,
    [ReleaseSourceS]         INT             NULL);

