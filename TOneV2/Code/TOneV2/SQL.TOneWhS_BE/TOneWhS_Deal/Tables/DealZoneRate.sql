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
    CONSTRAINT [PK_DealZoneRate_728429e9-063a-4269-8c19-6a775347eec5] PRIMARY KEY CLUSTERED ([ID] ASC)
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
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_IsSale_728429e9-063a-4269-8c19-6a775347eec5]
    ON [TOneWhS_Deal].[DealZoneRate]([IsSale] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_EED_728429e9-063a-4269-8c19-6a775347eec5]
    ON [TOneWhS_Deal].[DealZoneRate]([EED] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_DealId_728429e9-063a-4269-8c19-6a775347eec5]
    ON [TOneWhS_Deal].[DealZoneRate]([DealId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DealZoneRate_BED_728429e9-063a-4269-8c19-6a775347eec5]
    ON [TOneWhS_Deal].[DealZoneRate]([BED] ASC);

