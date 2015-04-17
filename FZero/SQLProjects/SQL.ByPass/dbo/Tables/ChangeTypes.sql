CREATE TABLE [dbo].[ChangeTypes] (
    [ID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ChangeTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);

