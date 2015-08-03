CREATE TABLE [dbo].[TrafficStatsDaily] (
    [ID]                     BIGINT          IDENTITY (1, 1) NOT NULL,
    [Calldate]               DATE            NULL,
    [SwitchId]               TINYINT         NOT NULL,
    [CustomerID]             VARCHAR (10)    NULL,
    [OurZoneID]              INT             NULL,
    [OriginatingZoneID]      INT             NULL,
    [SupplierID]             VARCHAR (10)    NULL,
    [SupplierZoneID]         INT             NULL,
    [Attempts]               INT             NOT NULL,
    [DeliveredAttempts]      INT             NOT NULL,
    [SuccessfulAttempts]     INT             NOT NULL,
    [DurationsInSeconds]     NUMERIC (19, 5) NOT NULL,
    [PDDInSeconds]           NUMERIC (9, 2)  NULL,
    [UtilizationInSeconds]   NUMERIC (19, 5) NULL,
    [NumberOfCalls]          INT             NULL,
    [DeliveredNumberOfCalls] INT             NULL,
    [PGAD]                   NUMERIC (9, 2)  NULL,
    [CeiledDuration]         BIGINT          NULL,
    [MaxDurationInSeconds]   NUMERIC (9, 2)  NULL,
    [ReleaseSourceAParty]    INT             NULL,
    [ReleaseSourceS]         INT             CONSTRAINT [DF_TrafficStatsDaily_ReleaseSourceS] DEFAULT ((0)) NULL
);


GO
CREATE CLUSTERED INDEX [IX_TrafficStatsDaily_DateTimeFirst]
    ON [dbo].[TrafficStatsDaily]([Calldate] ASC);

