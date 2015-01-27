CREATE TABLE [dbo].[RoutingPoolCustomer] (
    [ID]         INT         NOT NULL,
    [CustomerID] VARCHAR (5) NOT NULL,
    CONSTRAINT [PK_RoutingPoolCustomer] PRIMARY KEY CLUSTERED ([ID] ASC, [CustomerID] ASC),
    CONSTRAINT [fk_RPC_ID] FOREIGN KEY ([ID]) REFERENCES [dbo].[RoutingPool] ([ID])
);

