CREATE TABLE [FraudAnalysis].[AccountCase] (
    [ID]                INT           NOT NULL,
    [AccountNumber]     VARCHAR (50)  NOT NULL,
    [UserID]            INT           NULL,
    [Status]            INT           NOT NULL,
    [StatusUpdatedTime] DATETIME      NOT NULL,
    [ValidTill]         DATETIME      NULL,
    [CreatedTime]       DATETIME      CONSTRAINT [DF_AccountCase1_CreatedTime] DEFAULT (getdate()) NULL,
    [Reason]            VARCHAR (MAX) NULL,
    CONSTRAINT [PK_AccountCase] PRIMARY KEY CLUSTERED ([ID] ASC)
);


















GO



GO
CREATE NONCLUSTERED INDEX [IX_AccountCase_AccountNumber]
    ON [FraudAnalysis].[AccountCase]([AccountNumber] ASC);

