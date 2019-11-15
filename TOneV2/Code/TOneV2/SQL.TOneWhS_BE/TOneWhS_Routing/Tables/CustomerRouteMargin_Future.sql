CREATE TABLE [TOneWhS_Routing].[CustomerRouteMargin_Future] (
    [ID]                        BIGINT           NOT NULL,
    [CustomerID]                INT              NOT NULL,
    [SaleZoneID]                BIGINT           NOT NULL,
    [SaleRate]                  DECIMAL (20, 8)  NOT NULL,
    [SaleDealID]                INT              NULL,
    [IsRisky]                   BIT              NOT NULL,
    [SupplierZoneID]            BIGINT           NULL,
    [SupplierServiceIDs]        NVARCHAR (MAX)   NOT NULL,
    [SupplierRate]              DECIMAL (20, 8)  NOT NULL,
    [SupplierDealID]            INT              NULL,
    [Margin]                    DECIMAL (20, 8)  NOT NULL,
    [MarginCategoryID]          UNIQUEIDENTIFIER NULL,
    [OptimalSupplierZoneID]     BIGINT           NULL,
    [OptimalSupplierServiceIDs] NVARCHAR (MAX)   NOT NULL,
    [OptimalSupplierRate]       DECIMAL (20, 8)  NOT NULL,
    [OptimalSupplierDealID]     INT              NULL,
    [OptimalMargin]             DECIMAL (20, 8)  NOT NULL,
    [OptimalMarginCategoryID]   UNIQUEIDENTIFIER NULL,
    [CreatedTime]               DATETIME         NOT NULL,
    CONSTRAINT [PK_CustomerRouteMargin_ID_ccd15c93996a47569bca5a0b0c59ff97] UNIQUE CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerRouteMargin_SupplierZoneID_ccd15c93996a47569bca5a0b0c59ff97]
    ON [TOneWhS_Routing].[CustomerRouteMargin_Future]([SupplierZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerRouteMargin_SaleZoneID_ccd15c93996a47569bca5a0b0c59ff97]
    ON [TOneWhS_Routing].[CustomerRouteMargin_Future]([SaleZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerRouteMargin_CustomerID_SaleZoneID_ccd15c93996a47569bca5a0b0c59ff97]
    ON [TOneWhS_Routing].[CustomerRouteMargin_Future]([CustomerID] ASC, [SaleZoneID] ASC);

