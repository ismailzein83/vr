CREATE TABLE [TOneWhS_BE_Bkup].[SalePriceList] (
    [ID]                INT          NOT NULL,
    [OwnerType]         INT          NOT NULL,
    [OwnerID]           INT          NOT NULL,
    [CurrencyID]        INT          NOT NULL,
    [EffectiveOn]       DATETIME     NULL,
    [PriceListType]     TINYINT      NULL,
    [SourceID]          VARCHAR (50) NULL,
    [ProcessInstanceID] BIGINT       NULL,
    [FileID]            BIGINT       NULL,
    [CreatedTime]       DATETIME     CONSTRAINT [DF_SalePriceList_CreatedTime_1] DEFAULT (getdate()) NULL,
    [StateBackupID]     BIGINT       NOT NULL
);










GO
CREATE CLUSTERED INDEX [IX_SalePriceList_StateBackupID]
    ON [TOneWhS_BE_Bkup].[SalePriceList]([StateBackupID] ASC);

