CREATE TABLE [TOneWhS_BE].[RoutingProduct] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (255) NOT NULL,
    [SellingNumberPlanID] INT            NOT NULL,
    [Settings]            NVARCHAR (MAX) NOT NULL,
    [timestamp]           ROWVERSION     NULL,
    [CreatedTime]         DATETIME       CONSTRAINT [DF_RoutingProduct_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_RoutingProduct] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_RoutingProduct_SellingNumberPlan] FOREIGN KEY ([SellingNumberPlanID]) REFERENCES [TOneWhS_BE].[SellingNumberPlan] ([ID])
);





