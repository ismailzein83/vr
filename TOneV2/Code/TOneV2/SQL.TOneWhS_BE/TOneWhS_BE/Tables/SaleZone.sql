CREATE TABLE [TOneWhS_BE].[SaleZone] (
    [ID]                  BIGINT         NOT NULL,
    [SellingNumberPlanID] INT            NOT NULL,
    [CountryID]           INT            NOT NULL,
    [Name]                NVARCHAR (255) NOT NULL,
    [BED]                 DATETIME       NOT NULL,
    [EED]                 DATETIME       NULL,
    [timestamp]           ROWVERSION     NULL,
    [SourceID]            VARCHAR (50)   NULL,
    [ProcessInstanceID]   BIGINT         NULL,
    CONSTRAINT [PK_SaleZone] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SaleZone_SellingNumberPlan] FOREIGN KEY ([SellingNumberPlanID]) REFERENCES [TOneWhS_BE].[SellingNumberPlan] ([ID])
);












GO
CREATE NONCLUSTERED INDEX [IX_SaleZone_timestamp]
    ON [TOneWhS_BE].[SaleZone]([timestamp] DESC);

