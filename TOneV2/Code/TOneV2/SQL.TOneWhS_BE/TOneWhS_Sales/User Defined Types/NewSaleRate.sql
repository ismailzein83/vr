CREATE TYPE [TOneWhS_Sales].[NewSaleRate] AS TABLE (
    [ID]          BIGINT         NULL,
    [PriceListID] INT            NOT NULL,
    [ZoneID]      BIGINT         NOT NULL,
    [Rate]        DECIMAL (9, 5) NOT NULL,
    [BED]         DATETIME       NOT NULL,
    [EED]         DATETIME       NULL);

