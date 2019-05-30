CREATE TYPE [TOneWhS_CDR].[vw_CDRProblemsType] AS TABLE (
    [CDRId]                  BIGINT          NULL,
    [AttemptDateTime]        DATETIME        NULL,
    [DurationInSeconds]      DECIMAL (20, 4) NULL,
    [CustomerID]             INT             NULL,
    [SaleZoneID]             BIGINT          NULL,
    [SupplierID]             INT             NULL,
    [SupplierZoneID]         BIGINT          NULL,
    [CDPN]                   VARCHAR (50)    NULL,
    [OrigCDPNOut]            VARCHAR (50)    NULL,
    [CostRateId]             BIGINT          NULL,
    [SaleRateId]             BIGINT          NULL,
    [SaleFinancialAccountId] INT             NULL,
    [CostFinancialAccountId] INT             NULL);

