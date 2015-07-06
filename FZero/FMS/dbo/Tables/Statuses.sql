CREATE TABLE [dbo].[Statuses] (
    [ID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_Statuses] PRIMARY KEY CLUSTERED ([ID] ASC)
);

