CREATE TABLE [VR_NumberingPlan].[SaleCode] (
    [ID]          BIGINT       NOT NULL,
    [Code]        VARCHAR (20) NOT NULL,
    [ZoneID]      BIGINT       NOT NULL,
    [CodeGroupID] INT          NULL,
    [BED]         DATETIME     NOT NULL,
    [EED]         DATETIME     NULL,
    [timestamp]   ROWVERSION   NULL,
    [SourceID]    VARCHAR (50) NULL,
    [CreatedTime] DATETIME     CONSTRAINT [DF_SaleCode_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [FK_SaleCode_CodeGroup] FOREIGN KEY ([CodeGroupID]) REFERENCES [VR_NumberingPlan].[CodeGroup] ([ID]),
    CONSTRAINT [FK_SaleCode_SaleZone] FOREIGN KEY ([ZoneID]) REFERENCES [VR_NumberingPlan].[SaleZone] ([ID]),
    CONSTRAINT [IX_SaleCode_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_SaleCode_timestamp]
    ON [VR_NumberingPlan].[SaleCode]([timestamp] DESC);


GO
CREATE CLUSTERED INDEX [IX_SaleCode_Code]
    ON [VR_NumberingPlan].[SaleCode]([Code] ASC);

