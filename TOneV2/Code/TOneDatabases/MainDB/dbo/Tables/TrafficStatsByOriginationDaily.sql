CREATE TABLE [dbo].[TrafficStatsByOriginationDaily] (
    [ID]                     BIGINT          IDENTITY (1, 1) NOT NULL,
    [Calldate]               DATETIME        NOT NULL,
    [SwitchId]               TINYINT         NOT NULL,
    [CustomerID]             VARCHAR (10)    NULL,
    [OriginatingZoneID]      INT             NULL,
    [SupplierID]             VARCHAR (10)    NULL,
    [SupplierZoneID]         INT             NULL,
    [Attempts]               INT             NOT NULL,
    [DeliveredAttempts]      INT             NOT NULL,
    [SuccessfulAttempts]     INT             NOT NULL,
    [DurationsInSeconds]     NUMERIC (19, 5) NOT NULL,
    [PDDInSeconds]           NUMERIC (19, 5) NULL,
    [UtilizationInSeconds]   NUMERIC (19, 5) NULL,
    [NumberOfCalls]          INT             NULL,
    [DeliveredNumberOfCalls] INT             NULL,
    [PGAD]                   NUMERIC (19, 5) CONSTRAINT [DF_OriginatingTrafficStatsDaily_PGAD1] DEFAULT ((0)) NULL,
    [CeiledDuration]         BIGINT          NULL
);

