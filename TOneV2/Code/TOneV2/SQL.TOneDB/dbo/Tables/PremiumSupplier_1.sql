CREATE TABLE [dbo].[PremiumSupplier] (
    [PremiumSupplierID] INT            IDENTITY (1, 1) NOT NULL,
    [SupplierID]        VARCHAR (5)    NULL,
    [SupplierName]      NVARCHAR (100) NULL,
    CONSTRAINT [PK_PremiumSupplier] PRIMARY KEY CLUSTERED ([PremiumSupplierID] ASC)
);

