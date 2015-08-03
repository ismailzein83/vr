CREATE TABLE [dbo].[RoutingPoolSupplier] (
    [ID]         INT         NOT NULL,
    [SupplierID] VARCHAR (5) NOT NULL,
    [ZoneID]     INT         CONSTRAINT [DF_RoutingPoolSupplier_ZoneID] DEFAULT ((-1)) NOT NULL,
    CONSTRAINT [PK_RoutingPoolSupplier] PRIMARY KEY CLUSTERED ([ID] ASC, [SupplierID] ASC),
    CONSTRAINT [fk_RPS_ID] FOREIGN KEY ([ID]) REFERENCES [dbo].[RoutingPool] ([ID])
);

