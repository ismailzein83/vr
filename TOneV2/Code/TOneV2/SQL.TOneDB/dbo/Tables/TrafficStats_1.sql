CREATE TABLE [dbo].[TrafficStats] (
    [ID]                     BIGINT          IDENTITY (1, 1) NOT NULL,
    [SwitchId]               TINYINT         NOT NULL,
    [Port_IN]                VARCHAR (42)    NULL,
    [Port_OUT]               VARCHAR (42)    NULL,
    [CustomerID]             VARCHAR (10)    NULL,
    [OurZoneID]              INT             NULL,
    [OriginatingZoneID]      INT             NULL,
    [SupplierID]             VARCHAR (10)    NULL,
    [SupplierZoneID]         INT             NULL,
    [FirstCDRAttempt]        DATETIME        CONSTRAINT [DF_TrafficMonitor_SampleDate] DEFAULT (getdate()) NOT NULL,
    [LastCDRAttempt]         DATETIME        NOT NULL,
    [Attempts]               INT             NOT NULL,
    [DeliveredAttempts]      INT             NOT NULL,
    [SuccessfulAttempts]     INT             NOT NULL,
    [DurationsInSeconds]     NUMERIC (19, 5) NOT NULL,
    [PDDInSeconds]           NUMERIC (19, 5) NULL,
    [MaxDurationInSeconds]   NUMERIC (19, 5) CONSTRAINT [DF_TrafficStats_MaxDurationInSeconds] DEFAULT ((0)) NULL,
    [UtilizationInSeconds]   NUMERIC (19, 5) NULL,
    [NumberOfCalls]          INT             NULL,
    [DeliveredNumberOfCalls] INT             NULL,
    [PGAD]                   NUMERIC (19, 5) NULL,
    [CeiledDuration]         BIGINT          NULL,
    [ReleaseSourceAParty]    INT             NULL,
    [ReleaseSourceS]         INT             CONSTRAINT [DF_TrafficStats_ReleaseSourceS] DEFAULT ((0)) NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TrafficStats_ID]
    ON [dbo].[TrafficStats]([ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TrafficStats_Supplier]
    ON [dbo].[TrafficStats]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TrafficStats_Customer]
    ON [dbo].[TrafficStats]([CustomerID] ASC);


GO
CREATE CLUSTERED INDEX [IX_TrafficStats_DateTimeFirst]
    ON [dbo].[TrafficStats]([FirstCDRAttempt] DESC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'The output IP:Port or Trunk ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TrafficStats', @level2type = N'COLUMN', @level2name = N'Port_OUT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'The input IP:Port or Trunk ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TrafficStats', @level2type = N'COLUMN', @level2name = N'Port_IN';

