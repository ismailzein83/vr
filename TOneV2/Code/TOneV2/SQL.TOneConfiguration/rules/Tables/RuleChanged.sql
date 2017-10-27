CREATE TABLE [rules].[RuleChanged] (
    [ID]                    INT            IDENTITY (1, 1) NOT NULL,
    [RuleID]                INT            NOT NULL,
    [RuleTypeID]            INT            NOT NULL,
    [Data]                  NVARCHAR (MAX) NULL,
    [AdditionalInformation] NVARCHAR (MAX) NULL,
    [CreatedTime]           DATETIME       CONSTRAINT [DF_RuleChanged_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]             ROWVERSION     NULL,
    CONSTRAINT [PK_RuleChanged] PRIMARY KEY CLUSTERED ([ID] ASC)
);

