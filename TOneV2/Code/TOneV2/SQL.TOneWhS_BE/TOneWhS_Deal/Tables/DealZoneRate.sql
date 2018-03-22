CREATE TABLE [TOneWhS_Deal].[DealZoneRate] (
    [ID]          BIGINT          NOT NULL,
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
    CONSTRAINT [PK_DealZoneRate] PRIMARY KEY CLUSTERED ([ID] ASC)
);






GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_IsSale]
    ON [TOneWhS_Deal].[DealZoneRate]([IsSale] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_EED]
    ON [TOneWhS_Deal].[DealZoneRate]([EED] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_DealId]
    ON [TOneWhS_Deal].[DealZoneRate]([DealId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_BED]
    ON [TOneWhS_Deal].[DealZoneRate]([BED] ASC);

