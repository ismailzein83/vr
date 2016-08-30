CREATE TABLE [TOneWhS_BE].[SaleCode] (
    [ID]          BIGINT       NOT NULL,
    [Code]        VARCHAR (20) NOT NULL,
    [ZoneID]      BIGINT       NOT NULL,
    [CodeGroupID] INT          NULL,
    [BED]         DATETIME     NOT NULL,
    [EED]         DATETIME     NULL,
    [timestamp]   ROWVERSION   NULL,
    [SourceID]    VARCHAR (50) NULL,
    CONSTRAINT [FK_SaleCode_CodeGroup] FOREIGN KEY ([CodeGroupID]) REFERENCES [TOneWhS_BE].[CodeGroup] ([ID]),
    CONSTRAINT [FK_SaleCode_SaleZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SaleZone] ([ID]),
    CONSTRAINT [IX_SaleCode_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);












GO
CREATE NONCLUSTERED INDEX [IX_SaleCode_timestamp]
    ON [TOneWhS_BE].[SaleCode]([timestamp] DESC);


GO
CREATE CLUSTERED INDEX [IX_SaleCode_Code]
    ON [TOneWhS_BE].[SaleCode]([Code] ASC);

