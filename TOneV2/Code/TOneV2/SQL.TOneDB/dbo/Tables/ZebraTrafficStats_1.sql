CREATE TABLE [dbo].[ZebraTrafficStats] (
    [ID]                     INT             NULL,
    [Port_IN]                VARCHAR (42)    NULL,
    [Port_OUT]               VARCHAR (42)    NULL,
    [CustomerID]             VARCHAR (10)    NULL,
    [SupplierID]             VARCHAR (10)    NULL,
    [OurZoneID]              INT             NULL,
    [OurZoneName]            VARCHAR (50)    NULL,
    [SupplierZoneID]         INT             NULL,
    [SupplierZoneName]       VARCHAR (50)    NULL,
    [FirstCDRAttempt]        DATETIME        CONSTRAINT [DF_ZebraTrafficStats_FirstCDRAttempt] DEFAULT (getdate()) NOT NULL,
    [LastCDRAttempt]         DATETIME        NOT NULL,
    [Attempts]               INT             NOT NULL,
    [DeliveredAttempts]      INT             NOT NULL,
    [SuccessfulAttempts]     INT             NOT NULL,
    [DurationsInSeconds]     NUMERIC (19, 5) NOT NULL,
    [PDDInSeconds]           NUMERIC (19, 5) NULL,
    [MaxDurationInSeconds]   NUMERIC (19, 5) CONSTRAINT [DF_ZebraTrafficStats_MaxDurationInSeconds] DEFAULT ((0)) NULL,
    [UtilizationInSeconds]   NUMERIC (19, 5) NULL,
    [NumberOfCalls]          INT             NULL,
    [DeliveredNumberOfCalls] INT             NULL,
    [PGAD]                   NUMERIC (19, 5) NULL,
    [CeiledDuration]         BIGINT          NULL
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'The output IP:Port or Trunk ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ZebraTrafficStats', @level2type = N'COLUMN', @level2name = N'Port_OUT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'The input IP:Port or Trunk ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ZebraTrafficStats', @level2type = N'COLUMN', @level2name = N'Port_IN';

