CREATE TABLE [TOneWhS_Analytic].[TrafficStatsDaily] (
    [ID]                     BIGINT          NULL,
    [CustomerID]             INT             NULL,
    [SupplierID]             INT             NULL,
    [Attempts]               INT             NULL,
    [DurationInSeconds]      NUMERIC (20, 6) NULL,
    [FirstCDRAttempt]        DATETIME        NULL,
    [LastCDRAttempt]         DATETIME        NULL,
    [SaleZoneID]             BIGINT          NULL,
    [SupplierZoneID]         BIGINT          NULL,
    [SumOfPDDInSeconds]      NUMERIC (20, 6) NULL,
    [MaxDurationInSeconds]   NUMERIC (20, 6) NULL,
    [NumberOfCalls]          INT             NULL,
    [PortOut]                NVARCHAR (50)   NULL,
    [PortIn]                 NVARCHAR (50)   NULL,
    [DeliveredAttempts]      INT             NULL,
    [SuccessfulAttempts]     INT             NULL,
    [DeliveredNumberOfCalls] INT             NOT NULL,
    [CeiledDuration]         BIGINT          NOT NULL,
    [SwitchID]               INT             NULL,
    [SumOfPGAD]              NUMERIC (20, 6) NULL,
    [UtilizationInSeconds]   NUMERIC (20, 6) NULL,
    CONSTRAINT [IX_TrafficStatsDaily_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_TrafficStatsDaily_DateTimeFirst]
    ON [TOneWhS_Analytic].[TrafficStatsDaily]([FirstCDRAttempt] ASC);

