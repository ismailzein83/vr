CREATE TABLE [dbo].[SourceMappings] (
    [ID]                   INT           IDENTITY (1, 1) NOT NULL,
    [SourceID]             INT           NOT NULL,
    [ColumnName]           VARCHAR (100) NOT NULL,
    [MappedtoColumnNumber] INT           NOT NULL,
    [IncludeInCompare]     BIT           NOT NULL,
    [CreationDate]         DATETIME2 (0) CONSTRAINT [DF_SourceMappings_CreationDate] DEFAULT (getdate()) NULL,
    [CreatedBy]            INT           NULL,
    [LastUpdateDate]       DATETIME2 (0) NULL,
    [LastUpdatedBy]        INT           NULL,
    CONSTRAINT [PK_SourceMappings] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SourceMappings_ApplicationUsers] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[ApplicationUsers] ([ID]),
    CONSTRAINT [FK_SourceMappings_ApplicationUsers1] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [dbo].[ApplicationUsers] ([ID]),
    CONSTRAINT [FK_SourceMappings_PredefinedColumns] FOREIGN KEY ([MappedtoColumnNumber]) REFERENCES [dbo].[PredefinedColumns] ([ID]),
    CONSTRAINT [FK_SourceMappings_Sources] FOREIGN KEY ([SourceID]) REFERENCES [dbo].[Sources] ([ID])
);

