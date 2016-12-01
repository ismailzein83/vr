CREATE TABLE [TOneWhS_BE].[CustomerSellingProduct] (
    [ID]               INT        IDENTITY (1, 1) NOT NULL,
    [CustomerID]       INT        NOT NULL,
    [SellingProductID] INT        NOT NULL,
    [AllDestinations]  BIT        NULL,
    [BED]              DATETIME   NOT NULL,
    [EED]              DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    [CreatedTime]      DATETIME   CONSTRAINT [DF_CustomerSellingProduct_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_CustomerSellingProduct] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CustomerSellingProduct_SellingProduct] FOREIGN KEY ([SellingProductID]) REFERENCES [TOneWhS_BE].[SellingProduct] ([ID])
);



