CREATE TABLE [Retail_BE].[AccountStatusHistory] (
    [ID]                    BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountBEDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [AccountID]             BIGINT           NOT NULL,
    [StatusID]              UNIQUEIDENTIFIER NOT NULL,
    [PreviousStatusID]      UNIQUEIDENTIFIER NULL,
    [StatusChangedDate]     DATETIME         NOT NULL,
    [IsDeleted]             BIT              NULL,
    [CreatedTime]           DATETIME         CONSTRAINT [DF_AccountStatusHistory_CreatedTime] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [IX_AccountStatusHistory_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);








GO
CREATE CLUSTERED INDEX [IX_AccountStatusHistory_AccountDefAndId]
    ON [Retail_BE].[AccountStatusHistory]([AccountBEDefinitionID] ASC, [AccountID] ASC);

