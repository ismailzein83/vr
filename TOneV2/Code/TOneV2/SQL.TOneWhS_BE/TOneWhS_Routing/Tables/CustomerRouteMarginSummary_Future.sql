CREATE TABLE [TOneWhS_Routing].[CustomerRouteMarginSummary_Future] (
    [ID]                  BIGINT           NOT NULL,
    [CustomerID]          INT              NOT NULL,
    [SaleZoneID]          BIGINT           NOT NULL,
    [SaleRate]            DECIMAL (20, 8)  NOT NULL,
    [SaleDealID]          INT              NULL,
    [MinSupplierRate]     DECIMAL (20, 8)  NOT NULL,
    [MaxMargin]           DECIMAL (20, 8)  NOT NULL,
    [MaxMarginCategoryID] UNIQUEIDENTIFIER NULL,
    [MaxSupplierRate]     DECIMAL (20, 8)  NOT NULL,
    [MinMargin]           DECIMAL (20, 8)  NOT NULL,
    [MinMarginCategoryID] UNIQUEIDENTIFIER NULL,
    [AvgSupplierRate]     DECIMAL (20, 8)  NOT NULL,
    [AvgMargin]           DECIMAL (20, 8)  NOT NULL,
    [AvgMarginCategoryID] UNIQUEIDENTIFIER NULL,
    [CreatedTime]         DATETIME         NOT NULL,
    CONSTRAINT [PK_CustomerRouteMarginSummary_ID_133de3d960844d21b248a1b01c4c4872] UNIQUE CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerRouteMarginSummary_SaleZoneID_133de3d960844d21b248a1b01c4c4872]
    ON [TOneWhS_Routing].[CustomerRouteMarginSummary_Future]([SaleZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerRouteMarginSummary_CustomerID_SaleZoneID_133de3d960844d21b248a1b01c4c4872]
    ON [TOneWhS_Routing].[CustomerRouteMarginSummary_Future]([CustomerID] ASC, [SaleZoneID] ASC);

