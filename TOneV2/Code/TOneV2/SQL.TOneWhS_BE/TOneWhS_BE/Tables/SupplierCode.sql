CREATE TABLE [TOneWhS_BE].[SupplierCode] (
    [ID]          BIGINT       NOT NULL,
    [Code]        VARCHAR (20) NOT NULL,
    [ZoneID]      BIGINT       NOT NULL,
    [CodeGroupID] INT          NULL,
    [BED]         DATETIME     NOT NULL,
    [EED]         DATETIME     NULL,
    [timestamp]   ROWVERSION   NULL,
    [SourceID]    VARCHAR (50) NULL,
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

