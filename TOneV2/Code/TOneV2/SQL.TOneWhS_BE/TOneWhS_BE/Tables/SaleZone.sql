CREATE TABLE [TOneWhS_BE].[SaleZone] (
    [ID]                  BIGINT         IDENTITY (1, 1) NOT NULL,
    [SellingNumberPlanID] INT            NOT NULL,
    [CountryID]           INT            NULL,
    [Name]                NVARCHAR (255) NOT NULL,
    [BED]                 DATETIME       NOT NULL,
    [EED]                 DATETIME       NULL,
    [timestamp]           ROWVERSION     NULL,
    CONSTRAINT [PK_SaleZone] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SaleZone_SellingNumberPlan] FOREIGN KEY ([SellingNumberPlanID]) REFERENCES [TOneWhS_BE].[SellingNumberPlan] ([ID])
);



