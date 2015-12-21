CREATE TABLE [TOneWhS_Analytic].[TrafficStatsByCode] (
    [ID]                     BIGINT          NULL,
    [CustomerID]             INT             NULL,
    [SupplierID]             INT             NULL,
    [Attempts]               INT             NULL,
    [DurationInSeconds]      INT             NULL,
    [FirstCDRAttempt]        DATETIME        NULL,
    [LastCDRAttempt]         DATETIME        NULL,
    [SaleZoneID]             BIGINT          NULL,
    [SaleCode]               VARCHAR (20)    NULL,
    [SupplierZoneID]         BIGINT          NULL,
    [SupplierCode]           VARCHAR (20)    NULL,
    [SumOfPDDInSeconds]      INT             NULL,
    [MaxDurationInSeconds]   INT             NULL,
    [NumberOfCalls]          INT             NULL,
    [PortOut]                NVARCHAR (50)   NULL,
    [PortIn]                 NVARCHAR (50)   NULL,
    [DeliveredAttempts]      INT             NULL,
    [SuccessfulAttempts]     INT             NULL,
    [DeliveredNumberOfCalls] INT             NOT NULL,
    [CeiledDuration]         BIGINT          NOT NULL,
    [SwitchID]               INT             NULL,
    [SumOfPGAD]              INT             NULL,
    [UtilizationInSeconds]   NUMERIC (19, 5) NULL
);


GO
CREATE CLUSTERED INDEX [IX_TrafficStats_DateTimeFirst]
    ON [TOneWhS_Analytic].[TrafficStatsByCode]([FirstCDRAttempt] DESC);

