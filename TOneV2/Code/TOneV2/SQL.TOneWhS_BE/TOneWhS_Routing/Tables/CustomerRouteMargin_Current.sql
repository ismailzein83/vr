CREATE TABLE [TOneWhS_Routing].[CustomerRouteMargin_Current] (
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
    CONSTRAINT [PK_CustomerRouteMargin_ID_5bbe65a7bb8c4b64b47b7b26eaa74540] UNIQUE CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerRouteMargin_SupplierZoneID_5bbe65a7bb8c4b64b47b7b26eaa74540]
    ON [TOneWhS_Routing].[CustomerRouteMargin_Current]([SupplierZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerRouteMargin_SaleZoneID_5bbe65a7bb8c4b64b47b7b26eaa74540]
    ON [TOneWhS_Routing].[CustomerRouteMargin_Current]([SaleZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerRouteMargin_CustomerID_SaleZoneID_5bbe65a7bb8c4b64b47b7b26eaa74540]
    ON [TOneWhS_Routing].[CustomerRouteMargin_Current]([CustomerID] ASC, [SaleZoneID] ASC);

