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
    CONSTRAINT [PK_DealZoneRate_fa5aae9c-997e-4ef2-a128-e9bd9c913005] PRIMARY KEY CLUSTERED ([ID] ASC)
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
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_IsSale_fa5aae9c-997e-4ef2-a128-e9bd9c913005]
    ON [TOneWhS_Deal].[DealZoneRate]([IsSale] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_EED_fa5aae9c-997e-4ef2-a128-e9bd9c913005]
    ON [TOneWhS_Deal].[DealZoneRate]([EED] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_DealId_fa5aae9c-997e-4ef2-a128-e9bd9c913005]
    ON [TOneWhS_Deal].[DealZoneRate]([DealId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_BED_fa5aae9c-997e-4ef2-a128-e9bd9c913005]
    ON [TOneWhS_Deal].[DealZoneRate]([BED] ASC);

