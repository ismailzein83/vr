CREATE TABLE [TOneWhS_BE_Bkup].[SupplierCode] (
    [ID]            BIGINT       NOT NULL,
    [Code]          VARCHAR (20) NOT NULL,
    [ZoneID]        BIGINT       NOT NULL,
    [CodeGroupID]   INT          NULL,
    [BED]           DATETIME     NOT NULL,
    [EED]           DATETIME     NULL,
    [SourceID]      VARCHAR (50) NULL,
    [StateBackupID] INT          NOT NULL,
    CONSTRAINT [FK_SupplierCode_SupplierZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE_Bkup].[SupplierZone] ([ID]),
    CONSTRAINT [IX_SupplierCode_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);

