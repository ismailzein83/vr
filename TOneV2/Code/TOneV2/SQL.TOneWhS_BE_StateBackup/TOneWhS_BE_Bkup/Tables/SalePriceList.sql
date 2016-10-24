CREATE TABLE [TOneWhS_BE_Bkup].[SalePriceList] (
    [ID]            INT          NOT NULL,
    [OwnerType]     INT          NOT NULL,
    [OwnerID]       INT          NOT NULL,
    [CurrencyID]    INT          NOT NULL,
    [EffectiveOn]   DATETIME     NULL,
    [SourceID]      VARCHAR (50) NULL,
    [StateBackupID] BIGINT       NOT NULL,
    CONSTRAINT [PK_SalePriceList] PRIMARY KEY CLUSTERED ([ID] ASC)
);



