﻿CREATE TABLE [TOneWhS_BE_Bkup].[SupplierRate] (
    [ID]            BIGINT          NOT NULL,
    [PriceListID]   INT             NOT NULL,
    [ZoneID]        BIGINT          NOT NULL,
    [CurrencyID]    INT             NULL,
    [Rate]          DECIMAL (20, 8) NOT NULL,
    [RateTypeID]    INT             NULL,
    [Change]        TINYINT         NULL,
    [BED]           DATETIME        NOT NULL,
    [EED]           DATETIME        NULL,
    [SourceID]      VARCHAR (50)    NULL,
    [StateBackupID] BIGINT          NOT NULL,
    CONSTRAINT [PK_SupplierRate] PRIMARY KEY CLUSTERED ([ID] ASC)
);



