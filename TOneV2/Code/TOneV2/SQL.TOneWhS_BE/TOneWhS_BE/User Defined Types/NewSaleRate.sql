CREATE TYPE [TOneWhS_BE].[NewSaleRate] AS TABLE (
    [ZoneID]      BIGINT         NOT NULL,
    [PriceListID] INT            NOT NULL,
    [CurrencyID]  INT            NULL,
    [NormalRate]  DECIMAL (9, 5) NOT NULL,
    [BED]         DATETIME       NOT NULL,
    [EED]         DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([ZoneID] ASC));



