﻿CREATE TABLE [TOneWhS_BE].[SupplierPriceList] (
    [ID]          INT          NOT NULL,
    [SupplierID]  INT          NOT NULL,
    [CurrencyID]  INT          NOT NULL,
    [FileID]      BIGINT       NOT NULL,
    [EffectiveOn] DATETIME     NULL,
    [CreatedTime] DATETIME     CONSTRAINT [DF_SupplierPriceList_CreatedDate] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION   NULL,
    [SourceID]    VARCHAR (50) NULL,
    CONSTRAINT [PK_SupplierPriceList] PRIMARY KEY CLUSTERED ([ID] ASC)
);
















GO
CREATE NONCLUSTERED INDEX [IX_SupplierPriceList_timestamp]
    ON [TOneWhS_BE].[SupplierPriceList]([timestamp] DESC);

