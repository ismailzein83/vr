CREATE TABLE [dbo].[SourceTypes] (
    [ID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (50) NULL,
    CONSTRAINT [PK_SourceTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);

