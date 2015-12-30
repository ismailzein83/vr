CREATE TABLE [TOneWhS_CDR].[CDRFailed] (
    [ID]                BIGINT        NOT NULL,
    [CustomerID]        INT           NULL,
    [SupplierID]        INT           NULL,
    [Attempt]           DATETIME      NULL,
    [DurationInSeconds] INT           NULL,
    [Alert]             DATETIME      NULL,
    [Connect]           DATETIME      NULL,
    [Disconnect]        DATETIME      NULL,
    [PortOut]           NVARCHAR (50) NULL,
    [PortIn]            NVARCHAR (50) NULL,
    [SaleCode]          NVARCHAR (50) NULL,
    [SaleZoneID]        BIGINT        NULL,
    [SupplierCode]      NVARCHAR (50) NULL,
    [SupplierZoneID]    BIGINT        NULL,
    [CDPN]              NVARCHAR (50) NULL,
    [CGPN]              NVARCHAR (50) NULL,
    [ReleaseCode]       NVARCHAR (50) NULL,
    [ReleaseSource]     NVARCHAR (50) NULL,
    [SwitchID]          INT           NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_CDRFailed_SupplierID]
    ON [TOneWhS_CDR].[CDRFailed]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CDRFailed_SaleZoneID]
    ON [TOneWhS_CDR].[CDRFailed]([SaleZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CDRFailed_Customer]
    ON [TOneWhS_CDR].[CDRFailed]([CustomerID] ASC);


GO
CREATE CLUSTERED INDEX [IX_CDRFailed_Attempt]
    ON [TOneWhS_CDR].[CDRFailed]([Attempt] DESC);

