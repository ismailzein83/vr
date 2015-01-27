﻿CREATE TABLE [dbo].[Products] (
    [ProductID]       INT           IDENTITY (1, 1) NOT NULL,
    [ProductName]     NVARCHAR (40) NOT NULL,
    [SupplierID]      INT           NULL,
    [CategoryID]      INT           NULL,
    [QuantityPerUnit] NVARCHAR (20) NULL,
    [UnitCost]        MONEY         NULL,
    [UnitPrice]       MONEY         CONSTRAINT [DF_Products_UnitPrice] DEFAULT ((0)) NULL,
    [UnitsInStock]    SMALLINT      CONSTRAINT [DF_Products_UnitsInStock] DEFAULT ((0)) NULL,
    [UnitsOnOrder]    SMALLINT      CONSTRAINT [DF_Products_UnitsOnOrder] DEFAULT ((0)) NULL,
    [ReorderLevel]    SMALLINT      CONSTRAINT [DF_Products_ReorderLevel] DEFAULT ((0)) NULL,
    [Discontinued]    BIT           CONSTRAINT [DF_Products_Discontinued] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([ProductID] ASC),
    CONSTRAINT [CK_Products_UnitPrice] CHECK ([UnitPrice]>=(0)),
    CONSTRAINT [CK_ReorderLevel] CHECK ([ReorderLevel]>=(0)),
    CONSTRAINT [CK_UnitsInStock] CHECK ([UnitsInStock]>=(0)),
    CONSTRAINT [CK_UnitsOnOrder] CHECK ([UnitsOnOrder]>=(0))
);

