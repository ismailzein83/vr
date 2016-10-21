CREATE TABLE [TOneWhS_BE_Bkup].[SupplierRate] (
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
    [StateBackupID] INT             NOT NULL,
    CONSTRAINT [PK_SupplierRate] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SupplierRate_SupplierPriceList] FOREIGN KEY ([PriceListID]) REFERENCES [TOneWhS_BE_Bkup].[SupplierPriceList] ([ID]),
    CONSTRAINT [FK_SupplierRate_SupplierZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE_Bkup].[SupplierZone] ([ID])
);

