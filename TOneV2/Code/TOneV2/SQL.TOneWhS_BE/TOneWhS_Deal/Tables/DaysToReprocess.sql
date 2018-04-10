CREATE TABLE [TOneWhS_Deal].[DaysToReprocess] (
    [ID]               BIGINT IDENTITY (1, 1) NOT NULL,
    [Date]             DATE   NOT NULL,
    [IsSale]           BIT    NOT NULL,
    [CarrierAccountId] INT    NOT NULL,
    CONSTRAINT [PK_DaysToReprocess] PRIMARY KEY CLUSTERED ([ID] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_DaysToReprocess_Date]
    ON [TOneWhS_Deal].[DaysToReprocess]([Date] ASC);

