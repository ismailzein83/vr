CREATE TABLE [CP_SupPriceList].[CustomerUser] (
    [UserID]      INT        NOT NULL,
    [CustomerID]  INT        NOT NULL,
    [CreatedTime] DATETIME   CONSTRAINT [DF_CustomerUser_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION NULL,
    CONSTRAINT [PK_CustomerUser] PRIMARY KEY CLUSTERED ([UserID] ASC),
    CONSTRAINT [FK_CustomerUser_Customer] FOREIGN KEY ([CustomerID]) REFERENCES [CP_SupPriceList].[Customer] ([ID])
);



