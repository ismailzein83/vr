CREATE TABLE [NetworkRentalManager].[ProductSale] (
    [ID]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [CustomerID]      BIGINT          NULL,
    [ProductId]       BIGINT          NULL,
    [Quantity]        DECIMAL (20, 6) NULL,
    [UnitPrice]       DECIMAL (24, 8) NULL,
    [TotalPrice]      DECIMAL (24, 8) NULL,
    [CreatedTime]     DATETIME        NULL,
    [BED]             DATETIME        NULL,
    [EED]             DATETIME        NULL,
    [RecurringPeriod] INT             NULL,
    [Description]     NVARCHAR (1000) NULL,
    [CurrencyID]      INT             NULL,
    [QuantityUnit]    INT             NULL,
    CONSTRAINT [PK_ProductSale] PRIMARY KEY CLUSTERED ([ID] ASC)
);

