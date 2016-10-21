CREATE TABLE [reprocess].[ReprocessDefinition] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF__ReprocessDef__Id__16C440F5] DEFAULT (newid()) NOT NULL,
    [OldId]       INT              NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Settings]    NVARCHAR (MAX)   NULL,
    [timestamp]   ROWVERSION       NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_ReprocessDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [pk_ReprocessDefinition] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_ReprocessDefinition_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);





