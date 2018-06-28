CREATE TABLE [TOneWhS_BE_Bkup].[SupplierPriceList] (
    [ID]                INT          NOT NULL,
    [SupplierID]        INT          NOT NULL,
    [CurrencyID]        INT          NOT NULL,
    [FileID]            BIGINT       NULL,
    [EffectiveOn]       DATETIME     NULL,
    [PricelistType]     TINYINT      NULL,
    [CreatedTime]       DATETIME     CONSTRAINT [DF_SupplierPriceList_CreatedDate] DEFAULT (getdate()) NULL,
    [SourceID]          VARCHAR (50) NULL,
    [ProcessInstanceID] BIGINT       NULL,
    [SPLStateBackupID]  BIGINT       NULL,
    [UserID]            INT          NULL,
    [StateBackupID]     BIGINT       NULL
);














GO
CREATE CLUSTERED INDEX [IX_SupplierPriceList_StateBackupID]
    ON [TOneWhS_BE_Bkup].[SupplierPriceList]([StateBackupID] ASC);

