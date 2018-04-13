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
    CONSTRAINT [PK_DealZoneRate_14258c99-a77f-4ff9-a2db-1bbb73e3707b] PRIMARY KEY CLUSTERED ([ID] ASC)
);










GO



GO



GO



GO



GO



GO



GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_IsSale_14258c99-a77f-4ff9-a2db-1bbb73e3707b]
    ON [TOneWhS_Deal].[DealZoneRate]([IsSale] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_EED_14258c99-a77f-4ff9-a2db-1bbb73e3707b]
    ON [TOneWhS_Deal].[DealZoneRate]([EED] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_DealId_14258c99-a77f-4ff9-a2db-1bbb73e3707b]
    ON [TOneWhS_Deal].[DealZoneRate]([DealId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_BED_14258c99-a77f-4ff9-a2db-1bbb73e3707b]
    ON [TOneWhS_Deal].[DealZoneRate]([BED] ASC);

