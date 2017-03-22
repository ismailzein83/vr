CREATE TABLE [VR_NumberingPlan].[SaleZone] (
    [ID]                  BIGINT         NOT NULL,
    [SellingNumberPlanID] INT            NOT NULL,
    [CountryID]           INT            NOT NULL,
    [Name]                NVARCHAR (255) NOT NULL,
    [BED]                 DATETIME       NOT NULL,
    [EED]                 DATETIME       NULL,
    [timestamp]           ROWVERSION     NULL,
    [SourceID]            VARCHAR (50)   NULL,
    [CreatedTime]         DATETIME       CONSTRAINT [DF_SaleZone_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_SaleZone] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SaleZone_SellingNumberPlan] FOREIGN KEY ([SellingNumberPlanID]) REFERENCES [VR_NumberingPlan].[SellingNumberPlan] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_SaleZone_timestamp]
    ON [VR_NumberingPlan].[SaleZone]([timestamp] DESC);

