﻿CREATE TABLE [TOneWhS_BE_Bkup].[SaleRate] (
    [ID]               BIGINT          NOT NULL,
    [PriceListID]      INT             NOT NULL,
    [ZoneID]           BIGINT          NOT NULL,
    [CurrencyID]       INT             NULL,
    [RateTypeID]       INT             NULL,
    [Rate]             DECIMAL (20, 8) NOT NULL,
    [BED]              DATETIME        NOT NULL,
    [EED]              DATETIME        NULL,
    [SourceID]         VARCHAR (50)    NULL,
    [Change]           INT             NULL,
    [StateBackupID]    BIGINT          NOT NULL,
    [LastModifiedTime] DATETIME        NULL
);












GO
CREATE CLUSTERED INDEX [IX_SaleRate_StateBackupID]
    ON [TOneWhS_BE_Bkup].[SaleRate]([StateBackupID] ASC);

