CREATE TABLE [bp].[BPBusinessRuleDefinition] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (255)  NOT NULL,
    [BPDefintionId] INT            NOT NULL,
    [Settings]      NVARCHAR (MAX) NOT NULL,
    [timestamp]     ROWVERSION     NOT NULL,
    CONSTRAINT [PK_BPBusinessRuleDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);

