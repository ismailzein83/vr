CREATE TABLE [NetworkRentalManager].[Product] (
    [ID]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NULL,
    [ProductType] INT            NULL,
    [CreatedTime] DATETIME       NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ID] ASC)
);

