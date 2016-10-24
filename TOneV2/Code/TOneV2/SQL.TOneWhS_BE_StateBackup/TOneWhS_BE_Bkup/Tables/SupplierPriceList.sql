CREATE TABLE [TOneWhS_BE_Bkup].[SupplierPriceList] (
    [ID]            INT          NOT NULL,
    [SupplierID]    INT          NOT NULL,
    [CurrencyID]    INT          NOT NULL,
    [FileID]        BIGINT       NULL,
    [EffectiveOn]   DATETIME     NULL,
    [CreatedTime]   DATETIME     CONSTRAINT [DF_SupplierPriceList_CreatedDate] DEFAULT (getdate()) NULL,
    [SourceID]      VARCHAR (50) NULL,
    [StateBackupID] BIGINT       NULL
);





