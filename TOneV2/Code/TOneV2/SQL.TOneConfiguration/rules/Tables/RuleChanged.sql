CREATE TABLE [rules].[RuleChanged] (
    [ID]                    INT            IDENTITY (1, 1) NOT NULL,
    [RuleID]                INT            NOT NULL,
    [RuleTypeID]            INT            NOT NULL,
    [ActionType]            INT            NOT NULL,
    [InitialRule]           NVARCHAR (MAX) NOT NULL,
    [AdditionalInformation] NVARCHAR (MAX) NOT NULL,
    [CreatedTime]           DATETIME       CONSTRAINT [DF_RuleChanged_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]             ROWVERSION     NULL,
    CONSTRAINT [PK_RuleChanged] PRIMARY KEY CLUSTERED ([ID] ASC)
);



