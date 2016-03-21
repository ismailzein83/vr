CREATE TABLE [bp].[BPBusinessRuleActionType] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (MAX)  NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [timestamp]   ROWVERSION     NOT NULL,
    CONSTRAINT [PK_BPBusinessRuleActionType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

