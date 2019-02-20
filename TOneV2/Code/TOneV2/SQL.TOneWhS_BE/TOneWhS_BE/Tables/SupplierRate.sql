﻿CREATE TABLE [TOneWhS_BE].[SupplierRate] (
    [ID]               BIGINT          NOT NULL,
    [PriceListID]      INT             NOT NULL,
    [ZoneID]           BIGINT          NOT NULL,
    [CurrencyID]       INT             NULL,
    [Rate]             DECIMAL (20, 8) NOT NULL,
    [RateTypeID]       INT             NULL,
    [Change]           INT             NULL,
    [BED]              DATETIME        NOT NULL,
    [EED]              DATETIME        NULL,
    [SourceID]         VARCHAR (50)    NULL,
    [timestamp]        ROWVERSION      NULL,
    [LastModifiedTime] DATETIME        CONSTRAINT [DF_SupplierRate_LastModifiedTime] DEFAULT (getdate()) NULL,
    [CreatedTime]      DATETIME        CONSTRAINT [DF_SupplierRate_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_SupplierRate] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SupplierRate_SupplierPriceList] FOREIGN KEY ([PriceListID]) REFERENCES [TOneWhS_BE].[SupplierPriceList] ([ID]),
    CONSTRAINT [FK_SupplierRate_SupplierZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SupplierZone] ([ID])
);
























GO
CREATE NONCLUSTERED INDEX [IX_SupplierRate_timestamp]
    ON [TOneWhS_BE].[SupplierRate]([timestamp] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_SupplierRate_ZoneID]
    ON [TOneWhS_BE].[SupplierRate]([ZoneID] ASC);

