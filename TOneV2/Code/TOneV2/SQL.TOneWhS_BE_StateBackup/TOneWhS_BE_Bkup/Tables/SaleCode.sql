CREATE TABLE [TOneWhS_BE_Bkup].[SaleCode] (
    [ID]            BIGINT       NOT NULL,
    [Code]          VARCHAR (20) NOT NULL,
    [ZoneID]        BIGINT       NOT NULL,
    [CodeGroupID]   INT          NULL,
    [BED]           DATETIME     NOT NULL,
    [EED]           DATETIME     NULL,
    [SourceID]      VARCHAR (50) NULL,
    [StateBackupID] INT          NOT NULL,
    CONSTRAINT [FK_SaleCode_SaleZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE_Bkup].[SaleZone] ([ID]),
    CONSTRAINT [IX_SaleCode_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);

