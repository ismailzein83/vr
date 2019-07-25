CREATE TABLE [TOneWhS_CallQuality].[MappedCallQuality] (
    [CallQualityId]   BIGINT       NULL,
    [AttemptDateTime] DATETIME     NULL,
    [IsCLI]           BIT          NULL,
    [IsFAS]           BIT          NULL,
    [CGPN]            VARCHAR (40) NULL,
    [CDPN]            VARCHAR (40) NULL,
    [SupplierId]      INT          NULL,
    [SupplierZoneId]  BIGINT       NULL,
    [SaleZoneId]      BIGINT       NULL
);

