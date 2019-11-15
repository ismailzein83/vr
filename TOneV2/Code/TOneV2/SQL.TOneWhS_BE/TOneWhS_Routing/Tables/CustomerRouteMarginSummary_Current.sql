CREATE TABLE [TOneWhS_Routing].[CustomerRouteMarginSummary_Current] (
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
    CONSTRAINT [PK_CustomerRouteMarginSummary_ID_f6c9cab1f623449cad71bf274ee750c7] UNIQUE CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerRouteMarginSummary_SaleZoneID_f6c9cab1f623449cad71bf274ee750c7]
    ON [TOneWhS_Routing].[CustomerRouteMarginSummary_Current]([SaleZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerRouteMarginSummary_CustomerID_SaleZoneID_f6c9cab1f623449cad71bf274ee750c7]
    ON [TOneWhS_Routing].[CustomerRouteMarginSummary_Current]([CustomerID] ASC, [SaleZoneID] ASC);

