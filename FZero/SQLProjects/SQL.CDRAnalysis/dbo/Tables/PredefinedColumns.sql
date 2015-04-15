CREATE TABLE [dbo].[PredefinedColumns] (
    [ID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_PredefinedColumns] PRIMARY KEY CLUSTERED ([ID] ASC)
);

