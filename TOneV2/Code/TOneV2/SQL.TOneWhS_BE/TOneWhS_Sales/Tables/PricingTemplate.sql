CREATE TABLE [TOneWhS_Sales].[PricingTemplate] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (255) NULL,
    [SellingNumberPlanId] INT            NULL,
    [Settings]            NVARCHAR (MAX) NULL,
    [CreatedTime]         DATETIME       CONSTRAINT [DF_PricingTemplate_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]           ROWVERSION     NULL,
    CONSTRAINT [PK_PricingTemplate] PRIMARY KEY CLUSTERED ([ID] ASC)
);

