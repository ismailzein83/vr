CREATE TABLE [dbo].[Imports] (
    [ID]           INT           IDENTITY (1, 1) NOT NULL,
    [ImportDate]   DATETIME2 (0) NOT NULL,
    [ImportedBy]   INT           NULL,
    [ImportTypeId] INT           NOT NULL,
    CONSTRAINT [PK_Imports] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Imports_ApplicationUsers] FOREIGN KEY ([ImportedBy]) REFERENCES [dbo].[ApplicationUsers] ([ID]),
    CONSTRAINT [FK_Imports_ImportTypes] FOREIGN KEY ([ImportTypeId]) REFERENCES [dbo].[ImportTypes] ([ID])
);

