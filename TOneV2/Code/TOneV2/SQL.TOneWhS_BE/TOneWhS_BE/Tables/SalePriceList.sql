CREATE TABLE [TOneWhS_BE].[SalePriceList] (
    [ID]            INT          NOT NULL,
    [OwnerType]     INT          NOT NULL,
    [OwnerID]       INT          NOT NULL,
    [CurrencyID]    INT          NOT NULL,
    [EffectiveOn]   DATETIME     NULL,
    [PriceListType] TINYINT      NULL,
    [timestamp]     ROWVERSION   NULL,
    [SourceID]      VARCHAR (50) NULL,
    CONSTRAINT [PK_SalePriceList] PRIMARY KEY CLUSTERED ([ID] ASC)
);














GO
CREATE NONCLUSTERED INDEX [IX_SalePriceList_timestamp]
    ON [TOneWhS_BE].[SalePriceList]([timestamp] DESC);

