CREATE TABLE [TOneWhS_BE_Bkup].[SaleRate] (
    [ID]            BIGINT          NOT NULL,
    [PriceListID]   INT             NOT NULL,
    [ZoneID]        BIGINT          NOT NULL,
    [CurrencyID]    INT             NULL,
    [RateTypeID]    INT             NULL,
    [Rate]          DECIMAL (20, 8) NOT NULL,
    [BED]           DATETIME        NOT NULL,
    [EED]           DATETIME        NULL,
    [SourceID]      VARCHAR (50)    NULL,
    [Change]        TINYINT         NULL,
    [StateBackupID] BIGINT          NOT NULL
);





