CREATE TABLE [dbo].[TrafficStatsByCode] (
    [ID]                     BIGINT          IDENTITY (1, 1) NOT NULL,
    [SwitchId]               TINYINT         NOT NULL,
    [CustomerID]             VARCHAR (10)    NULL,
    [OurCode]                VARCHAR (20)    NULL,
    [OurZoneID]              INT             NULL,
    [OriginatingZoneID]      INT             NULL,
    [SupplierID]             VARCHAR (10)    NULL,
    [SupplierZoneID]         INT             NULL,
    [FirstCDRAttempt]        DATETIME        NOT NULL,
    [LastCDRAttempt]         DATETIME        NOT NULL,
    [Attempts]               INT             NOT NULL,
    [DeliveredAttempts]      INT             NOT NULL,
    [SuccessfulAttempts]     INT             NOT NULL,
    [DurationsInSeconds]     NUMERIC (19, 5) NOT NULL,
    [PDDInSeconds]           NUMERIC (19, 5) NULL,
    [MaxDurationInSeconds]   NUMERIC (19, 5) NULL,
    [UtilizationInSeconds]   NUMERIC (19, 5) NULL,
    [NumberOfCalls]          INT             NULL,
    [DeliveredNumberOfCalls] INT             NULL,
    [PGAD]                   NUMERIC (19, 5) NULL,
    [CeiledDuration]         BIGINT          NULL,
    [ReleaseSourceAParty]    INT             NULL,
    [SupplierCode]           VARCHAR (50)    NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_TrafficStatsByCode_Supplier]
    ON [dbo].[TrafficStatsByCode]([SupplierID] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TrafficStatsByCode_ID]
    ON [dbo].[TrafficStatsByCode]([ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TrafficStatsByCode_Customer]
    ON [dbo].[TrafficStatsByCode]([CustomerID] ASC);


GO
CREATE CLUSTERED INDEX [IX_TrafficStatsByCode_DateTimeFirst]
    ON [dbo].[TrafficStatsByCode]([FirstCDRAttempt] ASC);

