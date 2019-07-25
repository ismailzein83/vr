CREATE TABLE [TOneWhS_CallQuality].[AccumulatedCallQuality15Min] (
    [Id]             BIGINT   NULL,
    [BatchStart]     DATETIME NULL,
    [SupplierId]     INT      NULL,
    [SupplierZoneId] BIGINT   NULL,
    [SaleZoneId]     BIGINT   NULL,
    [NbOfRecords]    INT      NULL,
    [NbOfCLI]        INT      NULL,
    [NbOfFAS]        INT      NULL,
    [NbOfNonCLI]     INT      NULL,
    [NbOfNonFAS]     INT      NULL
);

