CREATE TYPE [TOneWhS_BE].[CustomerSellingProduct] AS TABLE (
    [ID]               INT      NULL,
    [CustomerId]       INT      NOT NULL,
    [SellingProductId] INT      NOT NULL,
    [BED]              DATETIME NOT NULL);

