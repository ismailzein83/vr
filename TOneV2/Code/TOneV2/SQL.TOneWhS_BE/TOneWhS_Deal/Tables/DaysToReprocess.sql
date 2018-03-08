CREATE TABLE [TOneWhS_Deal].[DaysToReprocess] (
    [ID]               BIGINT NOT NULL,
    [Date]             DATE   NOT NULL,
    [IsSale]           BIT    NOT NULL,
    [CarrierAccountId] INT    NOT NULL,
    CONSTRAINT [PK_DaysToReprocess] PRIMARY KEY CLUSTERED ([ID] ASC)
);

