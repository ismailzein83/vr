CREATE TABLE [TOneWhS_BE].[SupplierCode] (
    [ID]               BIGINT       NOT NULL,
    [Code]             VARCHAR (20) NOT NULL,
    [ZoneID]           BIGINT       NOT NULL,
    [CodeGroupID]      INT          NULL,
    [BED]              DATETIME     NOT NULL,
    [EED]              DATETIME     NULL,
    [timestamp]        ROWVERSION   NULL,
    [SourceID]         VARCHAR (50) NULL,
    [LastModifiedTime] DATETIME     CONSTRAINT [DF_SupplierCode_LastModifiedTime] DEFAULT (getdate()) NULL,
    [CreatedTime]      DATETIME     CONSTRAINT [DF_SupplierCode_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [FK_SupplierCode_CodeGroup] FOREIGN KEY ([CodeGroupID]) REFERENCES [TOneWhS_BE].[CodeGroup] ([ID]),
    CONSTRAINT [FK_SupplierCode_SupplierZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SupplierZone] ([ID]),
    CONSTRAINT [IX_SupplierCode_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);
















GO
CREATE NONCLUSTERED INDEX [IX_SupplierCode_timestamp]
    ON [TOneWhS_BE].[SupplierCode]([timestamp] DESC);


GO
CREATE CLUSTERED INDEX [IX_SupplierCode_Code]
    ON [TOneWhS_BE].[SupplierCode]([Code] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SupplierCode_ZoneID]
    ON [TOneWhS_BE].[SupplierCode]([ZoneID] ASC);

