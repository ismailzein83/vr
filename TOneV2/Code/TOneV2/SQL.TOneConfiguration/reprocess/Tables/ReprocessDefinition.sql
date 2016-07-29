CREATE TABLE [reprocess].[ReprocessDefinition] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [timestamp]   ROWVERSION     NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_ReprocessDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_ReprocessDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_ReprocessDefinition_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

