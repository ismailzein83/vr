CREATE TABLE [CP_SupPriceList].[CustomerSupplierMapping] (
    [ID]              INT            IDENTITY (1, 1) NOT NULL,
    [CustomerID]      INT            NOT NULL,
    [UserID]          INT            NOT NULL,
    [MappingSettings] NVARCHAR (MAX) NOT NULL,
    [CreatedTime]     DATETIME       CONSTRAINT [DF_CustomerSupplierUser_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]       ROWVERSION     NULL,
    CONSTRAINT [PK_CustomerSupplierMapping] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CustomerSupplierMapping_Customer] FOREIGN KEY ([CustomerID]) REFERENCES [CP_SupPriceList].[Customer] ([ID]),
    CONSTRAINT [IX_CustomerSupplierMapping_CustUser] UNIQUE NONCLUSTERED ([CustomerID] ASC, [UserID] ASC)
);



