CREATE TYPE [Retail_BE].[AccountPackageRecurChargePeriodType] AS TABLE (
    [AccountPackageId] BIGINT   NOT NULL,
    [FromDate]         DATETIME NOT NULL,
    [ToDate]           DATETIME NOT NULL);

