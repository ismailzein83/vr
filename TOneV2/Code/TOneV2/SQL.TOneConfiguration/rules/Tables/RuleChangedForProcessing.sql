CREATE TABLE [rules].[RuleChangedForProcessing] (
    [ID]                    INT            NOT NULL,
    [RuleID]                INT            NOT NULL,
    [RuleTypeID]            INT            NOT NULL,
    [ActionType]            INT            NOT NULL,
    [InitialRule]           NVARCHAR (MAX) NULL,
    [AdditionalInformation] NVARCHAR (MAX) NULL,
    [CreatedTime]           DATETIME       NULL,
    [timestamp]             ROWVERSION     NULL,
    CONSTRAINT [PK_RuleChangedForProcessing] PRIMARY KEY CLUSTERED ([ID] ASC)
);

