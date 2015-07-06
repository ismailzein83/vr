CREATE TABLE [dbo].[SourceMappings] (
    [ID]                   INT           IDENTITY (1, 1) NOT NULL,
    [SwitchID]             INT           NOT NULL,
    [ColumnName]           VARCHAR (100) NOT NULL,
    [MappedtoColumnNumber] INT           NOT NULL,
    [CreationDate]         DATETIME2 (0) CONSTRAINT [DF_SourceMappings_CreationDate] DEFAULT (getdate()) NULL,
    [CreatedBy]            INT           NULL,
    [LastUpdateDate]       DATETIME2 (0) NULL,
    [LastUpdatedBy]        INT           NULL,
    CONSTRAINT [PK_SourceMappings] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SourceMappings_PredefinedColumns] FOREIGN KEY ([MappedtoColumnNumber]) REFERENCES [dbo].[PredefinedColumns] ([ID]),
    CONSTRAINT [FK_SourceMappings_SwitchProfiles] FOREIGN KEY ([SwitchID]) REFERENCES [dbo].[SwitchProfiles] ([Id])
);

