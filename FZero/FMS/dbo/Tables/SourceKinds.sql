CREATE TABLE [dbo].[SourceKinds] (
    [ID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (50) NULL,
    CONSTRAINT [PK_SourceKinds] PRIMARY KEY CLUSTERED ([ID] ASC)
);

