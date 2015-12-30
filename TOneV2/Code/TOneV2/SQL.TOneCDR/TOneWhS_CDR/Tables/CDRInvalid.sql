CREATE TABLE [TOneWhS_CDR].[CDRInvalid] (
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
CREATE NONCLUSTERED INDEX [IX_CDRInvalid_SaleZoneID]
    ON [TOneWhS_CDR].[CDRInvalid]([SaleZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CDRInvalid_Customer]
    ON [TOneWhS_CDR].[CDRInvalid]([CustomerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CDRInvalid_SupplierID]
    ON [TOneWhS_CDR].[CDRInvalid]([SupplierID] ASC);


GO
CREATE CLUSTERED INDEX [IX_CDRInvalid_Attempt]
    ON [TOneWhS_CDR].[CDRInvalid]([Attempt] DESC);

