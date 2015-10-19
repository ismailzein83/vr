CREATE TYPE [TOneWhS_BE].[CustomerPricingProduct] AS TABLE (
    [ID]               INT      NULL,
    [CustomerId]       INT      NOT NULL,
    [PricingProductId] INT      NOT NULL,
    [AllDestinations]  BIT      NOT NULL,
    [BED]              DATETIME NOT NULL,
    [EED]              DATETIME NULL);

