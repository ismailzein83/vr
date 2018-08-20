CREATE TABLE [TOneWhS_Deal].[DealZoneRate] (
    [ID]          BIGINT          IDENTITY (1, 1) NOT NULL,
    [DealId]      INT             NOT NULL,
    [ZoneGroupNb] INT             NOT NULL,
    [IsSale]      BIT             NOT NULL,
    [TierNb]      INT             NOT NULL,
    [ZoneId]      BIGINT          NOT NULL,
    [Rate]        DECIMAL (20, 8) NOT NULL,
    [CurrencyId]  INT             NOT NULL,
    [BED]         DATETIME        NOT NULL,
    [EED]         DATETIME        NULL,
    [timestamp]   ROWVERSION      NULL,
    CONSTRAINT [PK_DealZoneRate_1f656041-0388-49d8-a5c7-8336c343ca98] PRIMARY KEY CLUSTERED ([ID] ASC)
);


























GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_IsSale_1f656041-0388-49d8-a5c7-8336c343ca98]
    ON [TOneWhS_Deal].[DealZoneRate]([IsSale] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_EED_1f656041-0388-49d8-a5c7-8336c343ca98]
    ON [TOneWhS_Deal].[DealZoneRate]([EED] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_DealId_1f656041-0388-49d8-a5c7-8336c343ca98]
    ON [TOneWhS_Deal].[DealZoneRate]([DealId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_BED_1f656041-0388-49d8-a5c7-8336c343ca98]
    ON [TOneWhS_Deal].[DealZoneRate]([BED] ASC);

