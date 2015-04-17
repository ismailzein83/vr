CREATE TABLE [dbo].[ValueTypes] (
    [ID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ValueTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);

