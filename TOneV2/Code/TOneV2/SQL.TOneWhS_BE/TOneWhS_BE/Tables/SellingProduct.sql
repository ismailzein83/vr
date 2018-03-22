CREATE TABLE [TOneWhS_BE].[SellingProduct] (
    [ID]                      INT            IDENTITY (1, 1) NOT NULL,
    [Name]                    NVARCHAR (255) NOT NULL,
    [DefaultRoutingProductID] INT            NULL,
    [SellingNumberPlanID]     INT            NOT NULL,
    [Settings]                NVARCHAR (MAX) NULL,
    [timestamp]               ROWVERSION     NULL,
    [CreatedTime]             DATETIME       CONSTRAINT [DF_SellingProduct_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]               INT            NULL,
    [LastModifiedBy]          INT            NULL,
    [LastModifiedTime]        DATETIME       NULL,
    CONSTRAINT [PK_SellingProduct] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SellingProduct_RoutingProduct] FOREIGN KEY ([DefaultRoutingProductID]) REFERENCES [TOneWhS_BE].[RoutingProduct] ([ID]),
    CONSTRAINT [FK_SellingProduct_SellingNumberPlan] FOREIGN KEY ([SellingNumberPlanID]) REFERENCES [TOneWhS_BE].[SellingNumberPlan] ([ID])
);





