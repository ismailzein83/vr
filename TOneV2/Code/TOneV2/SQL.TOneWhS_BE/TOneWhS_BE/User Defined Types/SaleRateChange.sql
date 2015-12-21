CREATE TYPE [TOneWhS_BE].[SaleRateChange] AS TABLE (
    [RateId] BIGINT   NOT NULL,
    [EED]    DATETIME NULL,
    PRIMARY KEY CLUSTERED ([RateId] ASC));

