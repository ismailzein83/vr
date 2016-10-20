CREATE TABLE [reprocess].[ReprocessDefinition] (
    [Name]        NVARCHAR (255)   NOT NULL,
    [Settings]    NVARCHAR (MAX)   NULL,
    [timestamp]   ROWVERSION       NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_ReprocessDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [OldId]       INT              NULL,
    [Id]          UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    CONSTRAINT [pk_ReprocessDefinition] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_ReprocessDefinition_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);



