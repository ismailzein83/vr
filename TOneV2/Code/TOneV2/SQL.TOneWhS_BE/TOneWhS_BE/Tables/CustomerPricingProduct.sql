CREATE TABLE [TOneWhS_BE].[CustomerPricingProduct] (
    [ID]               INT        IDENTITY (1, 1) NOT NULL,
    [CustomerID]       INT        NOT NULL,
    [PricingProductID] INT        NOT NULL,
    [AllDestinations]  BIT        NOT NULL,
    [BED]              DATETIME   NOT NULL,
    [EED]              DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    CONSTRAINT [PK_CustomerPricingProduct] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CustomerPricingProduct_CarrierAccount] FOREIGN KEY ([CustomerID]) REFERENCES [TOneWhS_BE].[CarrierAccount] ([ID]),
    CONSTRAINT [FK_CustomerPricingProduct_PricingProduct] FOREIGN KEY ([PricingProductID]) REFERENCES [TOneWhS_BE].[PricingProduct] ([ID])
);

