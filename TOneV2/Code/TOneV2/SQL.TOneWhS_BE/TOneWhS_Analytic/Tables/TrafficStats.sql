CREATE TABLE [TOneWhS_Analytic].[TrafficStats] (
    [ID]                     BIGINT          NULL,
    [CustomerID]             INT             NULL,
    [SupplierID]             INT             NULL,
    [Attempts]               INT             NULL,
    [DurationInSeconds]      NUMERIC (20, 6) NULL,
    [FirstCDRAttempt]        DATETIME        NULL,
    [LastCDRAttempt]         DATETIME        NULL,
    [SaleZoneID]             BIGINT          NULL,
    [SaleCode]               VARCHAR (20)    NULL,
    [SupplierZoneID]         BIGINT          NULL,
    [SupplierCode]           VARCHAR (20)    NULL,
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
    [CodeGroup]              VARCHAR (10)    NULL,
    CONSTRAINT [IX_TrafficStats_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);








GO
CREATE CLUSTERED INDEX [IX_TrafficStats_DateTimeFirst]
    ON [TOneWhS_Analytic].[TrafficStats]([FirstCDRAttempt] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_TrafficStats_Supplier]
    ON [TOneWhS_Analytic].[TrafficStats]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TrafficStats_Customer]
    ON [TOneWhS_Analytic].[TrafficStats]([CustomerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TrafficStats_SaleZone]
    ON [TOneWhS_Analytic].[TrafficStats]([SaleZoneID] ASC);

