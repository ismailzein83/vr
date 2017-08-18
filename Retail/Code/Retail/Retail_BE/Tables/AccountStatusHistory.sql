CREATE TABLE [Retail_BE].[AccountStatusHistory] (
    [ID]                    BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountBEDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [AccountID]             BIGINT           NOT NULL,
    [StatusID]              UNIQUEIDENTIFIER NOT NULL,
    [PreviousStatusID]      UNIQUEIDENTIFIER NULL,
    [StatusChangedDate]     DATETIME         NOT NULL,
    CONSTRAINT [PK_AccountStatusHistory] PRIMARY KEY CLUSTERED ([ID] ASC)
);





