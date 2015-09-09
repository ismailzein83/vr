CREATE TABLE [FraudAnalysis].[AccountCase] (
    [ID]                INT          IDENTITY (1, 1) NOT NULL,
    [AccountNumber]     VARCHAR (50) NOT NULL,
    [UserID]            INT          NOT NULL,
    [Status]            INT          NOT NULL,
    [StatusUpdatedTime] DATETIME     NOT NULL,
    [ValidTill]         DATETIME     NULL,
    [CreatedTime]       DATETIME     CONSTRAINT [DF_AccountCase1_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_AccountCase] PRIMARY KEY CLUSTERED ([ID] ASC)
);












GO



GO


