﻿CREATE TABLE [TOneWhS_Analytic].[TrafficStatsWeekly] (
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
    CONSTRAINT [IX_TrafficStatsWeekly_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);

