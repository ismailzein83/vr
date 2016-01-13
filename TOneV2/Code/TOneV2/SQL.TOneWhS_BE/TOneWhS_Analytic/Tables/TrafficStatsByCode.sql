CREATE TABLE [TOneWhS_Analytic].[TrafficStatsByCode] (
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
    CONSTRAINT [IX_TrafficStatsByCode_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_TrafficStats_DateTimeFirst]
    ON [TOneWhS_Analytic].[TrafficStatsByCode]([FirstCDRAttempt] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_TrafficStatsByCode_Supplier]
    ON [TOneWhS_Analytic].[TrafficStatsByCode]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TrafficStatsByCode_Customer]
    ON [TOneWhS_Analytic].[TrafficStatsByCode]([CustomerID] ASC);

