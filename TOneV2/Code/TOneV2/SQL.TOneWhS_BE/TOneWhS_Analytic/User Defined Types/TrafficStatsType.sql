CREATE TYPE [TOneWhS_Analytic].[TrafficStatsType] AS TABLE (
    [ID]                     BIGINT          NULL,
    [CustomerId]             INT             NULL,
    [SupplierId]             INT             NULL,
    [Attempts]               INT             NOT NULL,
    [TotalDurationInSeconds] NUMERIC (20, 6) NOT NULL,
    [FirstCDRAttempt]        DATETIME        NOT NULL,
    [LastCDRAttempt]         DATETIME        NOT NULL,
    [SaleZoneID]             BIGINT          NULL,
    [SupplierZoneID]         BIGINT          NULL,
    [PDDInSeconds]           NUMERIC (20, 6) NOT NULL,
    [MaxDurationInSeconds]   NUMERIC (20, 6) NOT NULL,
    [NumberOfCalls]          INT             NOT NULL,
    [PortOut]                NVARCHAR (50)   NULL,
    [PortIn]                 NVARCHAR (50)   NULL,
    [DeliveredAttempts]      INT             NOT NULL,
    [SuccessfulAttempts]     INT             NOT NULL,
    [SumOfPGAD]              NUMERIC (20, 6) NOT NULL,
    [DeliveredNumberOfCalls] INT             NOT NULL,
    [CeiledDuration]         BIGINT          NOT NULL,
    [UtilizationInSeconds]   NUMERIC (20, 6) NOT NULL);



