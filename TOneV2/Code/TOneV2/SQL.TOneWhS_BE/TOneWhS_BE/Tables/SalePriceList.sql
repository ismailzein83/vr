CREATE TABLE [TOneWhS_BE].[SalePriceList] (
    [ID]          INT          IDENTITY (1, 1) NOT NULL,
    [OwnerType]   INT          NOT NULL,
    [OwnerID]     INT          NOT NULL,
    [CurrencyID]  INT          NOT NULL,
    [EffectiveOn] DATETIME     NULL,
    [timestamp]   ROWVERSION   NULL,
    [SourceID]    VARCHAR (50) NULL,
    CONSTRAINT [PK_SalePriceList] PRIMARY KEY CLUSTERED ([ID] ASC)
);










GO
CREATE NONCLUSTERED INDEX [IX_SalePriceList_timestamp]
    ON [TOneWhS_BE].[SalePriceList]([timestamp] DESC);

