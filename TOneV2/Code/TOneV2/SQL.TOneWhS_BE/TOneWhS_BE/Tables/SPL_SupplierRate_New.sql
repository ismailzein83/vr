CREATE TABLE [TOneWhS_BE].[SPL_SupplierRate_New] (
    [ID]          BIGINT         NULL,
    [PriceListID] INT            NOT NULL,
    [ZoneID]      BIGINT         NOT NULL,
    [CurrencyID]  INT            NULL,
    [NormalRate]  DECIMAL (9, 5) NOT NULL,
    [OtherRates]  VARCHAR (MAX)  NULL,
    [BED]         DATETIME       NOT NULL,
    [EED]         DATETIME       NULL
);

